using System.ComponentModel;
using System.Globalization;

namespace QuanLyHoaLan.Application.DTOs.Orchid;

[TypeConverter(typeof(RegionTypeConverter))]
public enum Region
{
    NORTHERN_MIDLANDS_AND_MOUNTAINS,
    RED_RIVER_DELTA,
    NORTH_CENTRAL,
    SOUTH_CENTRAL_COAST,
    CENTRAL_HIGHLANDS,
    SOUTHEAST,
    MEKONG_DELTA
}

[TypeConverter(typeof(BloomSeasonTypeConverter))]
public enum BloomSeason
{
    SPRING,
    SUMMER,
    AUTUMN,
    WINTER,
    ALL_YEAR
}

[TypeConverter(typeof(FlowerColorTypeConverter))]
public enum FlowerColor
{
    RED,
    ORANGE,
    YELLOW,
    WHITE,
    PINK,
    PURPLE,
    GREEN,
    LIGHT_GREEN,
    BLUE,
    CREAM,
    BROWN,
    BLACK
}

public static class OrchidEnumValue
{
    public static string ToStorageValue<TEnum>(this TEnum value)
        where TEnum : struct, Enum
    {
        return value.ToString();
    }

    public static bool TryParse<TEnum>(string? value, out TEnum result)
        where TEnum : struct, Enum
    {
        result = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim();
        var enumName = Enum.GetNames<TEnum>()
            .FirstOrDefault(name => string.Equals(name, normalized, StringComparison.OrdinalIgnoreCase));
        if (enumName != null && Enum.TryParse(enumName, out result))
        {
            return true;
        }

        return false;
    }

    public static List<TEnum> ParseStoredValues<TEnum>(IEnumerable<string>? values)
        where TEnum : struct, Enum
    {
        if (values == null)
        {
            return new List<TEnum>();
        }

        return values
            .Select(value => TryParse<TEnum>(value, out var parsed) ? parsed : (TEnum?)null)
            .Where(value => value.HasValue)
            .Select(value => value!.Value)
            .Distinct()
            .ToList();
    }
}

public abstract class OrchidEnumTypeConverter<TEnum> : EnumConverter
    where TEnum : struct, Enum
{
    protected OrchidEnumTypeConverter() : base(typeof(TEnum))
    {
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object value)
    {
        if (value is string text && OrchidEnumValue.TryParse<TEnum>(text, out var parsed))
        {
            return parsed;
        }

        if (value is string)
        {
            throw new FormatException($"Giá trị '{value}' không hợp lệ cho {typeof(TEnum).Name}.");
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object? value,
        Type destinationType)
    {
        if (destinationType == typeof(string) && value is TEnum enumValue)
        {
            return enumValue.ToStorageValue();
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}

public sealed class RegionTypeConverter : OrchidEnumTypeConverter<Region>
{
}

public sealed class BloomSeasonTypeConverter : OrchidEnumTypeConverter<BloomSeason>
{
}

public sealed class FlowerColorTypeConverter : OrchidEnumTypeConverter<FlowerColor>
{
}
