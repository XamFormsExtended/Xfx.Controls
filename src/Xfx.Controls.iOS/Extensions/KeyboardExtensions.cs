using UIKit;
using Xamarin.Forms;

namespace Xfx.Controls.iOS.Extensions
{
    public static class KeyboardExtensions
    {
        public static UIKeyboardType ToNative(this Keyboard input)
        {
            if (input == Keyboard.Chat)
            {
                return UIKeyboardType.Twitter;
            }
            if (input == Keyboard.Text)
            {
                return UIKeyboardType.ASCIICapable;
            }
            if (input == Keyboard.Numeric)
            {
                return UIKeyboardType.NumberPad;
            }
            if (input == Keyboard.Telephone)
            {
                return UIKeyboardType.PhonePad;
            }
            if (input == Keyboard.Url)
            {
                return UIKeyboardType.Url;
            }
            if (input == Keyboard.Email)
            {
                return UIKeyboardType.EmailAddress;
            }
            return UIKeyboardType.Default;
        }
    }
}