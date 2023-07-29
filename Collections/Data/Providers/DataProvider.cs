using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collections;

public class DataProvider
{
    //private ExcelSheet<Mount> mountSheet { get; set; }
    //public List<Collectible<Mount>> mountList = new();
    public Dictionary<string, List<ICollectible>> Collections = new();

    public DataProvider()
    {
        Dev.Start();
        //mountSheet = Excel.GetExcelSheet<Mount>()!;
        Task.Run(() => PopulateData());
        Dev.Stop();
    }

    private void PopulateData()
    {
        //var y = new List<Type>() { typeof(MountCollectible) };
        Collections[MountCollectible.GetCollectionName()] = MountCollectible.GetCollection();
    }

    //private List<Collectible<T>> GetCollection<T>() where T : ExcelRow
    //{

    //}
}
