namespace Bread.Mvc;


public interface IFileProvider
{
    /// <summary>
    /// 使用系统资源管理器打开文件
    /// </summary>
    /// <param name="path"></param>
    void OpenFileInFileManager(string path);

    /// <summary>
    /// 使用系统默认程序打开文件
    /// </summary>
    /// <param name="path"></param>
    void OpenFile(string path);

    /// <summary>
    /// 拷贝文件，如果文件较大则弹出系统对话框
    /// </summary>
    /// <param name="path"></param>
    /// <param name="destinationPath"></param>
    void CopyFile(string path, string destinationPath);

    /// <summary>
    /// 移动文件，如果文件较大则弹出系统对话框
    /// </summary>
    /// <param name="path"></param>
    /// <param name="destinationPath"></param>
    void MoveFile(string path, string destinationPath);

    /// <summary>
    /// 将文件删除到回收站，如果文件较大则弹出系统对话框
    /// </summary>
    /// <param name="path"></param>
    void DeleteFile(string path);
}
