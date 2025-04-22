using Collections;

// Provides collections a way to specify ways of sorting 
public class CollectibleSortOption
{
    public CollectibleSortOption(string Name, Comparer<ICollectible> Comparer, bool Reverse, (FontAwesomeIcon AscendingIcon, FontAwesomeIcon DescendingIcon)? Icons)
    {
        this.Name = Name;
        this.Reverse = Reverse;
        this.Comparer = Comparer;
        if(Icons != null)
        {
            ascendingIcon = Icons.Value.AscendingIcon;
            descendingIcon = Icons.Value.DescendingIcon;
        }
    }
    public string Name {get; set;}    

    public bool Reverse {get; set;}

    public Comparer<ICollectible> Comparer {get; set;}
    private FontAwesomeIcon ascendingIcon = FontAwesomeIcon.SortUp;
    private FontAwesomeIcon descendingIcon = FontAwesomeIcon.SortDown;
    public FontAwesomeIcon GetSortIcon() => Reverse ? ascendingIcon : descendingIcon;
    public List<ICollectible> SortCollection(List<ICollectible> collection)
    {
        var temp = collection.AsParallel().OrderByDescending(c => !c.IsFavorite()).ThenBy(c => c, Comparer);
        if(Reverse) return temp.Reverse().ToList();
        return temp.ToList();
    }
}