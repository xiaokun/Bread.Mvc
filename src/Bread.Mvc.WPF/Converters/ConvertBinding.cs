using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Binding = System.Windows.Data.Binding;

namespace Bread.Mvc.WPF;

[ContentProperty("Converter")]
public class MultiValueConverterAdapter : IMultiValueConverter
{
    public IValueConverter? Converter { get; set; } = null;

    private object? lastParameter = null;

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (Converter == null) return values[0]; // Required for VS design-time
        if (values.Length > 1) lastParameter = values[1];
        return Converter.Convert(values[0], targetType, lastParameter, culture);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (Converter == null) return new object[] { value }; // Required for VS design-time
        return new object[] { Converter.ConvertBack(value, targetTypes[0], lastParameter, culture) };
    }
}

public class ConvertBinding : MarkupExtension
{
    public Binding? Binding { get; set; }

    public IValueConverter? Converter { get; set; }

    public Binding? ConverterParameterBinding { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Binding == null || Converter == null || ConverterParameterBinding == null) {
            throw new InvalidOperationException("Binding or Converter or ConverterParameterBinding must be setted before ProvideValue.");
        }

        MultiBinding multiBinding = new MultiBinding();
        multiBinding.Bindings.Add(Binding);
        multiBinding.Bindings.Add(ConverterParameterBinding);
        MultiValueConverterAdapter adapter = new MultiValueConverterAdapter();
        adapter.Converter = Converter;
        multiBinding.Converter = adapter;
        return multiBinding.ProvideValue(serviceProvider);
    }
}
