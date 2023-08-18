using System.Linq.Expressions;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;

namespace Bread.Mvc.Avalonia;

public static class ReactiveHelper
{
    public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> action)
    {
        return observable.Subscribe(new AnonymousObserver<T>(action));
    }

    /// <summary>
    /// TextBlock Bind to Model's T property
    /// </summary>
    public static void BindTo<M, P>(this TextBlock block, M m, Expression<Func<M, P>> exp, string? format = null)
        where M : Model
    {
        var property = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var obj = (P?)property.GetValue(m);
                if (obj == null) {
                    block.Text = string.Empty;
                    return;
                }

                if (string.IsNullOrEmpty(format)) {
                    var text = obj.ToString();
                    block.Text = text;
                }
                else {
                    var text = string.Format(format, obj);
                    block.Text = text;
                }
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, property.Name);
    }


    /// <summary>
    /// TextBox Bind to Model's T property
    /// </summary>
    public static void BindTo<M, P>(this TextBox box, M m, Expression<Func<M, P>> exp, string? format = null)
        where M : Model
        where P : IParsable<P>
    {
        var property = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var obj = (P?)property.GetValue(m);
                if (obj == null) return;

                var text = string.IsNullOrWhiteSpace(format) ? obj.ToString() : string.Format(format, obj);
                if (box.Text == text) return;
                box.Text = text;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, property.Name);

        box.TextChanged += (s, e) => {
            try {
                var text = box.Text;
                if (string.IsNullOrWhiteSpace(text)) {
                    property.SetValue(m, default(P));
                    return;
                }

                if (P.TryParse(text, null, out var obj)) {
                    property.SetValue(m, obj);
                };
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }

    /// <summary>
    /// RadioButton Bind to Model's Enum property
    /// </summary>
    public static void BindTo<M, E>(this RadioButton btn, M m, Expression<Func<M, E>> exp, E target)
        where M : Model
        where E : Enum
    {
        var property = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var value = (E?)property.GetValue(m);
                if (value == null) return;
                btn.IsChecked = value!.Equals(target);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, property.Name);

        btn.IsCheckedChanged += (s, e) => {
            try {
                if (btn.IsChecked == true) {
                    property.SetValue(m, target);
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
    public static void BindTo<M>(this ToggleSwitch btn, M m, Expression<Func<M, bool>> exp) where M : Model
    {
        var property = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var value = (bool?)property.GetValue(m);
                if (value == btn.IsChecked) return;
                btn.IsChecked = value;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, property.Name);


        btn.IsCheckedChanged += (s, e) => {
            try {
                var isChecked = btn.IsChecked == true;
                property.SetValue(m, isChecked);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }

    /// <summary>
    /// CheckBox Bind to Model's bool property
    /// </summary>
    public static void BindTo<M>(this CheckBox box, M m, Expression<Func<M, bool>> exp) where M : Model
    {
        var property = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var value = (bool?)property.GetValue(m);
                if (value == box.IsChecked) return;
                box.IsChecked = value;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, property.Name);

        box.IsCheckedChanged += (o, s) => {
            try {
                var isChecked = box.IsChecked == true;
                property.SetValue(m, isChecked);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }


    /// <summary>
    /// Slider Bind to Model's double property
    /// </summary>
    public static void BindTo<M>(this Slider slider, M m, Expression<Func<M, double>> exp)
        where M : Model
    {
        var property = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var value = (double?)property.GetValue(m);
                if (value == null) return;
                if (Math.Abs(slider.Value - value.Value) < 0.001) return;
                slider.Value = value.Value;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, property.Name);

        slider.Watch(() => {
            try {
                var value = (double?)property.GetValue(m);
                if (value == null) return;
                if (Math.Abs(slider.Value - value.Value) < 0.001) return;
                property.SetValue(m, slider.Value);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, nameof(Slider.Value));
    }


    /// <summary>
    /// SelectedButton Bind to Model's bool property
    /// </summary>
    public static void BindTo<M>(this ToggleButton btn, M m, Expression<Func<M, bool>> exp) where M : Model
    {
        var property = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var value = (bool?)property.GetValue(m);
                if (value == null || value == btn.IsChecked) return;
                btn.IsChecked = value;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, property.Name);


        btn.IsCheckedChanged += (s, e) => {
            try {
                var isChecked = btn.IsChecked == true;
                property.SetValue(m, isChecked);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }

    /// <summary>
    /// ToggleButton Bind to Model's Enum property
    /// </summary>
    public static void BindTo<M, E>(this ToggleButton btn, M m, Expression<Func<M, E>> exp, E target)
        where M : Model
        where E : Enum
    {
        var property = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var value = (E?)property.GetValue(m);
                if (value == null) return;
                btn.IsChecked = value!.Equals(target);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, property.Name);

        btn.IsCheckedChanged += (s, e) => {
            try {
                if (btn.IsChecked == true) {
                    property.SetValue(m, target);
                }
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }

}
