using System.Diagnostics;

namespace Bread.Utility.IO;

public class ProcessHelper
{
    public static void ExeCmd(string target, params string[] paras)
    {
        string cmds = string.Empty;
        foreach (var s in paras) {
            cmds += s;
            cmds += " ";
        }
        cmds = cmds.TrimEnd(' ', '&');

        using (Process p = new Process()) {
            p.StartInfo.FileName = target;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = cmds;
            p.Start();
            p.WaitForExit();
            p.Close();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="paras"></param>
    public static Process AsyncExeCmd(string target, params string[] paras)
    {
        string cmds = string.Empty;
        foreach (var s in paras) {
            cmds += s;
            cmds += " ";
        }
        cmds = cmds.TrimEnd(' ', '&');

        Process p = new Process();
        p.StartInfo.FileName = target;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = false;
        p.StartInfo.Arguments = cmds;
        p.Start();
        return p;
    }

    public static string ShellCmd(params string[] paras)
    {
        string output = string.Empty;
        string cmds = string.Empty;
        foreach (var s in paras) {
            cmds += s;
            cmds += " ";
        }
        cmds = cmds.TrimEnd(' ', '&');
        cmds += " &exit"; //防止假死

        using (Process p = new Process()) {
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            p.StandardInput.WriteLine(cmds);
            p.StandardInput.AutoFlush = true;
            output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
        }
        return output;
    }
}
