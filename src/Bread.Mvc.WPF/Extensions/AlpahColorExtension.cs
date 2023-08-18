using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;

namespace Bread.Mvc.WPF;

[MarkupExtensionReturnType(typeof(Color))]
public class AlpahColorExtension : MarkupExtension
{
    public int Alpha { get; set; } = -1;

    /// <summary>
    ///  Constructor that takes no parameters
    /// </summary>
    public AlpahColorExtension()
    {
    }

    /// <summary>
    ///  Constructor that takes the resource key that this is a static reference to.
    /// </summary>
    public AlpahColorExtension(object resourceKey)
    {
        if (resourceKey == null) {
            throw new ArgumentNullException("resourceKey");
        }
        _resourceKey = resourceKey;
    }


    /// <summary>
    ///  Return an object that should be set on the targetObject's targetProperty
    ///  for this markup extension.  For DynamicResourceExtension, this is the object found in
    ///  a resource dictionary in the current parent chain that is keyed by ResourceKey
    /// </summary>
    /// <returns>
    ///  The object to set on this property.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (ResourceKey == null) {
            throw new InvalidOperationException("ResourceKey is empty");
        }

        var o = Application.Current.FindResource(_resourceKey);
        if (o == null) throw new ResourceReferenceKeyNotFoundException();


        if (o is SolidColorBrush brush) {
            Color c = new Color();
            c.R = brush.Color.R;
            c.G = brush.Color.G;
            c.B = brush.Color.B;
            if (Alpha < 0 || Alpha > 255) {
                c.A = brush.Color.A;
            }
            else c.A = (byte)Alpha;
            return c;
        }
        else if (o is Color c) {
            if (Alpha >= 0 && Alpha <= 255) {
                c.A = (byte)Alpha;
            }
            return c;
        }

        throw new InvalidCastException("Can't convert resource to Color");
    }

    /// <summary>
    ///  The key in a Resource Dictionary used to find the object refered to by this
    ///  Markup Extension.
    /// </summary>
    [ConstructorArgument("resourceKey")] // Uses an instance descriptor
    public object? ResourceKey
    {
        get { return _resourceKey; }
        set {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
            _resourceKey = value;
        }
    }

    private object? _resourceKey;


}
