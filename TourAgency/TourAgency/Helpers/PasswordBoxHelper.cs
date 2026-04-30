using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TourAgency.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static readonly DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        public static string GetBoundPassword(DependencyObject d) => (string)d.GetValue(BoundPasswordProperty);
        public static void SetBoundPassword(DependencyObject d, string value) => d.SetValue(BoundPasswordProperty, value);

        public static bool GetBindPassword(DependencyObject d) => (bool)d.GetValue(BindPasswordProperty);
        public static void SetBindPassword(DependencyObject d, bool value) => d.SetValue(BindPasswordProperty, value);
        private static void OnBindPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                bool wasBound = (bool)e.OldValue;
                bool needToBind = (bool)e.NewValue;

                if (wasBound)
                {
                    passwordBox.PasswordChanged -= HandlePasswordChanged;
                }

                if (needToBind)
                {
                    passwordBox.PasswordChanged += HandlePasswordChanged;
                }
            }
        }
        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            SetIsUpdating(passwordBox, true);

            SetBoundPassword(passwordBox, passwordBox.Password);

            SetIsUpdating(passwordBox, false);
        }
        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = d as PasswordBox;
            if (d != null && !GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
        }
        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordBoxHelper));

        private static bool GetIsUpdating(DependencyObject d) => (bool)d.GetValue(IsUpdatingProperty);
        private static void SetIsUpdating(DependencyObject d, bool value) => d.SetValue(IsUpdatingProperty, value);
    }
}
