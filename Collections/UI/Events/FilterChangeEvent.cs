namespace Collections;

public class FilterChangeEventArgs : EventArgs
{
    public CollectibleStatusFilter? itemStatusFilter = null;
    public CollectibleSortOption? sortOption = null;
    public string? searchFilter = null;
    public FilterChangeEventArgs(
        CollectibleStatusFilter? itemStatusFilter = null, CollectibleSortOption? sortOptionSelected = null, string? searchFilter = null
    )
    {
        this.itemStatusFilter = itemStatusFilter;
        this.sortOption = sortOptionSelected;
        this.searchFilter = searchFilter;
    }

}

public class FilterChangeEvent : Event<FilterChangeEventArgs>
{
}

public enum CollectibleStatusFilter
{
    All,
    Obtained,
    Unobtained,
    Favorite,
}