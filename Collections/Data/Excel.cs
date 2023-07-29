using Lumina.Excel;

namespace Collections;

public class Excel
{
    public static ExcelSheet<T> GetExcelSheet<T>() where T : ExcelRow
    {
        return Services.DataManager.GetExcelSheet<T>();
    }
}
