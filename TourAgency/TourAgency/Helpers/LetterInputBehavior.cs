using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace TourAgency.Helpers
{
    public class LetterInputBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            DataObject.AddPastingHandler(AssociatedObject, OnPasting);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            DataObject.RemovePastingHandler(AssociatedObject, OnPasting);
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
       
            Regex regex = new Regex(@"^[А-Яа-яЁёЇїІіЄєҐґ'A-Za-z -]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                Regex regex = new Regex(@"^[А-Яа-яЁёЇїІіЄєҐґ'A-Za-z -]+$");
                if (!regex.IsMatch(text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }
    }
}