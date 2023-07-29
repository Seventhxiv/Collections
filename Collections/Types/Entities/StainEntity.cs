using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System.Numerics;

namespace Collections;

[Sheet("Stain")]
public class StainEntity : Stain
{
    public string HEXcolor { get; set; }
    public RGBColor RGBcolor { get; set; }
    public Vector4 VecColor { get; set; }
    public override void PopulateData(RowParser parser, Lumina.GameData lumina, Language language)
    {
        base.PopulateData(parser, lumina, language);
        if (Color != 0)
        {
            HEXcolor = ColorHelper.DecimalToHex((int)Color);
            RGBcolor = ColorHelper.HexToRGB(HEXcolor);
            VecColor = new Vector4(RGBcolor.R / 255f, RGBcolor.G / 255f, RGBcolor.B / 255f, 1);
        }
    }
}
