using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Bread.Mvc.Avalonia;

public class LighterConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) throw new InvalidProgramException();
        var brush = value as SolidColorBrush;
        if (brush == null) throw new InvalidProgramException();
        var color = brush.Color;
        float fraction = 0.1f;
        byte r = (byte)Math.Max(0, Math.Min(255, color.R + 255 * fraction));
        byte g = (byte)Math.Max(0, Math.Min(255, color.G + 255 * fraction));
        byte b = (byte)Math.Max(0, Math.Min(255, color.B + 255 * fraction));
        return new SolidColorBrush(Color.FromRgb(r, g, b));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class DarkerConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) throw new InvalidProgramException();
        var brush = value as SolidColorBrush;
        if (brush == null) throw new InvalidProgramException();
        var color = brush.Color;
        float fraction = 0.1f;
        byte r = (byte)Math.Max(0, Math.Min(255, color.R - 255 * fraction));
        byte g = (byte)Math.Max(0, Math.Min(255, color.G - 255 * fraction));
        byte b = (byte)Math.Max(0, Math.Min(255, color.B - 255 * fraction));
        return new SolidColorBrush(Color.FromRgb(r, g, b));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class IsGreaterThanConverter : IValueConverter
{
    public static readonly IValueConverter Instance = new IsGreaterThanConverter();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        double intValue = System.Convert.ToDouble(value);
        double compareToValue = System.Convert.ToDouble(parameter);
        return intValue > compareToValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class PercentConverter : IValueConverter
{
    public double Percent { get; set; } = 1.0;

    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null) throw new InvalidProgramException();

        double dst = ((double)value) * Percent;

        if (targetType == typeof(double)) {
            return dst;
        }
        else if (targetType == typeof(Thickness)) {
            return new Thickness(dst);
        }
        else {
            throw new NotImplementedException();
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class IndexIncreaseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index) {
            return index + 1;
        }
        return -1;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index) {
            return index - 1;
        }
        return -1;
    }
}

public class DoubleToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType != typeof(string)) {
            throw new InvalidProgramException("DoubleToStringConverter target type error");
        }

        if (value is double d) {
            return ((int)(d + 0.5)).ToString();
        }
        return "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
