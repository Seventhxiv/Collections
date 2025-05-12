using Collections;

// Provides collections a way to specify ways of sorting 
public class CollectibleSortOption
{
    public CollectibleSortOption(string Name, Comparer<ICollectible> Comparer, bool Reverse = false, (FontAwesomeIcon AscendingIcon, FontAwesomeIcon DescendingIcon)? Icons = null)
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
    // name of the sort option
    public string Name {get; set;}    
    // whether the sorted items should be returned in reverse order
    public bool Reverse {get; set;}
    // how items should be sorted against one another.
    public Comparer<ICollectible> Comparer {get; set;}
    // optional icon to represent sorting
    private FontAwesomeIcon ascendingIcon = FontAwesomeIcon.SortUp;
    // optional icon to represent reverse sorting
    private FontAwesomeIcon descendingIcon = FontAwesomeIcon.SortDown;
    public FontAwesomeIcon GetSortIcon() => Reverse ? ascendingIcon : descendingIcon;
    public OrderedParallelQuery<ICollectible> SortCollection(IEnumerable<ICollectible> collection)
    {
        // can't just call reverse, causes the favorites to drop down to bottom.
        if(Reverse) return collection.AsParallel().OrderByDescending(c => c.IsFavorite()).ThenByDescending(c => c, Comparer).ThenByDescending(c => c.Id);
        return collection.AsParallel().OrderByDescending(c => c.IsFavorite()).ThenBy(c => c, Comparer).ThenBy(c => c.Id);
    }

    public override bool Equals(object? obj)
    {
        if(obj == null || obj.GetType() != this.GetType()) return false;
        var other = (CollectibleSortOption)obj;
        return this.Name == other.Name && this.ascendingIcon == other.ascendingIcon && this.descendingIcon == other.descendingIcon && this.Reverse == other.Reverse;
    }

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            hash = hash * 23 + Name.GetHashCode();
            hash = hash * 23 + Reverse.GetHashCode();
            hash = hash * 23 + ascendingIcon.GetHashCode();
            hash = hash * 23 + descendingIcon.GetHashCode();
            return hash;
        }
    }
}