namespace Bread.Utility;

public unsafe static class Similarity
{
    public static double SimilarityTo(this string src, string dst)
    {
        double factor = 2.0;
        double srcFactor = 1.0;
        double dstFactor = 1.0;
        int intersect = src.Intersect(dst).Count();
        int srcDiff = src.Length - src.Where((char o) => dst.Contains(o)).Count();
        int dstDiff = dst.Length - dst.Where((char o) => src.Contains(o)).Count();
        return factor * (double)intersect / (factor * (double)intersect + srcFactor * (double)dstDiff + dstFactor * (double)srcDiff);
    }

    public unsafe static double SimilarityTo(this IntPtr img1, IntPtr img2, int width, int height)
    {
        int bytesPerPixel = 3;
        int step = 255;
        byte* pSrc = (byte*)img1.ToPointer();
        byte* pDst = (byte*)img2.ToPointer();

        double wfactor = 0.01;
        double hfactor = 0.03;
        double wvalue = wfactor * (double)step * (wfactor * (double)step);
        double hvalue = hfactor * (double)step * (hfactor * (double)step);

        double hvalueHalf = hvalue / 2.0;
        double srcTotal = 0.0;
        double dstTotal = 0.0;
        for (int h = 0; h < height; h++) {
            for (int w = 0; w < width; w++) {
                for (int k = 0; k < bytesPerPixel; k++) {
                    srcTotal += (double)(int)(*(pSrc++));
                    dstTotal += (double)(int)(*(pDst++));
                }
            }
        }
        double srcValue = srcTotal / (double)(width * height * bytesPerPixel);
        double dstValue = dstTotal / (double)(width * height * bytesPerPixel);

        pSrc = (byte*)img1.ToPointer();
        pDst = (byte*)img2.ToPointer();
        double wAvg = 0.0;
        double hAvg = 0.0;
        double whAvg = 0.0;
        for (int h = 0; h < height; h++) {
            for (int w = 0; w < width; w++) {
                for (int n = 0; n < bytesPerPixel; n++) {
                    double x = (double)(int)(*(pSrc++)) - srcValue;
                    double y = (double)(int)(*(pDst++)) - dstValue;
                    wAvg += x * x;
                    hAvg += y * y;
                    whAvg += x * y;
                }
            }
        }
        double wNormal = Math.Sqrt(wAvg / (double)(width * height * bytesPerPixel - 1));
        double hNormal = Math.Sqrt(hAvg / (double)(width * height * bytesPerPixel - 1));
        double whNormal = whAvg / (double)(width * height * bytesPerPixel - 1);

        double v1 = (2.0 * srcValue * dstValue + wvalue) / (srcValue * srcValue + dstValue * dstValue + wvalue);
        double v2 = (2.0 * wNormal * hNormal + hvalue) / (wNormal * wNormal + hNormal * hNormal + hvalue);
        double v3 = (whNormal + hvalueHalf) / (wNormal * hNormal + hvalueHalf);
        double result = v1 * v2 * v3;
        return result;
    }
}
