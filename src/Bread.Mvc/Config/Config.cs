using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Bread.Mvc;

/// <summary>
/// ini file configuration
/// </summary>
public class Config
{
    public string Path => _path;

    public string? this[string key]
    {
        get {
            return GetValue(string.Empty, key);
        }
        set {
            if (string.IsNullOrWhiteSpace(value) == false) {
                SetValue(string.Empty, key, value);
            }
        }
    }

    public string? this[string group, string key]
    {
        get {
            if (group.EndsWith("Model")) group = group[0..^5];
            return GetValue(group, key);
        }
        set {
            if (group.EndsWith("Model")) group = group[0..^5];
            if (string.IsNullOrWhiteSpace(value) == false) {
                SetValue(group, key, value);
            }
        }
    }

    public bool HasModified { get; private set; } = false;

    string _path;
    Dictionary<string, Group> Groups = new();
    Dictionary<string, string> Items = new();

    public Config(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));
        _path = path;

        var folder = System.IO.Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(folder)) {
            Log.Error($"");
            return;
        }

        if(Directory.Exists(folder) == false) {
            try {
                Directory.CreateDirectory(folder);
            }
            catch(Exception ex) {
                Log.Exception(ex);
            }
        }
    }

    public void Load()
    {
        Groups.Clear();
        Items.Clear();
        HasModified = false;

        if (!File.Exists(_path)) {
            Log.Error($"配置文件加载失败：{_path}");
            return;
        }

        try {
            var lines = File.ReadAllLines(_path);
            if (lines == null || lines.Length == 0) return;

            Group? group = null;

            foreach (var l in lines) {
                var line = l.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                if (TryParseCategory(line, out var name)) {
                    if (group != null) {
                        Groups.Add(group.Name, group);
                    }
                    group = new(name);
                    continue;
                }

                if (TryParseLine(line, out var item)) {
                    if (group == null) Items.Add(item.Value.Key, item.Value.Value);
                    else group.Items.Add(item.Value.Key, item.Value.Value);
                }
            }

            if (group != null) Groups.Add(group.Name, group); //add the last category
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
        finally {
            HasModified = false;
        }
    }

    public void Save()
    {
        if (!HasModified) return;

        string bakPath = _path + ".bak";
        try {
            if (File.Exists(_path)) File.Copy(_path, bakPath, true);
        }
        catch (Exception ex) {
            Log.Exception(ex);
            return;
        }

        bool successd = false;
        try {
            using var writer = File.CreateText(_path);
            foreach (var item in Items) {
                writer.WriteLine($"{item.Key} = {item.Value}");
            }
            foreach (var g in Groups) {
                g.Value.SaveTo(writer);
            }
            writer.Flush();
            successd = true;
            HasModified = false;
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
        finally {
            if (File.Exists(bakPath)) {
                if (!successd) File.Copy(bakPath, _path, true);
                File.Delete(bakPath);
            }
        }
    }

    public void Rename(string name)
    {
        var folder = System.IO.Path.GetDirectoryName(_path);
        if (string.IsNullOrWhiteSpace(folder)) return;

        var old = _path;

        var ext = System.IO.Path.GetExtension(_path);
        name = System.IO.Path.GetFileNameWithoutExtension(name);
        var path = System.IO.Path.Combine(folder, $"{name}{ext ?? string.Empty}");
        SaveAs(path);

        try {
            if (File.Exists(path)) {
                File.Delete(old);
            }
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
    }

    public void SaveAs(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException();

        Save();

        try {
            File.Copy(_path, path, true);
            _path = path;
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
    }

    public void Clear(string group)
    {
        if(string.IsNullOrEmpty(group)) {
            if(Items.Count > 0) HasModified = true;
            Items.Clear();
            return;
        }

        if (group.EndsWith("Model")) group = group[0..^5];
        if (Groups.ContainsKey(group) == false) return;
        var g = Groups[group];
        g.Clear();
        HasModified = true;
    }


    /// <summary>
    /// 获取Category的Value
    /// </summary>
    /// <param name="group"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private string? GetValue(string group, string key)
    {
        if (string.IsNullOrEmpty(key)) return null;

        if (string.IsNullOrEmpty(group)) {
            if (Items.ContainsKey(key)) {
                return Items[key];
            }
            return null;
        }

        if (Groups.ContainsKey(group)) {
            var c = Groups[group];
            if (c.Items.ContainsKey(key))
                return c.Items[key];
        }

        return null;
    }

    private void SetValue(string group, string key, string value)
    {
        if (string.IsNullOrEmpty(key)) return;

        if (string.IsNullOrEmpty(group)) {
            if (Items.ContainsKey(key)) {
                if (Items[key] != value) {
                    Items[key] = value;
                    HasModified = true;
                }
            }
            else {
                Items.Add(key, value);
                HasModified = true;
            }
            return;
        }

        if (Groups.ContainsKey(group) == false) {
            Groups.Add(group, new(group));
        }

        var c = Groups[group];
        if (c.Items.ContainsKey(key)) {
            if (c.Items[key] != value) {
                c.Items[key] = value;
                HasModified = true;
            }
        }
        else {
            c.Items.Add(key, value);
            HasModified = true;
        }
    }

    private bool TryParseCategory(string line, [NotNullWhen(true)] out string? name)
    {
        name = null;
        if (line[0] != '[' || line[^1] != ']') return false;
        name = line[1..^1];
        if (string.IsNullOrWhiteSpace(name)) return false;
        return true;
    }

    private bool TryParseLine(string line, [NotNullWhen(true)] out KeyValuePair<string, string>? pair)
    {
        pair = null;
        var index = line.IndexOf('=');
        if (index <= 0 || index >= line.Length - 1) return false;

        var key = line[..index].Trim();
        if (string.IsNullOrEmpty(key)) return false;

        var value = line[(index + 1)..].Trim();
        if (string.IsNullOrEmpty(value)) return false;

        pair = new KeyValuePair<string, string>(key, value);
        return true;
    }

    class Group
    {
        public string Name { get; private set; } = string.Empty;

        public Dictionary<string, string> Items { get; } = new();

        public Group(string? name = null)
        {
            if (name != null) {
                if (name.EndsWith("Model")) {
                    name = name[0..^5];
                }
                Name = name;
            }
        }

        public void SaveTo(StreamWriter writer)
        {
            writer.WriteLine();
            if (string.IsNullOrEmpty(Name) == false) {
                writer.WriteLine("[" + Name + "]");
            }

            foreach (var item in Items) {
                writer.WriteLine($"{item.Key} = {item.Value}");
            }
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}
