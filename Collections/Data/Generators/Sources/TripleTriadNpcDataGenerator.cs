namespace Collections;

public class TripleTriadNpcDataGenerator : BaseDataGenerator<ENpcResident>
{
    protected override void InitializeData()
    {
        foreach (var tripleTriadCardResident in ExcelCache<TripleTriadCardResident>.GetSheet())
        {
            if (tripleTriadCardResident.AcquisitionType == 6 || tripleTriadCardResident.AcquisitionType == 10)
            {
                if (tripleTriadCardResident.Acquisition.RowId != 0)
                {
                    AddEntry(tripleTriadCardResident.RowId, ExcelCache<ENpcResident>.GetSheet().GetRow(tripleTriadCardResident.Acquisition.RowId).Value);
                }
            }
        }
    }
}

