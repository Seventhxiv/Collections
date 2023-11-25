using System.Collections;
using System.Collections.Concurrent;

namespace Collections;

public class ExcelCache<T> : IEnumerable<T> where T : ExcelRow
{
    private static ExcelCache<T>? InternalInstance;
    public static ExcelCache<T> Instance => InternalInstance ??= new ExcelCache<T>();

    private ExcelSheet<T> excelSheet { get; set; }
    private readonly ConcurrentDictionary<uint, T> rowCache = new();
    private readonly ConcurrentDictionary<Tuple<uint, uint>, T> subRowCache = new();

    private ExcelCache()
    {
        excelSheet = Services.DataManager.GetExcelSheet<T>(Dalamud.ClientLanguage.English);
    }
    public static ExcelCache<T> GetSheet()
    {
        return Instance;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return excelSheet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public T? GetRow(uint id)
    {
        return excelSheet.GetRow(id);
        //if (rowCache.TryGetValue(id, out var value))
        //{
        //    return value;
        //}
        //if (excelSheet.GetRow(id) is not { } result) return null;

        //return rowCache[id] = result;
    }

    public T? GetRow(uint row, uint subRow)
    {
        return excelSheet.GetRow(row, subRow);
        //var targetRow = new Tuple<uint, uint>(row, subRow);

        //if (subRowCache.TryGetValue(targetRow, out var value))
        //{
        //    return value;
        //}
        //if (excelSheet.GetRow(row, subRow) is not { } result) return null;

        //return subRowCache[targetRow] = result;
    }
}
