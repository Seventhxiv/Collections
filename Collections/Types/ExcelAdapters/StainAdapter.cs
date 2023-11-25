namespace Collections;

[Sheet("Stain")]
public class StainAdapter : Stain
{
    public string HEXcolor { get; set; }
    public RGBColor RGBcolor { get; set; }
    public Vector4 VecColor { get; set; }
    public override void PopulateData(RowParser parser, Lumina.GameData lumina, Language language)
    {
        base.PopulateData(parser, lumina, language);
        if (Color != 0)
        {
            HEXcolor = StainColorConverter.DecimalToHex((int)Color);
            RGBcolor = StainColorConverter.HexToRGB(HEXcolor);
            VecColor = new Vector4(RGBcolor.R / 255f, RGBcolor.G / 255f, RGBcolor.B / 255f, 1);
        }
    }
}
