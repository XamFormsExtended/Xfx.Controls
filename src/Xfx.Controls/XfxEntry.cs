using System;
using Xamarin.Forms;

namespace Xfx
{
    public class XfxEntry : Entry, IXfxValidatableElement
    {
        public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText),
            typeof(string),
            typeof(XfxEntry),
            default(string), propertyChanged: OnErrorTextChangedInternal);

        public static readonly BindableProperty ActivePlaceholderColorProperty = BindableProperty.Create(nameof(ActivePlaceholderColor),
            typeof(Color),
            typeof(XfxEntry),
            Color.Accent);
        
        public static readonly BindableProperty FloatingHintEnabledProperty = BindableProperty.Create(nameof(FloatingHintEnabled),
            typeof(bool),
            typeof(XfxEntry),
            true);

        /// <summary>
        ///     ActivePlaceholderColor summary. This is a bindable property.
        /// </summary>
        public Color ActivePlaceholderColor
        {
            get => (Color) GetValue(ActivePlaceholderColorProperty);
            set => SetValue(ActivePlaceholderColorProperty, value);
        }

        /// <summary>
        ///     Gets or Sets whether or not the Error Style is 'Underline' or 'None'
        /// </summary>
        public ErrorDisplay ErrorDisplay { get; set; } = ErrorDisplay.Underline;

        /// <summary>
        ///     <c>true</c> to float the hint into a label, otherwise <c>false</c>. This is a bindable property.
        /// </summary>
        public bool FloatingHintEnabled
        {
            get => (bool) GetValue(FloatingHintEnabledProperty);
            set => SetValue(FloatingHintEnabledProperty, value);
        }

        /// <summary>
        ///     Error text for the entry. An empty string removes the error. This is a bindable property.
        /// </summary>
        public string ErrorText
        {
            get => (string) GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }

        /// <summary>
        ///     Raised when the value of the error text changes
        /// </summary>
        public event EventHandler<TextChangedEventArgs> ErrorTextChanged;

        protected virtual void OnErrorTextChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
        }

        private static void OnErrorTextChangedInternal(BindableObject bindable, object oldvalue, object newvalue)
        {
            var materialEntry = (XfxEntry) bindable;
            materialEntry.OnErrorTextChanged(bindable, oldvalue, newvalue);
            materialEntry.ErrorTextChanged?.Invoke(materialEntry, new TextChangedEventArgs((string) oldvalue, (string) newvalue));
        }
    }

    public enum ErrorDisplay
    {
        Underline,
        None
    }
}