using System;
using System.Linq.Expressions;
using System.Reflection;
using Avalonia.Controls;

namespace Bread.Mvc.Avalonia;

public static class NumericUpDownReactiveHelper
{
    /// <summary>
    /// NumericUpDown Bind to Model's double property
    /// </summary>
    public static void BindTo<TModel>(this NumericUpDown box, TModel m, Expression<Func<TModel, double>> exp)
        where TModel : Model
    {
        var property = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var target = (double?)property.GetValue(m);
                if (target == null) return;

                if (box.Value != null) {
                    var value = (double)box.Value;
                    if (Math.Abs(value - target.Value) < 0.0001) {
                        return;
                    }
                }

                box.Value = (decimal)target.Value;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, property.Name);

        box.ValueChanged += (s, e) => {
            try {

                var target = (double?)property.GetValue(m);
                if (target == null) return;

                if (box.Value != null) {
                    var value = (double)box.Value;
                    if (Math.Abs(value - target.Value) < 0.0001) {
                        return;
                    }
                    property.SetValue(m, (double)box.Value);
                }
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }

    /// <summary>
    /// NumericUpDown Bind to Model's float property
    /// </summary>
    public static void BindTo<TModel>(this NumericUpDown box, TModel m, Expression<Func<TModel, float>> exp)
        where TModel : Model
    {
        var property = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var target = (float?)property.GetValue(m);
                if (target == null) return;

                if (box.Value != null) {
                    var value = (float)box.Value;
                    if (Math.Abs(value - target.Value) < 0.0001f) {
                        return;
                    }
                }

                box.Value = (decimal)target.Value;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, property.Name);

        box.ValueChanged += (s, e) => {
            try {

                var target = (float?)property.GetValue(m);
                if (target == null) return;

                if (box.Value != null) {
                    var value = (float)box.Value;
                    if (Math.Abs(value - target.Value) < 0.0001f) {
                        return;
                    }
                    property.SetValue(m, (float)box.Value);
                }
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }

    /// <summary>
    /// NumericUpDown Bind to Model's int property
    /// </summary>
    public static void BindTo<TModel>(this NumericUpDown box, TModel m, Expression<Func<TModel, int>> exp)
        where TModel : Model
    {
        var property = (PropertyInfo)((MemberExpression)exp.Body).Member;

        m.Watch(() => {
            try {
                var target = (int?)property.GetValue(m);
                if (target == null) return;

                if (box.Value != null) {
                    var value = (int)box.Value;
                    if (value == target.Value) {
                        return;
                    }
                }

                box.Value = target.Value;
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }, property.Name);

        box.ValueChanged += (s, e) => {
            try {

                var target = (int?)property.GetValue(m);
                if (target == null) return;

                if (box.Value != null) {
                    var value = (int)box.Value;
                    if (value == target.Value) {
                        return;
                    }
                    property.SetValue(m, (int)box.Value);
                }
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        };
    }

}
