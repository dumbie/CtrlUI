using System.Windows;
using System.Windows.Controls;

namespace CtrlUI.Styles
{
    //Import:
    //xmlns:styles="clr-namespace:CtrlUI.Styles"
    //Usage:
    //<Textbox styles:TextboxPlaceholder.Placeholder="Hello"/>
    public static class TextboxPlaceholder
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(TextboxPlaceholder), new PropertyMetadata(default(string), PlaceholderChanged));

        private static void PlaceholderChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            TextBox textbox = dependencyObject as TextBox;
            if (textbox == null) { return; }

            textbox.LostFocus -= OnLostFocus;
            textbox.GotFocus -= OnGotFocus;

            if (args.NewValue != null)
            {
                textbox.GotFocus += OnGotFocus;
                textbox.LostFocus += OnLostFocus;
            }
        }

        private static void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            TextBox textbox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textbox.Text))
            {
                textbox.Text = GetPlaceholder(textbox);
            }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            TextBox textbox = sender as TextBox;
            string placeholderText = GetPlaceholder(textbox);
            if (textbox.Text == placeholderText)
            {
                textbox.Text = string.Empty;
            }
        }

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static void SetPlaceholder(DependencyObject element, string value)
        {
            element.SetValue(PlaceholderProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static string GetPlaceholder(DependencyObject element)
        {
            return (string)element.GetValue(PlaceholderProperty);
        }
    }
}