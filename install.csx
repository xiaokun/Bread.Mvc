using System.Diagnostics;
using System.Runtime.CompilerServices;

try {

    string old = string.Empty;
    string version = string.Empty ;

    if (Args.Count == 1) {
        old = version = Args[0] ?? string.Empty;
    }
    else if(Args.Count == 2){
        old = Args[0] ?? string.Empty;
        version = Args[1] ?? string.Empty;
    }
    else {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"参数数量{Args.Count}不正确");
        return;
    }

    if (string.IsNullOrWhiteSpace(old) || string.IsNullOrWhiteSpace(version)) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"参数不正确, old version: {old}, new version: {version}");
        return;
    }

    // build Bread.Utility
    var success = Helper.InstallPackage("Bread.Utility", old, version);
    if (!success) return;

    success = Helper.InstallPackage("Bread.Mvc", old, version);
    if (!success) return;

    success = Helper.InstallPackage("Bread.Mvc.Avalonia", old, version);
    if (!success) return;

    success = Helper.InstallPackage("Bread.Mvc.WPF", old, version);
    if (!success) return;

    Console.WriteLine("Install success!", ConsoleColor.Green);
}
finally {
    Console.ForegroundColor = ConsoleColor.White;
}

public static class Helper
{
    public static void RunCmd(string cmds, string workingFolder)
    {
        using (Process process = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = "cmd.exe",
                Arguments = $"/c {cmds}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingFolder,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            }
        }) {
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            Console.WriteLine(output);

            if (!string.IsNullOrEmpty(error)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Run command error: {process.StartInfo.Arguments}");
                Console.WriteLine(error);
            }
        }
    }

    public static bool InstallPackage(string name, string oldVersion, string newVersion)
    {
        var folder = GetScriptFolder();
        folder = Path.Combine(folder, "src", name);
        var proj = Path.Combine(folder, $"{name}.csproj");
        if (File.Exists(proj) == false) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[error] {name}.csproj file not found.");
            return false;
        }

        Directory.Delete(Path.Combine(folder, "bin", "Release"), true);

        var content = File.ReadAllText(proj);
        content = content.Replace(oldVersion, newVersion);
        File.WriteAllText(proj, content);

        RunCmd($"dotnet clean", folder);
        RunCmd($"dotnet restore", folder);
        RunCmd($"dotnet build -c Release", folder);

        var pkg = Path.Combine(folder, "bin", "Release", $"{name}.{newVersion}.nupkg");
        if (File.Exists(pkg) == false) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[error] {name}.{newVersion}.nupkg not found. {pkg}");
            return false;
        }

        var nuget = @"C:\Program Files (x86)\Microsoft SDKs\NuGetPackages";
        var cmds = $"nuget add {name}.{newVersion}.nupkg -Source \"{nuget}\"";
        RunCmd(cmds, Path.Combine(folder, "bin", "Release"));
        nuget = Path.Combine(nuget, name.ToLower(), newVersion);
        if (Directory.Exists(nuget) == false) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[error] {name}.{newVersion}.nupkg install failed.");
            return false;
        }
        return true;
    }

    public static string GetScriptFolder([CallerFilePath] string path = null) => Path.GetDirectoryName(path);

}
