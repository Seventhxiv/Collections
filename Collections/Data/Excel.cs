namespace Collections;

public class Excel
{
    public static ExcelSheet<T> GetExcelSheet<T>() where T : ExcelRow
    {
        //return Services.DataManager.GetExcelSheet<T>();
        return Excel<T>.Instance.GetExcelSheet();
    }
}

public class Excel<T> where T : ExcelRow
{
    private static Excel<T>? instance;
    public static Excel<T> Instance => instance ??= new Excel<T>();

    private ExcelSheet<T> sheetCache { get; set; }

    public ExcelSheet<T> GetExcelSheet()
    {
        // TODO ideally no code should depend on the Language, need to do some more testing to make sure that's the case, for now assuming English
        return sheetCache ??= Services.DataManager.GetExcelSheet<T>(Dalamud.ClientLanguage.English);
    }
}
