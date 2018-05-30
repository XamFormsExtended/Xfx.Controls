using Xamarin.Forms;

namespace Xfx
{
    public class XfxSelectedItemChangedEventArgs : SelectedItemChangedEventArgs
    {
        public int SelectedItemIndex { get; private set; }

        public XfxSelectedItemChangedEventArgs(object selectedItem, int selectedIndex) : base(selectedItem)
        {
            SelectedItemIndex = selectedIndex;
        }
    }
}
