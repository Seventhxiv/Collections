//namespace Collections;

//public class SourcesFactory
//{
//    public static List<ICollectibleSource> Get(Key key, uint id)
//    {
//        var SourceDataAggregator2 = new SourceDataAggregator2();
//        var sourceDict = SourceDataAggregator2.GetSources<uint>(key, id);
//        var result = new List<ICollectibleSource>();

//        foreach (var (source, set) in sourceDict)
//        {
//            switch (source)
//            {
//                case Source.Achievement:
//                    result.AddRange(set.Select(entry => new AchievementSource(ExcelCache<Achievement>.GetSheet().GetRow(entry))));
//                    break;
//                default:
//                    throw new ArgumentException();
//            }
//        }

//        return result;
//    }
//}

//public class CraftingDataGenerator2 : SourceDataGenerator2<Key, uint, Source, uint>
//{
//    protected override void InitializeData()
//    {
//        var recipes = ExcelCache<Recipe>.GetSheet();
//        foreach (var recipe in recipes)
//        {
//            var itemId = recipe.ItemResult.Row;
//            AddEntry(Key.Item, Source.Crafting, itemId, recipe.RowId);
//        }
//    }
//}

//public abstract class SourceDataGenerator2<TLayer, TIdentifier, TLookup, TResult> : IDataGenerator
//{
//    public readonly Dictionary<TLayer, Dictionary<TIdentifier, Dictionary<TLookup, HashSet<TResult>>>> data = new();

//    protected abstract void InitializeData();

//    public SourceDataGenerator2()
//    {
//        InitializeData();
//    }

//    public Dictionary<TLookup, HashSet<TResult>>? GetLookups(TLayer layer, TIdentifier id)
//    {
//        if (data.TryGetValue(layer, out var identifiers))
//        {
//            if (identifiers.TryGetValue(id, out var lookups))
//            {
//                return lookups;
//            }
//        }
//        return null;
//    }

//    protected void AddEntry(TLayer layer, TLookup lookup, TIdentifier id, TResult result)
//    {
//        //if (!data.ContainsKey(key))
//        //    data[key] = new Dictionary<uint, HashSet<T>>();

//        //data[key][source].Add(sourceId);
//    }
//}



////public abstract class SourceDataAggregator<T>
////{
////    public readonly Dictionary<Key, Dictionary<Source, HashSet<T>>> data = new();

////    protected abstract List<SourceDataGenerator<T>> GetDataGenerators();

////    public SourceDataAggregator()
////    {
////        var dataGenerators = GetDataGenerators();
////        Aggregate(dataGenerators);
////    }

////    public T GetEntry()
////    {

////    }

////    private void Aggregate(List<SourceDataGenerator<T>> dataGenerators)
////    { 
////        foreach (var generator in dataGenerators)
////        {
////            foreach (var (key, keyDict) in generator.data)
////            {
////                foreach (var (source, set) in keyDict)
////                {
////                    data[key][source].UnionWith(set);
////                }
////            }
////        }
////    }
////}

////public class IntSourceDataAggregator : SourceDataAggregator<uint>
////{
////    protected override List<SourceDataGenerator<uint>> GetDataGenerators()
////    {
////        return new List<SourceDataGenerator<uint>>()
////        {
////            new CraftingDataGenerator(),
////        };
////    }
////}