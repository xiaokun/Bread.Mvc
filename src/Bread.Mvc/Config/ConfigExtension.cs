namespace Bread.Mvc;

public static class ConfigExtension
{
    private const string KeyValueSeperator = "|#|";

    public static void Load(this Config config, string group, string key, Action<bool> action)
    {
        var content = config[group, key];

        try {
            if (string.IsNullOrWhiteSpace(content)) return;
            if (bool.TryParse(content, out bool result)) {
                action(result);
            }
            else {
                Log.Error($"convert value {content ?? "null"} from [{group}]:{key} to type bool failed.");
            }
        }
        catch (Exception ex) {
            Log.Error($"convert value {content ?? "null"} from [{group}]:{key} to type bool failed.");
            Log.Exception(ex);
        }
    }

    public static void Load(this Config config, string group, string key, Action<string> action)
    {
        var content = config[group, key];

        try {
            if (string.IsNullOrWhiteSpace(content)) {
                action(string.Empty);
            }
            else {
                action(content);
            }
        }
        catch (Exception ex) {
            Log.Error($"convert value {content ?? "null"} from [{group}]:{key} to type string failed.");
            Log.Exception(ex);
        }
    }

    public static void Load<T>(this Config config, string group, string key, Action<T> action,
        IFormatProvider? format = null) where T : IParsable<T>
    {
        var content = config[group, key];

        try {
            if (string.IsNullOrWhiteSpace(content)) return;
            if (T.TryParse(content, format, out T? result)) {
                action(result);
            }
            else {
                Log.Error($"convert value {content ?? "null"} from [{group}]:{key} to type {nameof(T)} failed.");
            }
        }
        catch (Exception ex) {
            Log.Error($"convert value {content ?? "null"} from [{group}]:{key} to type {nameof(T)} failed.");
            Log.Exception(ex);
        }
    }

    #region List

    public static List<string> LoadList(this Config config, string group)
    {
        var result = new List<string>();

        try {
            var cnt = config[group, "Count"];
            if (int.TryParse(cnt, out var count) == false) {
                return result;
            }

            for (int i = 0; i < count; i++) {
                var content = config[group, i.ToString()];
                if (string.IsNullOrWhiteSpace(content)) continue;
                result.Add(content);
            }
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
        return result;
    }


    public static List<T> LoadList<T>(this Config config, string group,
        IFormatProvider? valueFormat = null) where T : IParsable<T>
    {
        var result = new List<T>();

        try {
            var cnt = config[group, "Count"];
            if (int.TryParse(cnt, out var count) == false) {
                return result;
            }

            for (int i = 0; i < count; i++) {
                var content = config[group, i.ToString()];
                if (string.IsNullOrWhiteSpace(content)) continue;

                if (T.TryParse(content, valueFormat, out T? item)) {
                    result.Add(item);
                }
            }
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
        return result;
    }

    public static void SaveList<T>(this Config config, string group, IList<T> list) where T : notnull
    {
        config.Clear(group);

        int count = list.Count;
        config[group, "Count"] = count.ToString();
        if (count == 0) return;

        for (int i = 0; i < count; i++) {
            var item = list[i];
            if (item == null) continue;
            config[group, i.ToString()] = item.ToString();
        }
    }

    #endregion

    #region Dictionary


    public static void LoadDictionary(this Config config, string group, Dictionary<string, string> dics)
    {
        try {
            dics.Clear();
            if (int.TryParse(config[group, "Count"], out var count) == false) {
                return;
            }

            for (int i = 0; i < count; i++) {
                var text = config[group, i.ToString()];
                if (string.IsNullOrWhiteSpace(text)) continue;
                var splits = text.Split(KeyValueSeperator);
                if (splits.Length != 2) continue;

                if (string.IsNullOrWhiteSpace(splits[0]) == false &&
                    string.IsNullOrWhiteSpace(splits[1]) == false) {
                    if (dics.ContainsKey(splits[0]))
                        dics[splits[0]] = splits[1];
                    else {
                        dics.Add(splits[0], splits[1]);
                    }
                }
            }
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
    }

    public static void LoadDictionary<V>(this Config config, string group, Dictionary<string, V> dics,
        IFormatProvider? valueFormat = null) where V : IParsable<V>
    {
        try {
            dics.Clear();

            if (int.TryParse(config[group, "Count"], out var count) == false) {
                return;
            }

            for (int i = 0; i < count; i++) {
                var text = config[group, i.ToString()];
                if (string.IsNullOrWhiteSpace(text)) continue;
                var splits = text.Split(KeyValueSeperator);
                if (splits.Length != 2) continue;

                if (string.IsNullOrWhiteSpace(splits[0]) == false &&
                    string.IsNullOrWhiteSpace(splits[1]) == false) {
                    if (V.TryParse(splits[1], valueFormat, out var data)) {
                        if (dics.ContainsKey(splits[0]))
                            dics[splits[0]] = data;
                        else {
                            dics.Add(splits[0], data);
                        }
                    }
                }
            }
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
    }

    public static void LoadDictionary<K>(this Config config, string group, Dictionary<K, string> dics,
         IFormatProvider? keyFormat = null) where K : IParsable<K>
    {
        try {
            dics.Clear();
            if (int.TryParse(config[group, "Count"], out var count) == false) {
                return;
            }

            for (int i = 0; i < count; i++) {
                var text = config[group, i.ToString()];
                if (string.IsNullOrWhiteSpace(text)) continue;
                var splits = text.Split(KeyValueSeperator);
                if (splits.Length != 2) continue;

                if (K.TryParse(splits[0], keyFormat, out var key)) {
                    if (string.IsNullOrEmpty(splits[1]) == false) {
                        if (dics.ContainsKey(key))
                            dics[key] = splits[1];
                        else {
                            dics.Add(key, splits[1]);
                        }
                    }
                }
            }
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
    }

    public static void LoadDictionary<K, V>(this Config config, string group, Dictionary<K, V> dics,
        IFormatProvider? keyFormat = null, IFormatProvider? valueFormat = null)
        where K : IParsable<K>
        where V : IParsable<V>
    {
        try {
            dics.Clear();

            if (int.TryParse(config[group, "Count"], out var count) == false) {
                return;
            }

            for (int i = 0; i < count; i++) {
                var text = config[group, i.ToString()];
                if (string.IsNullOrWhiteSpace(text)) continue;
                var splits = text.Split(KeyValueSeperator);
                if (splits.Length != 2) continue;

                if (string.IsNullOrWhiteSpace(splits[0]) == false &&
                    string.IsNullOrWhiteSpace(splits[1]) == false) {
                    if (K.TryParse(splits[0], keyFormat, out var key) == false) {
                        return;
                    }

                    if (V.TryParse(splits[1], valueFormat, out var value) == false) {
                        return;
                    }

                    if (dics.ContainsKey(key))
                        dics[key] = value;
                    else {
                        dics.Add(key, value);
                    }
                }
            }
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
    }

    public static void SaveDictionary<K, V>(this Config config, string group, Dictionary<K, V> dics)
         where K : notnull
         where V : notnull
    {
        config.Clear(group);

        int count = dics.Count;
        config[group, "Count"] = count.ToString();

        if (count == 0) return;

        int index = 0;
        foreach (var item in dics) {
            config[group, index.ToString()] = $"{item.Key.ToString()}{KeyValueSeperator}{item.Value.ToString()}";
            index++;
        }
    }

    #endregion

}
