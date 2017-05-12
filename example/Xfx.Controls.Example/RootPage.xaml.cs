using System;
using Xamarin.Forms;

namespace Xfx.Controls.Example
{
    public partial class RootPage
    {
        public RootPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this,false);
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }
    }
}
