namespace Collections;

public interface ICollectible
{
    public string Name { get; init; }
    public uint Id { get; init; }
    public string PrimaryDescription { get; init; }
    public string SecondaryDescription { get; init; }
    public ICollectibleKey CollectibleKey { get; init; }
    public bool IsFavorite();
    public void SetFavorite(bool favorite);
    public bool IsWishlist();
    public void SetWishlist(bool wishlist);
    public void OpenGamerEscape();
    public bool GetIsObtained();
    public void UpdateObtainedState();
    public IDalamudTextureWrap GetIconLazy();
    public void Interact();
}