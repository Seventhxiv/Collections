using System.Text.RegularExpressions;
using Collections;
using FFXIVClientStructs.FFXIV.Common.Lua;

// Provides collections a way to specify ways of sorting 
public class CollectibleSortOption
{
    public CollectibleSortOption(string Name, Func<ICollectible, dynamic> keySelector, bool Reverse = false, (FontAwesomeIcon AscendingIcon, FontAwesomeIcon DescendingIcon)? Icons = null)
    {
        this.Name = Name;
        this.ReverseDefault = Reverse;
        this.Reverse = Reverse;
        this.GetSortValue = keySelector;
        this.Comparer = Comparer<ICollectible>.Create((c1, c2) => keySelector(c1).CompareTo(keySelector(c2)));
        if (Icons != null)
        {
            ascendingIcon = Icons.Value.AscendingIcon;
            descendingIcon = Icons.Value.DescendingIcon;
        }
    }

    // name of the sort option
    public string Name { get; set; }
    // Default setting this sort option was created with for reverse
    public bool ReverseDefault { get; init; }
    // whether the sorted items should be returned in reverse order
    public bool Reverse { get; set; }
    // how items should be sorted against one another.
    public Comparer<ICollectible> Comparer { get; init; }
    // optional parameter to allow sort options to determine headers when displaying items.
    public Func<ICollectible, dynamic> GetSortValue { get; init; }
    // optional icon to represent sorting
    private FontAwesomeIcon ascendingIcon = FontAwesomeIcon.SortUp;
    // optional icon to represent reverse sorting
    private FontAwesomeIcon descendingIcon = FontAwesomeIcon.SortDown;
    public FontAwesomeIcon GetSortIcon() => Reverse ? ascendingIcon : descendingIcon;
    public OrderedParallelQuery<ICollectible> SortCollection(IEnumerable<ICollectible> collection)
    {
        // can't just call reverse, causes the favorites drop down to bottom.
        if (Reverse) return collection.AsParallel().OrderByDescending(c => c, Comparer).ThenByDescending(c => c.GetCollectionName()).ThenByDescending(c => c.Id);
        return collection.AsParallel().OrderBy(c => c, Comparer).ThenBy(c => c.GetCollectionName()).ThenBy(c => c.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != this.GetType()) return false;
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
