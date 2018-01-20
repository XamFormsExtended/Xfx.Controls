using System;
using Xamarin.Forms;

namespace Xfx
{
    public class XfxPicker : Picker, IXfxValidatableElement
    {
        public static readonly BindableProperty LabelColorProperty = BindableProperty.Create(nameof(LabelColor),
            typeof(Color),
            typeof(XfxPicker),
            Color.Default);

        public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText),
            typeof(string),
            typeof(XfxPicker),
            default(string),
            propertyChanged: RaiseTextChangedInternal);

        public static readonly BindableProperty ActiveLabelColorProperty = BindableProperty.Create(nameof(ActiveLabelColor),
            typeof(Color),
            typeof(XfxPicker),
            Color.Accent);

        public static readonly BindableProperty FloatingHintEnabledProperty = BindableProperty.Create(nameof(FloatingHintEnabled),
            typeof(bool),
            typeof(XfxPicker),
            true);

        /// <summary>
        /// FloatingHintEnabled summary. This is a bindable property.
        /// </summary>
        public bool FloatingHintEnabled
        {
            get => (bool) GetValue(FloatingHintEnabledProperty);
            set => SetValue(FloatingHintEnabledProperty, value);
        }

        /// <summary>
        /// ActiveLabelColor summary. This is a bindable property.
        /// </summary>
        public Color ActiveLabelColor
        {
            get => (Color) GetValue(ActiveLabelColorProperty);
            set => SetValue(ActiveLabelColorProperty, value);
        }

        /// <summary>
        ///     LabelColor summary. This is a bindable property.
        /// </summary>
        public Color LabelColor
        {
            get => (Color) GetValue(LabelColorProperty);
            set => SetValue(LabelColorProperty, value);
        }

        /// <summary>
        ///     ErrorText summary. This is a bindable property.
        /// </summary>
        public string ErrorText
        {
            get => (string) GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }

        public event EventHandler<TextChangedEventArgs> ErrorTextChanged;

        private static void RaiseTextChangedInternal(BindableObject bindable, object oldvalue, object newvalue)
        {
            var materialPicker = (XfxPicker) bindable;
            materialPicker.OnErrorTextChanged(bindable, oldvalue, newvalue);
            materialPicker.ErrorTextChanged?.Invoke(materialPicker,
                new TextChangedEventArgs((string) oldvalue, (string) newvalue));
        }

        protected virtual void OnErrorTextChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
        }
    }
}