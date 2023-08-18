using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic;

namespace Bread.Mvc.WPF;

public static class GridExtend
{

    #region Rows

    public static DependencyProperty GridRowsProperty =
                 DependencyProperty.RegisterAttached("GridRows", typeof(string),
                     MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(Grid),
                     new FrameworkPropertyMetadata(string.Empty,
                         FrameworkPropertyMetadataOptions.AffectsArrange,
                         new PropertyChangedCallback(GridRowsPropertyChanged)));

    public static string GetGridRows(Grid grid)
    {
        return Convert.ToString(grid.GetValue(GridRowsProperty)) ?? string.Empty;
    }

    public static void SetGridRows(Grid grid, string Value)
    {
        grid.SetValue(GridRowsProperty, Value);
    }

    private static void GridRowsPropertyChanged(object Sender, DependencyPropertyChangedEventArgs e)
    {
        var grid = Sender as Grid;
        if (grid == null)
            throw new Exception("Only elements of type 'Grid' can utilize the 'GridRows' attached property");

        var rows = GetGridRows(grid).Split(Convert.ToChar(","));
        grid.RowDefinitions.Clear();

        foreach (var row in rows) {
            switch (row.Trim().ToLower()) {
                case "auto":
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                    break;
                case "*":
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    break;
                default:
                    if (System.Text.RegularExpressions.Regex.IsMatch(row, "^\\d+\\*$")) {
                        grid.RowDefinitions.Add(new RowDefinition {
                            Height = new GridLength(
                          Convert.ToInt32(row.Substring(0, row.IndexOf(Convert.ToChar("*")))), GridUnitType.Star)
                        });
                    }
                    else if (Information.IsNumeric(row)) {
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(Convert.ToDouble(row), GridUnitType.Pixel) });
                    }
                    else {
                        throw new Exception("The only acceptable value for the 'GridRows' " +
                          "attached property is a comma separated list comprised of the following options:" +
                          Constants.vbCrLf + Constants.vbCrLf + "Auto,*,x (where x is the pixel " +
                          "height of the row), x* (where x is the row height multiplier)");
                    }
                    break;
            }
        }
    }
    #endregion


    #region Columns

    public static DependencyProperty GridColumnsProperty =
                 DependencyProperty.RegisterAttached("GridColumns", typeof(string),
                     MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(Grid),
                     new FrameworkPropertyMetadata(string.Empty,
                        FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(GridColumnsPropertyChanged)));


    public static string GetGridColumns(Grid grid)
    {
        return Convert.ToString(grid.GetValue(GridColumnsProperty)) ?? string.Empty;
    }

    public static void SetGridColumns(Grid grid, string Value)
    {
        grid.SetValue(GridColumnsProperty, Value);
    }

    private static void GridColumnsPropertyChanged(object Sender, DependencyPropertyChangedEventArgs e)
    {
        var grid = Sender as Grid;
        if (grid == null)
            throw new Exception("Only elements of type 'Grid' can " +
              "utilize the 'GridColumns' attached property");

        var columns = GetGridColumns(grid).Split(Convert.ToChar(","));
        grid.ColumnDefinitions.Clear();

        foreach (var column in columns) {
            switch (column.Trim().ToLower()) {
                case "auto":
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    break;
                case "*":
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    break;
                default:
                    if (System.Text.RegularExpressions.Regex.IsMatch(column, "^\\d+\\*$")) {
                        grid.ColumnDefinitions.Add(new ColumnDefinition {
                            Width =
                          new GridLength(Convert.ToInt32(column.Substring(0,
                          column.IndexOf(Convert.ToChar("*")))), GridUnitType.Star)
                        });
                    }
                    else if (Information.IsNumeric(column)) {
                        grid.ColumnDefinitions.Add(new ColumnDefinition {
                            Width = new GridLength(Convert.ToDouble(column), GridUnitType.Pixel)
                        });
                    }
                    else {
                        throw new Exception("The only acceptable value for the 'GridColumns' attached " +
                           "property is a comma separated list comprised of the following options:" +
                           Constants.vbCrLf + Constants.vbCrLf +
                           "Auto,*,x (where x is the pixel width of the column), " +
                           "x* (where x is the column width multiplier)");
                    }
                    break;
            }
        }
    }
    #endregion
}
