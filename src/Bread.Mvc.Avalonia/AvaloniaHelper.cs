using Avalonia;

namespace Bread.Mvc.Avalonia;

public static class AvaloniaHelper
{
    public static T? FindAncestor<T>(this StyledElement control) where T : AvaloniaObject
    {
        var target = control;

        while (target != null) {
            if (target is T t) {
                return t;
            }

            if (target.Parent == null) {
                return null;
            }

            target = target.Parent;
        }

        return null;
    }

    public static bool HasAncestor(this StyledElement control, StyledElement parent)
    {
        var target = control;

        while (target != null) {
            if (target == control) {
                return true;
            }

            if (target.Parent == null) {
                return false;
            }

            target = target.Parent;
        }

        return false;
    }
}
