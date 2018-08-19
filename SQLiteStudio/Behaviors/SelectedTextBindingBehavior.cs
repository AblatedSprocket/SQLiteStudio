using System.Windows;
using System.Windows.Controls;

namespace SQLiteStudio.Behaviors
{
    public class SelectedTextBindingBehavior
    {
        #region Dependency Properties
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof(string), typeof(SelectedTextBindingBehavior), new PropertyMetadata(null, OnPasswordChanged));
        public static readonly DependencyProperty IsClearProperty = DependencyProperty.RegisterAttached("IsClear", typeof(bool), typeof(SelectedTextBindingBehavior), new PropertyMetadata(false, OnIsClearToggled));
        public static readonly DependencyProperty IsBoundProperty = DependencyProperty.RegisterAttached("IsBound", typeof(bool), typeof(SelectedTextBindingBehavior), new PropertyMetadata(false, OnIsBoundChanged));
        private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(SelectedTextBindingBehavior), new PropertyMetadata(false));
        #endregion
        #region Static Getters and Setters
        public static string GetPassword(DependencyObject source)
        {
            return (string)source.GetValue(PasswordProperty);
        }
        public static void SetPassword(DependencyObject target, string value)
        {
            target.SetValue(PasswordProperty, value);
        }
        public static bool GetIsBound(DependencyObject source)
        {
            return (bool)source.GetValue(IsBoundProperty);
        }
        public static void SetIsBound(DependencyObject target, bool value)
        {
            target.SetValue(IsBoundProperty, value);
        }
        public static bool GetIsClear(DependencyObject source)
        {
            return (bool)source.GetValue(IsClearProperty);
        }
        public static void SetIsClear(DependencyObject target, bool value)
        {
            target.SetValue(IsClearProperty, value);
        }
        private static bool GetIsUpdating(DependencyObject source)
        {
            return (bool)source.GetValue(IsUpdatingProperty);
        }
        private static void SetIsUpdating(DependencyObject target, bool value)
        {
            target.SetValue(IsUpdatingProperty, value);
        }
        #endregion
        #region Event Handlers
        private static void OnPasswordChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && GetIsBound(sender))
            {
                passwordBox.PasswordChanged -= HandlePasswordChanged;
                string password = (string)e.NewValue;
                if (!GetIsUpdating(passwordBox))
                {
                    passwordBox.Password = (string)e.NewValue;
                }
                passwordBox.PasswordChanged += HandlePasswordChanged;
            }
        }
        private static void OnIsClearToggled(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool && (!(e.OldValue is bool) || (bool)e.NewValue != (bool)e.OldValue))
            {
                PasswordBox passwordBox = sender as PasswordBox;
                if (passwordBox != null)
                {
                    passwordBox.Clear();
                }
            }
        }
        private static void OnIsBoundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && GetIsBound(sender))
            {
                bool wasBound = (bool)e.OldValue;
                bool needsToBind = (bool)e.NewValue;
                if (wasBound)
                {
                    passwordBox.PasswordChanged -= HandlePasswordChanged;
                }
                if (needsToBind)
                {
                    passwordBox.PasswordChanged += HandlePasswordChanged;
                }
            }
        }
        #endregion
        #region Private Methods
        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetIsUpdating(passwordBox, true);
                SetPassword(passwordBox, passwordBox.Password);
                SetIsUpdating(passwordBox, false);
            }
        }
        #endregion
    }
}
