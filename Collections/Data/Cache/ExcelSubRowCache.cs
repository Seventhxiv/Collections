using System.Collections;
using System.Collections.Concurrent;

namespace Collections;

public class ExcelSubRowCache<T> : IEnumerable<SubrowCollection<T>> where T : struct, IExcelSubrow<T>
{
    private static ConcurrentDictionary<Dalamud.Game.ClientLanguage, ExcelSubRowCache<T>> InternalInstance = new();

    private SubrowExcelSheet<T> excelSheet { get; set; }
    //private readonly ConcurrentDictionary<uint, T> rowCache = new();
    //private readonly ConcurrentDictionary<Tuple<uint, uint>, T> subRowCache = new();

    private ExcelSubRowCache(Dalamud.Game.ClientLanguage language)
    {
        excelSheet = Services.DataManager.GetSubrowExcelSheet<T>(language);
    }

    public static ExcelSubRowCache<T> GetSheet(Dalamud.Game.ClientLanguage? language = null)
    {
        var sheetLanguage = language is not null ? (Dalamud.Game.ClientLanguage)language : Services.DataManager.Language;
        if (InternalInstance.TryGetValue(sheetLanguage, out var instance))
        {
            return instance;
        }
        return InternalInstance[sheetLanguage] = new ExcelSubRowCache<T>(sheetLanguage);
    }

    public IEnumerator<SubrowCollection<T>> GetEnumerator()
    {
        return excelSheet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public SubrowCollection<T> GetRow(uint id)
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
        return excelSheet.GetSubrow(row, (ushort)subRow);
        //var targetRow = new Tuple<uint, uint>(row, subRow);

        //if (subRowCache.TryGetValue(targetRow, out var value))
        //{
        //    return value;
        //}
        //if (excelSheet.GetRow(row, subRow) is not { } result) return null;

        //return subRowCache[targetRow] = result;
    }
}
