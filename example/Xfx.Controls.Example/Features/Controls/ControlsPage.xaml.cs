using System.Diagnostics;
using Xamarin.Forms;

namespace Xfx.Controls.Example.Features.Controls
{
    public partial class ControlsPage
    {
        public ControlsPage()
        {
            InitializeComponent();
        }

        private void Email_OnFocused(object sender, FocusEventArgs e)
        {
            Debug.WriteLine("Email Focused");
        }

        private void Email_OnUnfocused(object sender, FocusEventArgs e)
        {
            Debug.WriteLine("Email Unfocused");
        }

        private void Email_ItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            Debug.WriteLine($"Selected Item from Event: {args.SelectedItem}");
        }
    }
}