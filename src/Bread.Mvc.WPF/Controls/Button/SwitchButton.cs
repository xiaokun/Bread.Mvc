using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Control = System.Windows.Controls.Control;

namespace Bread.Mvc.WPF
{
    [TemplatePart(Name = PART_ToggleButton_Name, Type = typeof(ToggleButton))]
    public class SwitchButton : Control
    {
        static SwitchButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchButton), new FrameworkPropertyMetadata(typeof(SwitchButton)));
        }

        public event Action? OnSwitched;

        public bool? IsChecked
        {
            get {
                if (uiToggleButton == null) return null;
                return uiToggleButton.IsChecked;
            }
            set {
                if (uiToggleButton == null) return;
                uiToggleButton.IsChecked = value;
            }
        }

        private const string PART_ToggleButton_Name = "PART_ToggleButton";
        private ToggleButton? uiToggleButton = null;

        public SwitchButton()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (uiToggleButton != null) {
                uiToggleButton.Checked -= UiToggleButton_Checked;
                uiToggleButton.Unchecked -= UiToggleButton_Unchecked;
            }

            var btn = Template.FindName(PART_ToggleButton_Name, this) as ToggleButton;
            if (btn == null) return;
            uiToggleButton = btn;
            uiToggleButton.Checked += UiToggleButton_Checked;
            uiToggleButton.Unchecked += UiToggleButton_Unchecked;
        }

        private void UiToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            OnSwitched?.Invoke();
        }

        private void UiToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            OnSwitched?.Invoke();
        }
    }
}
