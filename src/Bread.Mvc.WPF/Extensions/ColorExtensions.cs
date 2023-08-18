using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace Bread.Mvc.WPF;

public static class ColorExtensions
{
    public static SolidColorBrush ChangeLuminance(this Color color, double factor)
    {
        double h, s, v;

        double r = color.R;
        double g = color.G;
        double b = color.B;

        Rgb2Hsv(r, g, b, out h, out s, out v);
        v *= factor;
        v = Math.Min(v, 100);
        HsvToRgb(h, s, v, out r, out g, out b);

        Color c = Color.FromArgb(color.A, (byte)r, (byte)g, (byte)b);
        return new SolidColorBrush(c);
    }

    public static void Rgb2Hsv(double r, double g, double b, out double h, out double s, out double v)
    {
        // R, G, B values are divided by 255 
        // to change the range from 0..255 to 0..1 
        r = r / 255.0;
        g = g / 255.0;
        b = b / 255.0;

        // h, s, v = hue, saturation, value 
        double cmax = Math.Max(r, Math.Max(g, b)); // maximum of r, g, b 
        double cmin = Math.Min(r, Math.Min(g, b)); // minimum of r, g, b 
        double diff = cmax - cmin; // diff of cmax and cmin. 

        h = -1;
        s = -1;

        if (cmax == cmin) h = 0; // if cmax and cmax are equal then h = 0 
        // if cmax equal r then compute h 
        else if (cmax == r) h = (60 * ((g - b) / diff) + 360) % 360;
        // if cmax equal g then compute h 
        else if (cmax == g) h = (60 * ((b - r) / diff) + 120) % 360;
        // if cmax equal b then compute h 
        else if (cmax == b) h = (60 * ((r - g) / diff) + 240) % 360;
        // if cmax equal zero 
        if (cmax == 0) s = 0;
        else s = (diff / cmax) * 100;

        // compute v 
        v = cmax * 100;
    }

    public static void HsvToRgb(double h, double S, double V, out double r, out double g, out double b)
    {
        double H = h;
        while (H < 0) { H += 360; };
        while (H >= 360) { H -= 360; };

        double R, G, B;

        if (V <= 0) { R = G = B = 0; }
        else if (S <= 0) {
            R = G = B = V / 100.0;
        }
        else {
            S /= 100.0;
            V /= 100.0;

            double hf = H / 60.0;
            int i = (int)Math.Floor(hf);
            double f = hf - i;
            double pv = V * (1 - S);
            double qv = V * (1 - S * f);
            double tv = V * (1 - S * (1 - f));
            switch (i) {

                // Red is the dominant color

                case 0:
                    R = V;
                    G = tv;
                    B = pv;
                    break;

                // Green is the dominant color

                case 1:
                    R = qv;
                    G = V;
                    B = pv;
                    break;
                case 2:
                    R = pv;
                    G = V;
                    B = tv;
                    break;

                // Blue is the dominant color

                case 3:
                    R = pv;
                    G = qv;
                    B = V;
                    break;
                case 4:
                    R = tv;
                    G = pv;
                    B = V;
                    break;

                // Red is the dominant color

                case 5:
                    R = V;
                    G = pv;
                    B = qv;
                    break;

                // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                case 6:
                    R = V;
                    G = tv;
                    B = pv;
                    break;
                case -1:
                    R = V;
                    G = pv;
                    B = qv;
                    break;

                // The color is not defined, we should throw an error.

                default:
                    //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                    R = G = B = V; // Just pretend its black/white
                    break;
            }
        }
        r = R * 255.0;
        g = G * 255.0;
        b = B * 255.0;
    }

    /// <summary>
    /// Clamp a value to 0-255
    /// </summary>
    private static int Clamp(int i)
    {
        if (i < 0) return 0;
        if (i > 255) return 255;
        return i;
    }
}
