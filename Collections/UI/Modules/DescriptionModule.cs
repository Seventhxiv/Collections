namespace Collections;

public class HintModule
{
    public string Description { get; init; }
    public FontAwesomeIcon? Icon { get; init; }
    public HintModule(string description, FontAwesomeIcon? icon)
    {
        Description = description;
        Icon = icon;
    }
}