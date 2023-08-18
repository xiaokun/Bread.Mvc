using System;
using System.Windows;

namespace Bread.Mvc.WPF
{
    public class Button : System.Windows.Controls.Button
    {
        public CornerRadius CornerRadius
        {
            set { SetValue(CornerRadiusProperty, value); }
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
           DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Button));

        public FrameworkElement CheckedContent
        {
            set { SetValue(CheckedContentProperty, value); }
            get {
                var content = (FrameworkElement)GetValue(CheckedContentProperty);
                if (content != null) return content;
                return (FrameworkElement)GetValue(ContentProperty);
            }
        }
        public static readonly DependencyProperty CheckedContentProperty =
            DependencyProperty.Register("CheckedContent", typeof(FrameworkElement), typeof(Button));


        public bool IsChecked
        {
            set { SetValue(IsCheckedProperty, value); }
            get { return (bool)GetValue(IsCheckedProperty); }
        }
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(Button), 
                new PropertyMetadata(false, OnCheckedChanged));


        public bool IsAutoSelect
        {
            set { SetValue(IsAutSelectProperty, value); }
            get { return (bool)GetValue(IsAutSelectProperty); }
        }
        public static readonly DependencyProperty IsAutSelectProperty =
            DependencyProperty.Register("IsAutSelect", typeof(bool), typeof(Button), 
                new PropertyMetadata(false));

        public event Action? Checked;

        static Button()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));
        }

        public Button()
        {
            this.Click += Button_Click;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsAutoSelect) {
                IsChecked = !IsChecked;
            }
        }

        private void InvokeEvent()
        {
            Checked?.Invoke();
        }

        private static void OnCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue) return;

            if (d is Button btn && btn != null) {
                btn.InvokeEvent();
            }
        }
    }
}
