using System.Collections;
using System.Collections.Concurrent;

namespace Collections;

public class ExcelCache<T> : IEnumerable<T> where T : ExcelRow
{
    private static ConcurrentDictionary<Dalamud.ClientLanguage, ExcelCache<T>> InternalInstance = new();

    private ExcelSheet<T> excelSheet { get; set; }
    //private readonly ConcurrentDictionary<uint, T> rowCache = new();
    //private readonly ConcurrentDictionary<Tuple<uint, uint>, T> subRowCache = new();

    private ExcelCache(Dalamud.ClientLanguage language)
    {
        excelSheet = Services.DataManager.GetExcelSheet<T>(language);
    }

    public static ExcelCache<T> GetSheet(Dalamud.ClientLanguage? language = null)
    {
        var sheetLanguage = language is not null ? (Dalamud.ClientLanguage)language : Services.DataManager.Language;
        if (InternalInstance.TryGetValue(sheetLanguage, out var instance))
        {
            return instance;
        }
        return InternalInstance[sheetLanguage] = new ExcelCache<T>(sheetLanguage);
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
