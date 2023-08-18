using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Bread.Utility.IO;

public class FileHelper
{
    /// <summary>
    /// 等待被其他线程占用的文件释放后删除
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="timeout">每次等待的时间</param>
    /// <param name="tryCount">尝试次数</param>
    /// <returns></returns>
    public static bool SafeDelete(string path, int timeout = 10, int tryCount = 100)
    {
        if (string.IsNullOrEmpty(path)) return true;
        if (!File.Exists(path)) return true;
        int count = 0;
        while (File.Exists(path)) {
            if (count >= tryCount) break;
            try {
                File.Delete(path);
                return true;
            }
            catch (Exception) {
                Thread.Sleep(timeout);
                count++;
                continue;
            }
        }
        return false;
    }

    /// <summary>
    /// 返回空间最大磁盘盘符 如C:\\
    /// </summary>
    /// <returns></returns>
    public static string GetLargestSpace()
    {
        DriveInfo[] allDrives;

        try {
            allDrives = System.IO.DriveInfo.GetDrives();
        }
        catch (Exception ex) {
            Log.Exception(ex);
            return "C:\\";
        }

        var maxDrive = allDrives[0];
        foreach (var driveInfo in allDrives) {
            try {
                if ((driveInfo.DriveType == System.IO.DriveType.Fixed) && (maxDrive.TotalFreeSpace < driveInfo.TotalFreeSpace))
                    maxDrive = driveInfo;
            }
            catch (Exception ex) {
                Log.Exception(ex);
                continue;
            }

        }
        return maxDrive.Name;
    }

    public static string? GetFileVersion(string path)
    {
        try {
            return FileVersionInfo.GetVersionInfo(path).FileVersion;
        }
        catch (Exception ex) {
            Log.Exception(ex);
            return null;
        }
    }

    public static string GetReadableSizeString(long bytes)
    {
        double kb = bytes * 1.0 / 1024;
        if (kb >= 1024 * 1024) return (kb / 1024 / 1024).ToString("f2") + " GB";
        if (kb >= 1024) return (kb / 1024).ToString("f2") + " MB";
        return kb.ToString("f2") + " KB";
    }

    /// <summary>
    /// 计算文件的md5值
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="offset">偏移量，复数表示使用反向偏移量；文件的读取始终是正向的</param>
    /// <param name="length">读取长度，单位：byte</param>
    /// <returns></returns>
    public static string CalcMd5(string filePath, long offset = 0, int length = 0)
    {
        FileInfo info = new FileInfo(filePath);
        if (info.Length == 0) throw new IOException("file is empty");

        long start = offset >= 0 ? offset : info.Length + offset;
        long end = length == 0 ? info.Length - 1 : start + length - 1;
        if (end >= info.Length) end = info.Length - 1;
        length = (int)(end - start + 1);

        using (FileStream file = File.OpenRead(filePath)) {
            var md5 = MD5.Create();
            var sb = new StringBuilder();
            byte[] retVal;

            if (start == 0 && length == info.Length) {
                retVal = md5.ComputeHash(file);
            }
            else {
                byte[] data = new byte[length];
                file.Position = start;
                file.Read(data, 0, length);
                retVal = md5.ComputeHash(data, 0, length);
            }
            for (int i = 0; i < retVal.Length; i++) {
                sb.Append(retVal[i].ToString("x2"));
            }
            file.Close();
            return sb.ToString();
        }
    }

    public static string CraeteUrlHash(string url, string? token = null)
    {
        if (string.IsNullOrWhiteSpace(token)) token = "bread.utility";
        var txt = url + token + url;
        var bytes = Encoding.UTF8.GetBytes(txt);
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(bytes);
        var sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++) {
            sb.Append(hash[i].ToString("x2"));
        }
        return sb.ToString();
    }

    /// <summary>
    /// 快速的计算一个视频文件的hash值
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string CalcFileHash(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) == true) return "0";

        if (filePath.StartsWith("http://") || filePath.StartsWith("https://")) {
            return CraeteUrlHash(filePath);
        }

        using (var fs = new FileStream(filePath, FileMode.Open)) {
            long length = fs.Length;
            long hash = 0;
            if (length > 1024) {
                {
                    fs.Position = length / 4;
                    int i0 = fs.ReadByte();
                    int i1 = fs.ReadByte();
                    int i2 = fs.ReadByte();
                    int i3 = fs.ReadByte();
                    hash += i0 * 256 * 256 * 256;
                    hash += i1 * 256 * 256;
                    hash += i2 * 256;
                    hash += i3;
                }

                {
                    fs.Position = length / 2;
                    int i0 = fs.ReadByte();
                    int i1 = fs.ReadByte();
                    int i2 = fs.ReadByte();
                    int i3 = fs.ReadByte();
                    hash += i0 * 256 * 256 * 256;
                    hash += i1 * 256 * 256;
                    hash += i2 * 256;
                    hash += i3;
                }
            }

            return length.ToString("D16") + hash.ToString("D16");
        }
    }

    /// <summary>
    /// 递归获取文件夹所有子文件。<br/>
    /// 调用者必须确保 folder 文件夹存在。
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="files"></param>
    public static void GetAllFiles(string folder, List<string> filelist)
    {
        var dirs = Directory.GetDirectories(folder);
        foreach (var dir in dirs) {
            GetAllFiles(dir, filelist);
        }

        var files = Directory.GetFiles(folder);
        foreach (var file in files) filelist.Add(file);
    }

    /// <summary>
    /// 获取所有最后底层的文件夹（没有子文件夹）。<br/>
    /// 调用者必须确保 folder 文件夹存在。
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="folders"></param>
    public static void GetBottomFolders(string folder, List<string> folders)
    {
        var subs = Directory.GetDirectories(folder);
        if (subs == null || subs.Length == 0) {
            folders.Add(folder);
            return;
        }

        foreach (var sub in subs) {
            GetBottomFolders(sub, folders);
        }
    }
}
