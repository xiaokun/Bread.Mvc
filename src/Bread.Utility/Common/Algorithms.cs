namespace Bread.Utility;

public static class Algorithms
{
    public static U? Map<T, U>(this T? x, Func<T, U?> fn)
        where T : class
        where U : class
    {
        if (x is null) return null;
        return fn(x);
    }


    /// <summary>
    /// 折半迭代，二分法迭代
    /// </summary>
    /// <param name="action">0：命中；1：在后面找； -1：在前面找</param>
    /// <returns></returns>
    public static int HalfFind(int count, Func<int, int> action)
    {
        int end = count;
        int begin = 0;
        int mid;
        int result = 0;

        while (begin <= end) {
            mid = (begin + end) / 2;
            if (mid < 0 || mid >= count) return -1;

            result = action(mid);
            if (result == 0)
                return mid;
            else if (result < 0) {
                end = mid - 1;
                continue;
            }
            else {
                begin = mid + 1;
                continue;
            }
        }

        return -1;
    }

    /// <summary>
    /// 判断 [x1, x2] 与 [y1, y2] 两个区间是否有重叠
    /// </summary>
    public static bool HasOverlap(double x1, double x2, double y1, double y2)
    {
        if(x2 < y1) return false;
        if(y2 < x1) return false;
        return true;
    }

    /// <summary>
    /// 移除两个list中的相同部分，保留不同的部分
    /// </summary>
    public static void AcceptDifference<T>(List<T> n1, List<T> n2) where T : IComparable<T>
    {
        if (n1.Count == 0) return;
        if (n2.Count == 0) return;

        for (int i = n1.Count - 1; i >= 0; i--) {
            var v1 = n1[i];
            for (int j = n2.Count - 1; j >= 0; j--) {
                var v2 = n2[j];
                if(v1.CompareTo(v2) == 0) {
                    n2.RemoveAt(j);
                    n1.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
