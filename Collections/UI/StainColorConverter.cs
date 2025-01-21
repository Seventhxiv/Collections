namespace Collections;

public struct RGBColor
{
    public int R;
    public int G;
    public int B;
}

public class StainColorConverter
{
    // Find the closest stain based on a given color
    public static StainAdapter FindClosestStain(Vector3 color)
    {
        var selectedColor = Vector3ToRGBColor(color);
        var minDistance = double.MaxValue;
        StainAdapter? closestStain = null;

        foreach (var stain in Services.DataProvider.SupportedStains)
        {
            var distance = CalculateDistance(selectedColor, stain.RGBcolor);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestStain = stain;
            }
        }

        return (StainAdapter)closestStain!;
    }

    // Convert a HEX color string to an RGBColor
    public static RGBColor HexToRGB(string hexColor)
    {
        if (hexColor.Length != 6)
        {
            throw new ArgumentException("Invalid HEX color string.", nameof(hexColor));
        }

        var r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
        var g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
        var b = Convert.ToInt32(hexColor.Substring(4, 2), 16);

        return new RGBColor { R = r, G = g, B = b };
    }

    // Calculate the Euclidean distance between two colors
    private static double CalculateDistance(RGBColor color1, RGBColor color2)
    {
        var deltaR = color1.R - color2.R;
        var deltaG = color1.G - color2.G;
        var deltaB = color1.B - color2.B;

        return Math.Sqrt((deltaR * deltaR) + (deltaG * deltaG) + (deltaB * deltaB));
    }

    public static string DecimalToHex(int decimalNumber)
    {
        if (decimalNumber == 0)
            return "0";

        var hexDigits = "0123456789ABCDEF";
        var hexString = "";

        while (decimalNumber > 0)
        {
            var remainder = decimalNumber % 16;
            hexString = hexDigits[remainder] + hexString;
            decimalNumber /= 16;
        }

        // Pad zeros to the left if necessary to ensure a length of 6 characters
        var paddingLength = 6 - hexString.Length;
        if (paddingLength > 0)
            hexString = new string('0', paddingLength) + hexString;

        return hexString;
    }

    private static RGBColor Vector3ToRGBColor(Vector3 vec)
    {
        return new RGBColor() { R = (int)(vec.X * 255), G = (int)(vec.Y * 255), B = (int)(vec.Z * 255) };
    }
}
