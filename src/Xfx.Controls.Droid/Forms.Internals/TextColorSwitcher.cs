using Android.Content.Res;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Xfx.Controls.Droid.Forms.Internals
{
    /// <summary>
    /// Handles color state management for the TextColor property 
    /// for Button, Picker, TimePicker, and DatePicker
    /// </summary>
    internal class TextColorSwitcher
    {
        static readonly int[][] _colorStates = { new[] { global::Android.Resource.Attribute.StateEnabled }, new[] { -global::Android.Resource.Attribute.StateEnabled } };

        readonly ColorStateList _defaultTextColors;
        Color _currentTextColor;

        public TextColorSwitcher(ColorStateList textColors)
        {
            _defaultTextColors = textColors;
        }

        public void UpdateTextColor(TextView control, Color color)
        {
            if (color == _currentTextColor)
                return;

            _currentTextColor = color;

            if (color == Color.Default)
                control.SetTextColor(_defaultTextColors);
            else
            {
                // Set the new enabled state color, preserving the default disabled state color
                int disabledColor = color.MultiplyAlpha(0.66).ToAndroid();
                control.SetTextColor(new ColorStateList(_colorStates, new[] { color.ToAndroid().ToArgb(), disabledColor }));
            }
        }

    }
}