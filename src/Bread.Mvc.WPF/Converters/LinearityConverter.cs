using System.Windows.Data;

namespace Bread.Mvc.WPF;


public class LinearityConverter : IValueConverter
{

	public double Factor { get; set; } = 1.0;

	public double Value { get; set; } = 0.0;

	public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		if (value is bool b) return System.Convert.ToDouble(b) * Factor + Value;
		else if (value is int i) return System.Convert.ToDouble(i) * Factor + Value;
		else if (value is double d) return System.Convert.ToDouble(d) * Factor + Value;
		else return 1.0;
	}

	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		return value != null ? value.Equals(true) : false;
	}
}
