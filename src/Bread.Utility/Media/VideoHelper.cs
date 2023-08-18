using System.Drawing;
using Bread.Utility.IO;

namespace Bread.Utility;

public class VideoHelper
{
    public static void ParseFrameRate(double frameRate, out int FPS, out bool isNtsc)
    {
        double dot = frameRate - (int)frameRate;
        if (dot < 0.001) {
            FPS = (int)frameRate;
            isNtsc = false;
            return;
        }
        isNtsc = true;
        FPS = (int)(frameRate + 0.5);
    }


    public static void ParseFrameRate(double frameRate, out int num, out int den)
    {
        num = 0;
        den = 0;

        if (Math.Abs(frameRate - (int)frameRate) < 0.001) {
            num = (int)frameRate;
            den = 1;
            return;
        }

        if (Math.Abs(frameRate - 23.976) < 0.01) {
            //24000/1001
            num = 24000;
            den = 1001;
            return;
        }

        if (Math.Abs(frameRate - 29.97) < 0.01) {
            //30000/1001
            num = 30000;
            den = 1001;
            return;
        }

        if (Math.Abs(frameRate - 29.97) < 0.01) {
            //60000/1001
            num = 60000;
            den = 1001;
            return;
        }

        throw new NotSupportedException($"FrameRate:{frameRate} not supported");
    }


    private static int MulDiv(int number, int numerator, int denominator)
    {
        return (int)(((long)number * numerator) / denominator);
    }


    public Rectangle LetterBoxRect(Size videoSource, Rectangle wndRect)
    {
        // figure out src/dest scale ratios
        int iSrcWidth = videoSource.Width;
        int iSrcHeight = videoSource.Height;

        int iDstWidth = wndRect.Width;
        int iDstHeight = wndRect.Height;

        int iDstLBWidth;
        int iDstLBHeight;

        if (MulDiv(iSrcWidth, iDstHeight, iSrcHeight) <= iDstWidth) {
            // Column letter boxing ("pillar box")
            iDstLBWidth = MulDiv(iDstHeight, iSrcWidth, iSrcHeight);
            iDstLBHeight = iDstHeight;
        }
        else {
            // Row letter boxing.
            iDstLBWidth = iDstWidth;
            iDstLBHeight = MulDiv(iDstWidth, iSrcHeight, iSrcWidth);
        }

        // Create a centered rectangle within the current destination rect
        int left = wndRect.Left + ((iDstWidth - iDstLBWidth) / 2);
        int top = wndRect.Top + ((iDstHeight - iDstLBHeight) / 2);

        return new Rectangle(left, top, left + iDstLBWidth, top + iDstLBHeight);
    }


    /// <summary>
    /// 获取所见即所得视频播放窗口的最佳显示位置
    /// </summary>
    /// <param name="destRect">目标区域</param>
    /// <param name="inputSize">输入尺寸，视频的未缩放前的实际尺寸</param>
    /// <param name="minMargin">最小边距</param>
    /// <returns></returns>
    public static RectangleF GetVideoRect(RectangleF destRect, SizeF inputSize, int minMargin = 0)
    {
        RectangleF rect = RectangleF.Empty;
        if (destRect.Width == 0 || destRect.Height == 0 || inputSize.Width == 0 || inputSize.Height == 0) {
            return rect;
        }

        double destRatio = destRect.Width / (double)destRect.Height; //宽高比
        double inputRatio = inputSize.Width / (double)inputSize.Height;
        if (destRatio > inputRatio) {
            rect.Height = destRect.Height - minMargin * 2;
            rect.Y = destRect.Y + minMargin;
            rect.Width = (int)(rect.Height * inputRatio);
            rect.X = destRect.X + (int)((destRect.Width - rect.Width) / 2.0);
        }
        else {
            rect.Width = destRect.Width - minMargin * 2;
            rect.X = destRect.X + minMargin;
            rect.Height = (int)(rect.Width / inputRatio);
            rect.Y = destRect.Y + (int)((destRect.Height - rect.Height) / 2.0);
        }
        return rect;
    }

    /// <summary>
    /// 获取采集时间 时:分:秒.帧数
    /// </summary>
    /// <returns>时:分:秒.帧数</returns>
    public static string GetVideoTimeInFrame(long hnsTime, float frameRate = 25)
    {
        long ONE_SECOND = 10000000;
        if (hnsTime <= 0) {
            return "00:00:00.00";
        }

        try {
            return (string.Format("{0:00}:{1:00}:{2:00}.{3:00}", hnsTime / ONE_SECOND / 3600, hnsTime / ONE_SECOND % 3600 / 60,
                hnsTime / ONE_SECOND % 60, (hnsTime % ONE_SECOND / 10000000.0) * frameRate));
        }
        catch {
            return "00:00:00.00";
        }
    }

