using System.Drawing;
using Bread.Utility;
using Color = System.Windows.Media.Color;

namespace Bread.Mvc.WPF;

public static class ConfigExtensions
{
    // try parse "X= 3" out to int  3
    // try parse "Width = 5" out to int  5
    private static bool TryParseEquation(string input, string type, out int result)
    {
        result = 0;
        int index = input.IndexOf('=');
        if (index == -1 || index >= input.Length - 1) return false;
        string first = input[..index].Trim();
        if (first == null || first.Length <= 0) return false;
        if (first.ToLower() != type.Trim().ToLower()) return false;
        string second = input.Substring(index + 1).Trim();
        if (int.TryParse(second, out result)) return true;
        return false;
    }

    //private Color StringToColor(string colorStr)
    //{
    //    Byte[] argb = new Byte[4];
    //    for (int i = 0; i < 4; i++) {
    //        char[] charArray = colorStr.Substring(i * 2 + 1, 2).ToCharArray();
    //        //string str = "11";
    //        Byte b1 = ToByte(charArray[0]);
    //        Byte b2 = ToByte(charArray[1]);
    //        argb[i] = (Byte)(b2 | (b1 << 4));
    //    }
    //    return Color.FromArgb(argb[0], argb[1], argb[2], argb[3]);//#FFFFFFFF
    //}

    //private static byte ToByte(char c)
    //{
    //    byte b = (byte)"0123456789ABCDEF".IndexOf(c);
    //    return b;
    //}

    public static void Load(this Config config, string group, string key, Action<Color> action)
    {
        var strValue = config[group, key];
        if (string.IsNullOrWhiteSpace(strValue)) return;
        if (strValue.Length != 9 && strValue.Length != 7) goto error;

        if (strValue.StartsWith("#") == false) goto error;
        strValue = strValue.Replace("#", string.Empty);
        int v = int.Parse(strValue, System.Globalization.NumberStyles.HexNumber);
        var color = v.ToColor();
        action(color);
        return;

error:
        Log.Error($"未能正确解析ini文件中的 Color 类型参数: {strValue}");
    }

    //{Width=0, Height=0}
    //{Width=100, Height=50}
    public static void Load(this Config config, string group, string key, Action<Size> action)
    {
        var strValue = config[group, key];
        if (string.IsNullOrWhiteSpace(strValue)) return;
        if (strValue.Length < 18) goto error;

        int start = strValue.IndexOf('{');
        int end = strValue.IndexOf('}');
        if (start == -1 || end == -1) goto error;

        int length = end - start - 1;
        if (length < 16) goto error;
        string sub = strValue.Substring(start + 1, length);
        if (string.IsNullOrWhiteSpace(sub)) goto error;

        string[] values = sub.Split(',');
        if (values == null || values.Length != 2) goto error;

        Size size = new Size(0, 0);
        if (TryParseEquation(values[0].Trim(), "Width", out int width)) size.Width = width;
        else goto error;
        if (TryParseEquation(values[1].Trim(), "Height", out int height)) size.Height = height;
        else goto error;

        action(size);
        return;

error:
        Log.Error($"未能正确解析ini文件中的 Size 类型参数: {strValue}");
    }

    //{X=20, Y=20, Width=100, Height=50}
    public static void Load(this Config config, string group, string key, Action<Rectangle> action)
    {

        var strValue = config[group, key];
        if (string.IsNullOrWhiteSpace(strValue)) return;
        if (strValue.Length < 9) goto error;

        int start = strValue.IndexOf('{');
        int end = strValue.IndexOf('}');
        if (start == -1 || end == -1) goto error;

        int length = end - start - 1;
        if (length < 7) goto error;
        string sub = strValue.Substring(start + 1, length);
        if (string.IsNullOrWhiteSpace(sub)) goto error;

        string[] values = sub.Split(',');
        if (values == null || values.Length != 4) goto error;

        Rectangle rect = new Rectangle(0, 0, 0, 0);
        if (TryParseEquation(values[0].Trim(), "X", out int x)) rect.X = x;
        else goto error;
        if (TryParseEquation(values[1].Trim(), "Y", out int y)) rect.Y = y;
        else goto error;
        if (TryParseEquation(values[2].Trim(), "Width", out int width)) rect.Width = width;
        else goto error;
        if (TryParseEquation(values[3].Trim(), "Height", out int height)) rect.Height = height;
        else goto error;

        action(rect);
        return;

error:
        Log.Error($"未能正确解析ini文件中的 Rectangle 类型参数: {strValue}");
    }
}
