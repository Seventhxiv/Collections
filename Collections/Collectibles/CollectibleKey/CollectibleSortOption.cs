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