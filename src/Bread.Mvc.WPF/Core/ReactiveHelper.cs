using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Bread.Utility;
using Bread.Utility.IO;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using TextBox = System.Windows.Controls.TextBox;
using TextBoxBase = System.Windows.Controls.Primitives.TextBoxBase;

namespace Bread.Mvc.WPF;

public static class ReactiveHelper
{
    /// <summary>
    /// TextBox Bind to Model's T property
    /// </summary>
    /// <typeparam name="M">Model type</typeparam>
    /// <typeparam name="T">Property type</typeparam>
    /// <param name="box">TextBox</param>
    /// <param name="m">Model</param>
    /// <param name="exp">property</param>
    public static void Bind<TModel, TPropertty>(this TextBox box, TModel m, Expression<Func<TModel, TPropertty>> exp)
        where TModel : Model
    {
        var propertyInfo = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var obj = (TPropertty?)propertyInfo.GetValue(m);
                if (obj == null) return;

                var text = obj.ToText();
                if (string.IsNullOrEmpty(box.Text)) {
                    box.Text = text;
                    return;
                }

                var value = box.Text.ToProperty<TPropertty>();
                if (value == null) return;
                if (value.Equals(obj)) {
                    return;
                }

                if (box.Text == text) return;
                box.Text = text;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, propertyInfo.Name);

        box.TextChanged += (s, e) => {
            try {
                var text = box.Text;
                if (string.IsNullOrEmpty(text)) {
                    propertyInfo.SetValue(m, default(TPropertty));
                    return;
                }

                var value = text.ToProperty<TPropertty>();
                if (value != null) {
                    propertyInfo.SetValue(m, value);
                }
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }

    /// <summary>
    /// SwitchButton Bind to Model's bool property
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <param name="btn"></param>
    /// <param name="m"></param>
    /// <param name="exp">property</param>
    public static void Bind<M>(this SwitchButton btn, M m, Expression<Func<M, bool>> exp) where M : Model
    {
        var propertyInfo = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var value = (bool?)propertyInfo.GetValue(m);
                if (value == btn.IsChecked) return;
                btn.IsChecked = value;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, propertyInfo.Name);

        btn.OnSwitched += () => {
            try {
                var isChecked = btn.IsChecked == true;
                propertyInfo.SetValue(m, isChecked);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }

    /// <summary>
    /// CheckBox Bind to Model's bool property
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <param name="btn"></param>
    /// <param name="m"></param>
    /// <param name="exp">property</param>
    public static void Bind<M>(this CheckBox box, M m, Expression<Func<M, bool>> exp) where M : Model
    {
        var propertyInfo = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var value = (bool?)propertyInfo.GetValue(m);
                if (value == box.IsChecked) return;
                box.IsChecked = value;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, propertyInfo.Name);

        box.Checked += (o, s) => {
            try {
                var isChecked = box.IsChecked == true;
                propertyInfo.SetValue(m, isChecked);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };

        box.Unchecked += (o, s) => {
            try {
                var isChecked = box.IsChecked == true;
                propertyInfo.SetValue(m, isChecked);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }

    /// <summary>
    /// Slider Bind to Model's double property
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <param name="btn"></param>
    /// <param name="m"></param>
    /// <param name="exp">property</param>
    public static void Bind<TModel>(this Slider slider, TModel m, Expression<Func<TModel, double>> exp)
        where TModel : Model
    {
        var propertyInfo = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var value = (double?)propertyInfo.GetValue(m);
                if (value == null) return;
                if (Math.Abs(slider.Value - value.Value) < 0.001) return;
                slider.Value = value.Value;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, propertyInfo.Name);

        slider.ValueChanged += (s, e) => {
            try {
                var value = (double?)propertyInfo.GetValue(m);
                if (value == null) return;
                if (Math.Abs(slider.Value - value.Value) < 0.001) return;
                propertyInfo.SetValue(m, slider.Value);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }


    /// <summary>
    /// SelectedButton Bind to Model's bool property
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <param name="btn"></param>
    /// <param name="m"></param>
    /// <param name="exp">property</param>
    public static void Bind<M>(this Button btn, M m, Expression<Func<M, bool>> exp) where M : Model
    {
        var propertyInfo = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var value = (bool?)propertyInfo.GetValue(m);
                if(value == null) return;
                if (value == btn.IsChecked) return;
                btn.IsChecked = value.Value;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, propertyInfo.Name);


        btn.Checked += () => {
            try {
                propertyInfo.SetValue(m, btn.IsChecked);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }


    /// <summary>
    /// SelectedButton Bind to Model's bool property
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <param name="btn"></param>
    /// <param name="m"></param>
    /// <param name="propertyName"></param>
    public static void Bind<M, T>(this ComboBox box, M m, Expression<Func<M, T>> exp) where M : Model
    {
        var propertyInfo = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var value = (T?)propertyInfo.GetValue(m);
                if (value == null) return;

                var text = value.ToText();
                if (box.Text == text) return;

                for (int i = 0; i < box.Items.Count; i++) {
                    var boxItem = box.Items[i];
                    if (boxItem is ListItemNode<T> item) {
                        if (item.Value.ToText() == text) {
                            box.SelectedIndex = i;
                            box.Text = item.Title;
                            return;
                        }
                    }
                    else if (boxItem is T strItem) {
                        if (strItem.ToText() == text) {
                            box.SelectedIndex = i;
                            box.Text = strItem.ToText();
                            return;
                        }
                    }
                }

                box.Text = text;
                box.SelectedIndex = -1;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }

        }, propertyInfo.Name);

        if (box.IsEditable) {
            box.AddHandler(TextBoxBase.TextChangedEvent,
                  new TextChangedEventHandler((s, e) => {
                      try {
                          var text = box.Text;
                          if (string.IsNullOrEmpty(text)) {
                              propertyInfo.SetValue(m, default(T));
                              return;
                          }

                          var value = text.ToProperty<T>();
                          propertyInfo.SetValue(m, value);
                      }
                      catch (Exception ex) {
                          Log.Exception(ex);
                      }
                  }));
        }

        box.SelectionChanged += (s, e) => {
            try {
                if (box.SelectedIndex == -1) {
                    box.Text = "";
                    propertyInfo.SetValue(m, default(T));
                    return;
                }

                var value = (T?)propertyInfo.GetValue(m);
                if (value == null) return;
                if (box.SelectedItem is ListItemNode<T> item) {
                    var text = item.Value.ToText();
                    if (value.ToText() == text) {
                        return;
                    }

                    if (box.IsEditable) box.Text = text;
                    propertyInfo.SetValue(m, item.Value);
                }
                else if (box.SelectedItem is T itemT) {
                    var text = itemT.ToText();
                    if (value.ToText() == text) {
                        return;
                    }

                    if (box.IsEditable) box.Text = text;
                    propertyInfo.SetValue(m, itemT);
                }

            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }
}
