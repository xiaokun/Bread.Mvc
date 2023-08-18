using System.ComponentModel;
using System.Text;
using System.Web;

namespace Bread.Utility;

public static class StringHelper
{
    public static T? Value<T>(this string input) where T : IParsable<T>
    {
        if (T.TryParse(input, null, out T? value)) {
            return value;
        }
        return default(T);
    }

    public static object? Value(this string input, Type type)
    {
        try {
            return TypeDescriptor.GetConverter(type).ConvertFromString(input);
        }
        catch {
            return null;
        }
    }

    /// <summary>
    /// 将路径进行URLEncode
    /// 此方法仅用于输出FCPXML
    /// </summary>
    /// <param name="path"></param>
    public static string EncodeFilePath(this string path)
    {
        string url = HttpUtility.UrlEncode(path);
        url = url.Replace("%5c", "/"); //替换回“/”
        return url.Replace("+", "%20");//过滤掉“+”
    }

    public static string Wrap(this string input, int maxLength, string demiter = "...")
    {
        if (maxLength <= demiter.Length + 1) return demiter;

        if (input.Length < maxLength) {
            return input;
        }

        int trimLength = (maxLength - demiter.Length) / 2;
        return input.Substring(0, trimLength) + demiter + input.Substring(input.Length - trimLength);
    }

    public static bool IsNullOrWhiteSpace(this string? str)
    {
        if (str == null) return true;
        if (str.Length == 0) return true;
        foreach (var ch in str) {
            if (ch != ' ') {
                return false;
            }
        }
        return true;
    }

    public static bool IsNullOrEmpty(this string? str)
    {
        if (str == null) return true;
        if (str!.Length == 0) return true;
        return false;
    }

    public static bool IsNotNullOrWhiteSpace(this string? str)
    {
        if (str == null) return true;
        if (str.Length == 0) return true;
        foreach (var ch in str) {
            if (ch != ' ') {
                return false;
            }
        }
        return true;
    }

    public static bool IsNotNullOrEmpty(this string? str)
    {
        if (str == null) return false;
        if (str.Length == 0) return false;
        return true;
    }

    public static string? IfNullOrEmpty(this string? str, string? value)
    {
        if (str is null) return value;
        if (str.Length == 0) return value;
        return str;
    }

    public static string? IfNullOrWhiteSpace(this string? str, string? value)
    {
        if (str is null) return value;
        if (str.Length == 0) return value;
        foreach (var ch in str) {
            if (ch != ' ') {
                return str;
            }
        }
        return value;
    }

    public static string Concat(this string separator, List<string>? list)
    {
        if (list is null) return string.Empty;

        var builder = new StringBuilder();
        for (var i = 0; i < list.Count; i++) {
            var str = list[i];
            if (string.IsNullOrWhiteSpace(str)) continue;
            builder.Append(str);
            if (i == list.Count - 1) break;
            builder.Append(separator);
        }
        var result = builder.ToString();
        if (result.EndsWith(separator)) {
            return result.Substring(0, result.Length - separator.Length);
        }
        return result;
    }

    public static string Concat(string separator, params string[]? strs)
    {
        if (strs is null) return string.Empty;

        var builder = new StringBuilder();
        for (var i = 0; i < strs.Length; i++) {
            var str = strs[i];
            if (string.IsNullOrWhiteSpace(str)) continue;
            builder.Append(str);
            if (i == strs.Length - 1) break;
            builder.Append(separator);
        }
        var result = builder.ToString();
        if (result.EndsWith(separator)) {
            return result.Substring(0, result.Length - separator.Length);
        }
        return result;
    }

    /// <summary> 
    /// 检测含有中文字符串的实际长度 
    /// </summary> 
    /// <param name="str">字符串</param> 
    public static int GetChineseLength(string str)
    {
        System.Text.ASCIIEncoding n = new System.Text.ASCIIEncoding();
        byte[] b = n.GetBytes(str);
        int l = 0; // l 为字符串之实际长度 
        for (int i = 0; i <= b.Length - 1; i++) {
            if (b[i] == 63) //判断是否为汉字或全脚符号 
            {
                l++;
            }
            l++;
        }
        return l;

    }
}
