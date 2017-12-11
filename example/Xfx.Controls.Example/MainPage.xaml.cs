using System;
using Xamarin.Forms;

namespace Xfx.Controls.Example
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this,false);
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Features.Controls.ControlsPage());
        }
    }
}
