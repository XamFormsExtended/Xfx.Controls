using System;

namespace Xfx.Controls.Example
{
    public partial class RootPage
    {
        public RootPage()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }
    }
}
