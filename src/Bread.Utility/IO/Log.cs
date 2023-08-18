using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Bread.Utility.IO;

public enum LogFilter : int
{
    None,
    Info = 1,
    Warn = 2,
    Error = 4,
    Debug = 8,
    Production = 7,
    All = 15
}

public static class Log
{
    /// <summary>
    ///	是否允许输出
    /// </summary>
    public static bool IsEnable { get; set; } = true;

    /// <summary>
    /// 输出每一条日志到文件
    /// </summary>
    public static bool IsAutoFlush { get; set; } = false;

    /// <summary>
    /// 追加模式下，日志文件的最大体积
    /// </summary>
    public static double MaxSize { get; set; } = 100; // mb

    /// <summary>
    /// 用于过滤日志，可以使用位运算进行组合
    /// </summary>
    public static LogFilter Filter { get; set; } = LogFilter.All;

    static TextWriter? _loger = null;
    static volatile bool _isOpened = false;
    static string? _logPath;

    /// <summary>
    /// 打开日志
    /// </summary>
    /// <param name="path">日志文件名称</param>
    /// <param name="expire">日志文件目录下最多保存天数。0表示不删除多余日志</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Open(string path, int expire = 0)
    {
        if (_isOpened && path == _logPath) return;
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path is null or emtpy");
        if (_loger != null) Close();

        try {
            var folder = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(folder) == false &&
                Directory.Exists(folder) == false) {
                Directory.CreateDirectory(folder);
            }

            if (File.Exists(path)) File.Delete(path);
            _loger = TextWriter.Synchronized(File.CreateText(path));
            Info("logger open success");
            _isOpened = true;
            _logPath = path;

            if (expire > 0) {
                DeleteExpiredLogs(folder!, expire);
            }
        }
        catch (Exception ex) {
            System.Console.WriteLine(ex.Message);
            if (_loger == null) return;
            Close();
        }
    }

    /// <summary>
    /// 关闭日志文件
    /// </summary>
    public static void Close()
    {
        if (_loger == null) return;
        if (_isOpened == false) return;
        _isOpened = false;

        try {
            Info("logger close success");
            _loger.Flush();
            _loger.Close();
            _loger.Dispose();
            _loger = null;
        }
        catch { }
    }

    //删除超过7天的日志
    private static void DeleteExpiredLogs(string dir, int days)
    {
        try {
            var filepaths = Directory.GetFiles(dir);
            for (int i = filepaths.Length - 1; i >= 0; i--) {
                var file = new FileInfo(filepaths[i]);
                if (file.Exists && (DateTime.Now - file.CreationTime).TotalDays > days) {
                    try { file.Delete(); }
                    catch (Exception ex) {
                        Log.Error($"[APP]\t清理过期日志时异常: {file.Name}");
                        Log.Exception(ex);
                        continue;
                    }
                }
            }
        }
        catch (Exception ex) {
            Log.Error("[APP]\t清理过期日志时异常");
            Log.Exception(ex);
        }
    }

    public static void Flush()
    {
        if (_loger != null) _loger.FlushAsync();
    }

    private static void WriteLine(string type, string log, string? category = null, string? className = null, string? methondName = null, int lineNumber = 0)
    {
        if (_loger == null) return;
        if (IsEnable == false) return;

        string msg = string.Format("{0}\t{1}", DateTime.Now.ToString("HH:mm:ss.fff"), type);

        if (category != null) {
            msg += "\t" + category;
        }

        if (log != null) {
            msg += "\t" + log;
        }

        if (type != "Info") {
            if (className != null) {
                className = Path.GetFileName(className);
                msg += "\t" + className;
            }
            if (methondName != null) {
                msg += "\t" + methondName;
            }
            msg += "\t" + lineNumber.ToString();
        }

        Trace.WriteLine(msg);

        if (_isOpened) {
            _loger.WriteLine(msg);
            if (IsAutoFlush) {
                _loger.Flush();
            }
        }
    }

    public static void Info(string info, string? category = null,
        [CallerFilePath] string? className = null,
        [CallerMemberName] string? methondName = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if ((Filter & LogFilter.Info) != LogFilter.Info) return;
        WriteLine("Info", info, category, className, methondName, lineNumber);
    }

    public static void Warn(string warn, string? category = null,
        [CallerFilePath] string? className = null,
        [CallerMemberName] string? methondName = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if ((Filter & LogFilter.Warn) != LogFilter.Warn) return;
        WriteLine("Warn", warn, category, className, methondName, lineNumber);
    }

    public static void Error(string error, string? category = null,
        [CallerFilePath] string? className = null,
        [CallerMemberName] string? methondName = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if ((Filter & LogFilter.Error) != LogFilter.Error) return;
        WriteLine("Error", error, category, className, methondName, lineNumber);
    }

    public static void Debug(string debug, string? category = null,
        [CallerFilePath] string? className = null,
        [CallerMemberName] string? methondName = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if ((Filter & LogFilter.Debug) != LogFilter.Debug) return;
        WriteLine("Debug", debug, category, className, methondName, lineNumber);
    }

    public static void Exception(Exception ex)
    {
        Exception inner = ex;
        string msg = ex.Message;

        while (inner.InnerException != null) {
            msg += "\r\n" + inner.InnerException.Message;
            inner = inner.InnerException;
        }

        if (ex.StackTrace != null) {
            msg += "\r\n堆栈信息：\r\n" + ex.StackTrace;
        }

        WriteLine("Exception", msg);
    }
}
