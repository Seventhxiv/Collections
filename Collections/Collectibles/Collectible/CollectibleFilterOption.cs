using Collections;

public class CollectibleFilterOption
{
    /// <summary>
    /// Represents an option for filtering collectibles
    /// </summary>
    /// <param name="Name"> name of the filter option </param>
    /// <param name="Filter"> function to determine whether the collectible should be filtered.
    /// this should return true if the collectible should be filtered. </param>
    /// <param name="Description"> optional description of the filter option. </param>
    /// <param name="Enabled"> whether this filter option should be applied. </param>
    public CollectibleFilterOption(string Name, Func<ICollectible, bool> Filter, string Description = "", bool Enabled = false)
    {
        this.Name = Name;
        this.Enabled = Enabled;
        this.Description = Description;
        this.Filter = Filter;
    }
    public string Name {get; set;}
    public bool Enabled {get; set;}
    public string Description{get; set;}
    public Func<ICollectible, bool> Filter {get; set;}

    public bool IsFiltered(ICollectible collectible)
    {
        return Enabled && Filter(collectible);
    }

    public IEnumerable<ICollectible> FilterCollection(IEnumerable<ICollectible> collection)
    {
        return collection.AsParallel().Where(c => !Filter(c));
    }
}