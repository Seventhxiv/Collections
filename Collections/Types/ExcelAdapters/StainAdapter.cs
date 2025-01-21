namespace Collections;

[Sheet("Stain")]
public struct StainAdapter(ExcelPage page, uint offset, uint row) : IExcelRow<StainAdapter>
{
    static StainAdapter IExcelRow<StainAdapter>.Create(ExcelPage page, uint offset, uint row)
    {
        var obj = new StainAdapter(page, offset, row);
        obj.PopulateData();
        return obj;
    }

    public void PopulateData()
    {
        if (Color != 0)
        {
            HEXcolor = StainColorConverter.DecimalToHex((int)Color);
            RGBcolor = StainColorConverter.HexToRGB(HEXcolor);
            VecColor = new Vector4(RGBcolor.R / 255f, RGBcolor.G / 255f, RGBcolor.B / 255f, 1);
        }
    }

    public string HEXcolor { get; set; }
    public RGBColor RGBcolor { get; set; }
    public Vector4 VecColor { get; set; }

    // Original
    public uint RowId => row;

    public readonly ReadOnlySeString Name => page.ReadString(offset, offset);
    public readonly ReadOnlySeString Name2 => page.ReadString(offset + 4, offset);
    public readonly uint Color => page.ReadUInt32(offset + 8);
    public readonly byte Shade => page.ReadUInt8(offset + 12);
    public readonly byte SubOrder => page.ReadUInt8(offset + 13);
    public readonly bool Unknown1 => page.ReadPackedBool(offset + 14, 0);
    public readonly bool Unknown2 => page.ReadPackedBool(offset + 14, 1);
}
