namespace Bread.Utility.Net;

public class MimeHelper
{
    public static Dictionary<string, string> Types = new Dictionary<string, string>
        {
            {".txt", "text/plain"},
            {".pdf", "application/pdf"},
            {".doc", "application/vnd.ms-word"},
            {".docx", "application/vnd.ms-word"},
            {".xls", "application/vnd.ms-excel"},
            {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"},
            {".csv", "text/csv"},
            {".exe","application/octet-stream" },
            {".dll","application/octet-stream" },
            {".data","application/octet-stream" },
            {".config","text/plain" },
            {".xml","text/xml" },
            {".zip","application/x-compressed"}
        };


    public static string GetMimeTypeByPath(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentException();
        string ext = System.IO.Path.GetExtension(path);
        return GetMimeTypeByExtension(ext);
    }

    public static string GetExtensionByMimeType(string type)
    {
        if (string.IsNullOrEmpty(type)) throw new ArgumentException("type is null");
        type = type.ToLowerInvariant();
        foreach (var d in Types) {
            if (d.Value == type) {
                return d.Key;
            }
        }
        throw new KeyNotFoundException();
    }

    public static string GetMimeTypeByExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension)) throw new ArgumentException("文件后缀名缺失");
        extension = extension.ToLowerInvariant();
        foreach (var d in Types) {
            if (d.Key == extension) {
                return d.Value;
            }
        }
        throw new NotImplementedException();
    }
}
