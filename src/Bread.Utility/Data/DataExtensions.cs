using Bread.Utility.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bread.Utility;

public static class DataExtensions
{
    
    public static bool IsEqual(this double v1, double v2, double eps = 0.0001)
    {
        if (Math.Abs(v1 - v2) > eps) return false;
        return true;
    }

    public static bool IsDiff(this double v1, double v2, double eps = 0.0001)
    {
        if (Math.Abs(v1 - v2) <= eps) return false;
        return true;
    }

    /// 生成16位GUID值
    /// </summary>
    public static string ToShort(this Guid id)
    {
        var bin = id.ToByteArray();
        long i = 1;
        foreach (var b in bin) {
            i *= ((int)b + 1);
        }
        return string.Format("{0:x}", i - DateTime.Now.Ticks);
    }

    public static long ToOffsetSeconds(this DateTime time)
    {
        var span = time.ToUniversalTime() - (new DateTime(1970, 1, 1));
        if (span.TotalSeconds < 0) return 0;
        return (long)span.TotalSeconds;
    }

    public static DateTime ToTime(this long seconds)
    {
        return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(seconds).ToLocalTime();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? ToProperty<T>(this string input)
    {
        try {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null) {
                // Cast ConvertFromString(string text) : object to (T)
                return (T?)converter.ConvertFromString(input);
            }
            return default(T);
        }
        catch (NotSupportedException ex) {
            Log.Exception(ex);
            return default(T);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToText<T>(this T obj)
    {
        if (obj == null) return string.Empty;

        if (obj is double d) {
            return $"{d:0.##}";
        }

        try {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null) {
                // Cast ConvertFromString(string text) : object to (T)
                return converter.ConvertToString(obj) ?? string.Empty;
            }
            else {
                return obj.ToString() ?? string.Empty;
            }
        }
        catch (NotSupportedException ex) {
            Log.Exception(ex);
            return obj.ToString() ?? string.Empty;
        }
    }

}

