namespace QuanLyHoaLan.Domain.Common;

public class FindResult<T>
{
    public int TotalCount { get; set; }
    public List<T> Items { get; set; } = new List<T>();
}
