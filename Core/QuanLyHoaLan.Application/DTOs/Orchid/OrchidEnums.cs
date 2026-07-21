using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;

namespace QuanLyHoaLan.Application.DTOs.Orchid;

[TypeConverter(typeof(RegionTypeConverter))]
public enum Region
{
    [JsonStringEnumMemberName("Trung trung bộ và Miền núi phía Bắc")]
    NORTHERN_MIDLANDS_AND_MOUNTAINS,

    [JsonStringEnumMemberName("Đồng bằng sông Hồng")]
    RED_RIVER_DELTA,

    [JsonStringEnumMemberName("Bắc Trung Bộ")]
    NORTH_CENTRAL,

    [JsonStringEnumMemberName("Duyên hải Nam Trung Bộ")]
    SOUTH_CENTRAL_COAST,

    [JsonStringEnumMemberName("Tây Nguyên")]
    CENTRAL_HIGHLANDS,

    [JsonStringEnumMemberName("Đông Nam Bộ")]
    SOUTHEAST,

    [JsonStringEnumMemberName("Đồng bằng sông Cửu Long")]
    MEKONG_DELTA
}

[TypeConverter(typeof(BloomSeasonTypeConverter))]
public enum BloomSeason
{
    [JsonStringEnumMemberName("Mùa xuân")]
    SPRING,

    [JsonStringEnumMemberName("Mùa hạ")]
    SUMMER,

    [JsonStringEnumMemberName("Mùa thu")]
    AUTUMN,

    [JsonStringEnumMemberName("Mùa đông")]
    WINTER,

    [JsonStringEnumMemberName("Quanh năm")]
    ALL_YEAR
}

[TypeConverter(typeof(FlowerColorTypeConverter))]
public enum FlowerColor
{
    [JsonStringEnumMemberName("#FF0000")]
    RED,

    [JsonStringEnumMemberName("#FFA500")]
    ORANGE,

    [JsonStringEnumMemberName("#FFD700")]
    YELLOW,

    [JsonStringEnumMemberName("#FFFFFF")]
    WHITE,

    [JsonStringEnumMemberName("#FFC0CB")]
    PINK,

    [JsonStringEnumMemberName("#800080")]
    PURPLE,

    [JsonStringEnumMemberName("#008000")]
    GREEN,

    [JsonStringEnumMemberName("#90EE90")]
    LIGHT_GREEN,

    [JsonStringEnumMemberName("#0000FF")]
    BLUE,

    [JsonStringEnumMemberName("#FFFDD0")]
    CREAM,

    [JsonStringEnumMemberName("#A52A2A")]
    BROWN,

    [JsonStringEnumMemberName("#000000")]
    BLACK
}

public static class OrchidEnumValue
{
    public static string ToStorageValue<TEnum>(this TEnum value)
        where TEnum : struct, Enum
    {
        var member = typeof(TEnum).GetMember(value.ToString()).Single();
        return member.GetCustomAttribute<JsonStringEnumMemberNameAttribute>()?.Name
            ?? value.ToString();
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

        foreach (var candidate in Enum.GetValues<TEnum>())
        {
            if (string.Equals(candidate.ToStorageValue(), normalized, StringComparison.OrdinalIgnoreCase))
            {
                result = candidate;
                return true;
            }
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
