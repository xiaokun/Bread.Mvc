namespace Bread.Utility;

public static class PowerPointFileFormat
{
    private static List<string> _exts = new List<string>() { ".ppt", ".pptx", ".pptm", ".pps", ".ppsx", ".ppsm", ".pot", ".potm", ".potx", ".dps", ".dpt" };

    public static bool ContainExtention(string ext)
    {
        foreach (var e in _exts) {
            if (e == ext) return true;
        }
        return false;
    }

    public static string GetFileFilter()
    {
        return "ppt文件|*.ppt;*.pptx;*.pptm;*.pps;*.ppsm;*.pot;*.potm;*.potx;*.dps;*.dpt|所有文件|*.*";
    }
}

public static class WordFileFormat
{
    private static List<string> _exts = new List<string>() { ".doc", ".docx", ".wps", ".wpt", ".dot", ".dotx", ".docm", ".dotm" };

    public static bool ContainExtention(string ext)
    {
        foreach (var e in _exts) {
            if (e == ext) return true;
        }
        return false;
    }

    public static string GetFileFilter()
    {
        throw new NotImplementedException();
    }
}

public static class ExcelFileFormat
{
    private static List<string> _exts = new List<string>() { ".xls", ".xlsx", ".et", ".ett", ".xlt", ".xlsm", ".xltm", ".xltx", ".xla", ".xlam" };

    public static bool ContainExtention(string ext)
    {
        foreach (var e in _exts) {
            if (e == ext) return true;
        }
        return false;
    }

    public static string GetFileFilter()
    {
        throw new NotImplementedException();
    }
}

public static class TextFileFormat
{
    private static List<string> _exts = new List<string>() { ".txt", ".text", ".md" };

    public static bool ContainExtention(string ext)
    {
        foreach (var e in _exts) {
            if (e == ext) return true;
        }
        return false;
    }

    public static string GetFileFilter()
    {
        throw new NotImplementedException();
    }
}

public enum ImageFileFormat : int
{
    None = 0,
    JPG,
    PNG,
    BMP,
    GIF,
    TIFF,
    SVG,
    TGA,
    WEBP
}

public static class ImageFileFormats
{
    public static List<string> GetExtensions()
    {
        var list = new List<string>();
        list.Add(".jpg");
        list.Add(".jpeg");
        list.Add(".png");
        list.Add(".jpeg");
        list.Add(".bmp");
        list.Add(".gif");
        list.Add(".tiff");
        list.Add(".tif");
        list.Add(".svg");
        list.Add(".tga");
        list.Add(".webp");
        return list;
    }

    public static string GetFilter()
    {
        return "图片文件|*.jpg;*.jpeg;*.png;*.jpeg;*.bmp;*.gif;*.tiff;*.tif;*.svg;*.tga;*.webp|所有文件|*.*";
    }

    public static string GetExtension(this ImageFileFormat format)
    {
        if (format == ImageFileFormat.None) return "";
        return $".{format.ToString().ToLower()}";
    }

    public static bool IsImageFile(this string ext)
    {
        var format = GetImageFileFormat(ext);
        if (format == ImageFileFormat.None) return false;
        return true;
    }

    public static ImageFileFormat GetImageFileFormat(this string ext)
    {
        if (string.IsNullOrEmpty(ext)) return ImageFileFormat.None;
        ext = ext.ToLower();
        switch (ext) {
            case ".png":
                return ImageFileFormat.PNG;
            case ".jpg":
            case ".jpeg":
                return ImageFileFormat.JPG;
            case ".bmp":
                return ImageFileFormat.BMP;
            case ".gif":
                return ImageFileFormat.GIF;
            case ".tiff":
            case ".tif":
                return ImageFileFormat.TIFF;
            case ".svg":
                return ImageFileFormat.SVG;
            case ".tga":
                return ImageFileFormat.TGA;
            case ".webp":
                return ImageFileFormat.WEBP;
        }
        return ImageFileFormat.None;
    }
}


public enum VideoFileFormat : int
{
    None = 0,
    MP4,
    MOV,
    MKV,
    AVI,
    WMV
}

public static class VideoFileFormats
{
    public static string GetFilter()
    {
        return "视频文件|*.mp4;*.mov;*.mkv;*.avi;*.wmv|所有文件|*.*";
    }

    public static string GetExtension(this VideoFileFormat format)
    {
        if (format == VideoFileFormat.None) return "";
        return $".{format.ToString().ToLower()}";
    }

    public static bool IsVideoFile(this string path)
    {
        var ext = Path.GetExtension(path);
        if (string.IsNullOrWhiteSpace(ext)) return false;

        var format = GetVideoFileFormat(ext);
        if (format == VideoFileFormat.None) return false;
        return true;
    }

    public static VideoFileFormat GetVideoFileFormat(this string ext)
    {
        if (string.IsNullOrEmpty(ext)) return VideoFileFormat.None;
        ext = ext.ToLower();
        switch (ext) {
            case ".mp4":
                return VideoFileFormat.MP4;
            case ".mkv":
                return VideoFileFormat.MKV;
            case ".mov":
                return VideoFileFormat.MOV;
            case ".avi":
                return VideoFileFormat.AVI;
            case ".wmv":
                return VideoFileFormat.WMV;
        }
        return VideoFileFormat.None;
    }
}


public enum AudioFileFormat : int
{
    None = 0,
    MP3,
    WAV,
    AAC,
    APE,
    FLAC,
    WMA
}

public static class AudioFileFormats
{
    public static string GetFilter()
    {
        return "音频文件|*.mp3;*.wav;*.aac;*.ape;*.flac;*.wma|所有文件|*.*";
    }

    public static string GetExtension(this AudioFileFormat format)
    {
        if (format == AudioFileFormat.None) return "";
        return $".{format.ToString().ToLower()}";
    }

    public static bool IsAudioFile(this string path)
    {
        var ext = Path.GetExtension(path);
        if (string.IsNullOrWhiteSpace(ext)) return false;

        var format = GetAudioFileFormat(ext);
        if (format == AudioFileFormat.None) return false;
        return true;
    }

    public static AudioFileFormat GetAudioFileFormat(string ext)
    {
        if (string.IsNullOrEmpty(ext)) return AudioFileFormat.None;
        ext = ext.ToLower();
        switch (ext) {
            case ".mp3":
                return AudioFileFormat.MP3;
            case ".wav":
                return AudioFileFormat.WAV;
            case ".aac":
                return AudioFileFormat.AAC;
            case ".ape":
                return AudioFileFormat.APE;
            case ".flac":
                return AudioFileFormat.FLAC;
            case ".wma":
                return AudioFileFormat.WMA;
        }
        return AudioFileFormat.None;
    }
}

public static class MediaFileFormats
{
    public static string GetFilter()
    {
        return "视频文件|*.mp4;*.mov;*.mkv;*.avi;*.wmv|音频文件|*.mp3;*.wav;*.aac;*.ape;*.flac;*.wma|所有文件|*.*";
    }

    public static bool IsMediaFile(this string path)
    {
        if (path.IsAudioFile()) return true;
        if (path.IsVideoFile()) return true;
        return false;
    }

}