    /// <summary>
    /// 获取采集时间 时:分:秒:毫秒
    /// </summary>
    /// <returns>时:分:秒:毫秒</returns>
    public static string GetVideoTimeInMillisecond(long hnsTime)
    {
        long ONE_SECOND = 10000000;
        if (hnsTime <= 0) {
            return "00:00:00:00";
        }

        try {
            return (string.Format("{0:00}:{1:00}:{2:00}:{3:00}", hnsTime / ONE_SECOND / 3600, hnsTime / ONE_SECOND % 3600 / 60,
                hnsTime / ONE_SECOND % 60, (hnsTime % ONE_SECOND / 10000000.0) * 1000));
        }
        catch {
            return "00:00:00:00";
        }
    }

    /// <summary>
    /// 获取采集时间 时:分:秒
    /// </summary>
    /// <param name="hnsTime"></param>
    /// <returns>时:分:秒</returns>
    public static string GetVideoTimeInSecondByHnsTime(long hnsTime)
    {
        long ONE_SECOND = 10000000;
        if (hnsTime <= 0) return "00:00:00";
        try {
            return (string.Format("{0:00}:{1:00}:{2:00}", hnsTime / ONE_SECOND / 3600,
                hnsTime / ONE_SECOND % 3600 / 60, hnsTime / ONE_SECOND % 60));
        }
        catch {
            return "00:00:00";
        }
    }

    /// <summary>
    /// 获取采集时间 时:分:秒
    /// </summary>
    /// <param name="hnsTime"></param>
    /// <param name="frameRate"></param>
    /// <returns>时:分:秒</returns>
    public static string GetVideoTimeInSecondByMillisecond(long milliseconds)
    {
        if (milliseconds <= 0) return "00:00:00";
        long seconds = milliseconds / 1000;
        return (string.Format("{0:00}:{1:00}:{2:00}", seconds / 3600, seconds % 3600 / 60, seconds % 60));
    }

    public static double GetMilliseconds(long hnsTime)
    {
        return hnsTime / 10000.0;
    }

    /// <summary>
    /// 获取采集时间 时:分:秒
    /// </summary>
    /// <param name="hnsTime"></param>
    /// <param name="frameRate"></param>
    /// <returns>时:分:秒</returns>
    public static string GetVideoTimeBySecond(double seconds, bool align = false)
    {
        if (seconds <= 0) return align ? "00:00:00" : "00:00";
        int s = (int)seconds;

        if (align) {
            return (string.Format("{0:00}:{1:00}:{2:00}", s / 3600, s % 3600 / 60, s % 60));
        }

        if(seconds < 3600) {
            return (string.Format("{0:00}:{1:00}", s / 60, s % 60));
        }
        else {
            return (string.Format("{0:00}:{1:00}:{2:00}", s / 3600, s % 3600 / 60, s % 60));
        }
    }


    /// <summary>
    /// duration in milliseconds
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static long GetDuration(string path)
    {
        try {
            if (!System.IO.File.Exists(path)) return 0;
            using (System.Diagnostics.Process pro = new System.Diagnostics.Process()) {
                pro.StartInfo.CreateNoWindow = true;
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.ErrorDialog = false;
                pro.StartInfo.RedirectStandardError = true;

                pro.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "ffmpeg.exe";
                pro.StartInfo.Arguments = " -i " + "\"" + path + "\"";

                pro.Start();
                System.IO.StreamReader errorreader = pro.StandardError;
                pro.WaitForExit(1000);

                string result = errorreader.ReadToEnd();
                if (!string.IsNullOrEmpty(result)) {
                    string duration = result.Substring(result.IndexOf("Duration: ") + ("Duration: ").Length, ("00:00:00").Length);
                    DateTime dt = DateTime.Now;
                    bool success = DateTime.TryParse(duration, out dt);
                    if (!success) return 0;

                    long time = (dt.Hour * 60 * 60 + dt.Minute * 60 + dt.Second) * 1000;
                    return time;
                }
                return 0;
            }
        }
        catch (Exception ex) {
            Log.Exception(ex);
            return 0;
        }
    }
}
