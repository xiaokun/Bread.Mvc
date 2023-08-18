using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Bread.Mvc.Avalonia;

public class BoolToValueConverter<T> : IValueConverter
{
    public T? FalseValue { get; set; } = default(T);

    public T? TrueValue { get; set; } = default(T);

    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if(FalseValue == null || TrueValue == null) {
            throw new InvalidOperationException("TrueValue or FalseValue must be setted before convert.");
        }

        if (value == null)
            return FalseValue;
        else
            return (bool)value ? TrueValue : FalseValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return value != null ? value.Equals(TrueValue) : false;
    }
}

public class BoolToBrushConverter : BoolToValueConverter<Brush> { }

public class BoolToObjectConverter : BoolToValueConverter<Object> { }


public class BoolConvertConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null)
            return true;
        else
            return (bool)value ? false : true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return value != null ? value.Equals(true) : false;
    }
}


public class OfTypeToBoolConverter : IValueConverter
{
    public Type? Type { get; set; } = null;

    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if(Type == null) throw new InvalidOperationException("Type must be setted before convert.");

        if (value == null) return false;
        if (value.GetType() == Type) return true;
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
