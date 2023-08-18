using Avalonia.Data.Converters;

namespace Bread.Mvc.Avalonia;

public class NullToValueConverter<T> : IValueConverter
{
    public T? FalseValue { get; set; }
    public T? TrueValue { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (FalseValue == null || TrueValue == null) {
            throw new InvalidOperationException("TrueValue or FalseValue must be setted before convert.");
        }

        if (value == null)
            return TrueValue;
        else
            return FalseValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NullToBoolConverter : NullToValueConverter<bool> { }
