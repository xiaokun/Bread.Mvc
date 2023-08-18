using System.Drawing;

namespace Bread.Mvc;

public static class DrawingConfigExtensions
{

    //{Width=0, Height=0}
    //{Width=100, Height=50}
    public static void Load(this Config config, string group, string key, Action<Size> action)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        var strValue = string.IsNullOrEmpty(group) ? config[key] : config[group, key];
        if (string.IsNullOrWhiteSpace(strValue)) return;

        Size size = new Size(0, 0);
        if (string.IsNullOrEmpty(strValue)) return;
        if (strValue.Length < 18) goto error;

        int start = strValue.IndexOf('{');
        int end = strValue.IndexOf('}');
        if (start == -1 || end == -1) goto error;

        int length = end - start - 1;
        if (length < 16) goto error;
        string sub = strValue.Substring(start + 1, length);
        if (string.IsNullOrEmpty(sub)) goto error;

        string[] values = sub.Split(',');
        if (values == null || values.Length != 2) goto error;

        int value = 0;
        if (TryParseEquation(values[0].Trim(), "Width", out value)) size.Width = value;
        else goto error;
        if (TryParseEquation(values[1].Trim(), "Height", out value)) size.Height = value;
        else goto error;

        action(size);
        return;

error:
        Log.Error($"未能正确解析ini文件中的 Size 类型参数: {strValue}");
    }

    //{X=20, Y=20, Width=100, Height=50}
    public static void Load(this Config config, string group, string key, Action<Rectangle> action)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        var strValue = string.IsNullOrEmpty(group) ? config[key] : config[group, key];
        if (string.IsNullOrWhiteSpace(strValue)) return;

        Rectangle rect = new Rectangle(0, 0, 0, 0);
        if (string.IsNullOrEmpty(strValue)) return;
        if (strValue.Length < 9) goto error;

        int start = strValue.IndexOf('{');
        int end = strValue.IndexOf('}');
        if (start == -1 || end == -1) goto error;

        int length = end - start - 1;
        if (length < 7) goto error;
        string sub = strValue.Substring(start + 1, length);
        if (string.IsNullOrEmpty(sub)) goto error;

        string[] values = sub.Split(',');
        if (values == null || values.Length != 4) goto error;

        int value = 0;
        if (TryParseEquation(values[0].Trim(), "X", out value)) rect.X = value;
        else goto error;
        if (TryParseEquation(values[1].Trim(), "Y", out value)) rect.Y = value;
        else goto error;
        if (TryParseEquation(values[2].Trim(), "Width", out value)) rect.Width = value;
        else goto error;
        if (TryParseEquation(values[3].Trim(), "Height", out value)) rect.Height = value;
        else goto error;

        action(rect);
        return;

error:
        Log.Error($"未能正确解析ini文件中的 Rectangle 类型参数: {strValue}");
    }

    //{X=20.0, Y=20.0, Width=100.83, Height=50.893}
    public static void Load(this Config config, string group, string key, Action<RectangleF> action)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        var strValue = string.IsNullOrEmpty(group) ? config[key] : config[group, key];
        if (string.IsNullOrWhiteSpace(strValue)) return;

        RectangleF rect = new RectangleF(0, 0, 0, 0);
        if (string.IsNullOrEmpty(strValue)) return;
        if (strValue.Length < 9) goto error;

        int start = strValue.IndexOf('{');
        int end = strValue.IndexOf('}');
        if (start == -1 || end == -1) goto error;

        int length = end - start - 1;
        if (length < 7) goto error;
        string sub = strValue.Substring(start + 1, length);
        if (string.IsNullOrEmpty(sub)) goto error;

        string[] values = sub.Split(',');
        if (values == null || values.Length != 4) goto error;

        float value = 0;
        if (TryParseEquation(values[0].Trim(), "X", out value)) rect.X = value;
        else goto error;
        if (TryParseEquation(values[1].Trim(), "Y", out value)) rect.Y = value;
        else goto error;
        if (TryParseEquation(values[2].Trim(), "Width", out value)) rect.Width = value;
        else goto error;
        if (TryParseEquation(values[3].Trim(), "Height", out value)) rect.Height = value;
        else goto error;

        action(rect);
        return;

error:
        Log.Error($"未能正确解析ini文件中的 RectangleF 类型参数: {strValue}");
    }

    /// <summary>
    /// parser (x1,y1)|(x2,y2)|(x3,y3)|(x4,y4) into point[4]
    /// </summary>
    /// <param name="strValue"></param>
    /// <param name="action"></param>
    public static void Load(this Config config, string group, string key, Action<PointF[]> action)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        var strValue = string.IsNullOrEmpty(group) ? config[key] : config[group, key];
        if (string.IsNullOrWhiteSpace(strValue)) return;

        if (strValue.Length < 23) goto error;

        string[] values = strValue.Split('|');
        if (values == null || values.Length != 4) goto error;

        PointF[] points = new PointF[4];

        try {
            for (int i = 0; i < 4; i++) {
                var str = values[i].TrimStart('(').TrimEnd(')').Split(',');
                if (str.Length != 2) goto error;
                if (float.TryParse(str[0], out float x)) {
                    if (float.TryParse(str[1], out float y)) {
                        points[i] = new PointF(x, y);
                        continue;
                    }
                }
                goto error;
            }
        }
        catch (Exception ex) {
            Log.Exception(ex);
            goto error;
        }

        action(points);
        return;

error:
        throw new InvalidProgramException($"未能正确解析ini文件中的 Rectangle 类型参数: {strValue}");
    }


    // try parse "X= 3" out to int  3
    // try parse "Width = 5" out to int  5
    private static bool TryParseEquation(string input, string type, out int result)
    {
        result = 0;
        int index = input.IndexOf('=');
        if (index == -1 || index >= input.Length - 1) return false;
        string first = input.Substring(0, index).Trim();
        if (first == null || first.Length <= 0) return false;
        if (first.ToLower() != type.Trim().ToLower()) return false;
        string second = input.Substring(index + 1).Trim();
        if (int.TryParse(second, out result)) return true;
        return false;
    }

    // try parse "X= 3" out to int  3
    // try parse "Width = 5" out to int  5
    private static bool TryParseEquation(string input, string type, out float result)
    {
        result = 0;
        int index = input.IndexOf('=');
        if (index == -1 || index >= input.Length - 1) return false;
        string first = input.Substring(0, index).Trim();
        if (first == null || first.Length <= 0) return false;
        if (first.ToLower() != type.Trim().ToLower()) return false;
        string second = input.Substring(index + 1).Trim();
        if (float.TryParse(second, out result)) return true;
        return false;
    }

}
