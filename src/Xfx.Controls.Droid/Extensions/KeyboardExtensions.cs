using Android.Text;
using Xamarin.Forms;

namespace Xfx.Controls.Droid.Extensions
{
    public static class KeyboardExtensions
    {
        public static InputTypes ToNative(this Keyboard input)
        {
            if (input == Keyboard.Url)
            {
                return InputTypes.ClassText | InputTypes.TextVariationUri;
            }
            if (input == Keyboard.Email)
            {
                return InputTypes.ClassText | InputTypes.TextVariationEmailAddress;
            }
            if (input == Keyboard.Numeric)
            {
                return InputTypes.ClassNumber;
            }
            if (input == Keyboard.Chat)
            {
                return InputTypes.ClassText | InputTypes.TextVariationShortMessage;
            }
            if (input == Keyboard.Telephone)
            {
                return InputTypes.ClassPhone;
            }
            if (input == Keyboard.Text)
            {
                return InputTypes.ClassText | InputTypes.TextFlagNoSuggestions;
            }
            return InputTypes.ClassText;
        }
    }
}