using System.Linq.Expressions;
using System.Reflection;
using Avalonia.Controls;

namespace Bread.Mvc.Avalonia;

public static class ComboBoxReactiveHelper
{

    public static void InitBy<T>(this ComboBox box, List<ListItemNode<T>> args) where T : Enum
    {
        box.ItemsSource = args;
    }

    public static void InitBy<T>(this ComboBox box, params ListItemNode<T>[] args) where T : Enum
    {
        box.ItemsSource = args;
    }

    public static void InitBy<T>(this ComboBox box, List<T> values, params string[] titles) where T : Enum
    {
        var count = values.Count;
        if (count <= 0) return;

        var list = new List<ListItemNode<T>>();
        for (int i = 0; i < count; i++) {
            if (i >= titles.Length) break;
            list.Add(new(titles[i], values[i]));
        }
        box.ItemsSource = list;
    }

    public static void InitBy<T, D>(this ComboBox box, D descriptioner, params T[] values)
        where T : Enum
        where D : IEnumDescriptioner<T>
    {
        var list = new List<ListItemNode<T>>();
        foreach (var item in values) {
            var title = descriptioner.GetDescription(item);
            if (string.IsNullOrWhiteSpace(title)) continue;
            list.Add(new(title, item));
        }
        box.ItemsSource = list;
    }

    /// <summary>
    /// ComboBox Bind to Model's property
    /// </summary>
    public static void BindTo<M, T>(this ComboBox box, M m, Expression<Func<M, T>> exp) where M : Model
    {
        var p = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var value = (T?)p.GetValue(m);
                if (value == null) return;

                var index = 0;
                foreach (var item in box!.Items!) {
                    if (item is ListItemNode<T> mode) {
                        if (mode.Value == null) return;
                        if (mode.Value.Equals(value)) {
                            break;
                        }
                    }
                    else {
                        if (item.Equals(value))
                            break;
                    }
                    index++;
                }

                if (box.SelectedIndex != index) {
                    box.SelectedIndex = index;
                }
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, p.Name);

        box.SelectionChanged += (s, e) => {
            try {
                if (box.SelectedIndex == -1) return;
                var item = box.SelectedItem;
                if (item == null) return;

                var value = (T?)p.GetValue(m);
                if (item is ListItemNode<T> mode) {
                    if (mode.Value == null) return;
                    if (mode.Value.Equals(value)) {
                        return;
                    }
                    p.SetValue(m, mode.Value);
                }
                else {
                    if (item.Equals(value)) return;
                    p.SetValue(m, item);
                }
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }
}
