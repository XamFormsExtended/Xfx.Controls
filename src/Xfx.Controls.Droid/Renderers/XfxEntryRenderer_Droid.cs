using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xfx;
using Xfx.Controls.Droid.Extensions;
using Xfx.Controls.Droid.Renderers;
using Application = Android.App.Application;
using FormsAppCompat = Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof (XfxEntry), typeof (XfxEntryRenderer_Droid))]

namespace Xfx.Controls.Droid.Renderers
{
    public class XfxEntryRenderer_Droid : FormsAppCompat.ViewRenderer<XfxEntry, TextInputLayout>,
        ITextWatcher,
        TextView.IOnEditorActionListener
    {
        private ColorStateList _defaultHintColor;
        private ColorStateList _defaultTextColor;

        public XfxEntryRenderer_Droid()
        {
            AutoPackage = false;
        }

        private EditText EditText
        {
            get { return Control.EditText; }
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.Done || actionId == ImeAction.ImeNull && e.KeyCode == Keycode.Enter)
            {
                Control.ClearFocus();
                HideKeyboard();
                ((IEntryController) Element).SendCompleted();
            }
            return true;
        }

        public virtual void AfterTextChanged(IEditable s)
        {
        }

        public virtual void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public virtual void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            if (string.IsNullOrWhiteSpace(Element.Text) && s.Length() == 0) return;
            ((IElementController) Element).SetValueFromRenderer(Entry.TextProperty, s.ToString());
        }

        protected void SetDefaultHintColorStateList(ColorStateList defaultHint)
        {
            _defaultHintColor = defaultHint;
        }

        protected void SetDefaultTextColorStateList(ColorStateList defaultText)
        {
            _defaultTextColor = defaultText;
        }

        protected override TextInputLayout CreateNativeControl()
        {
            var layout = (TextInputLayout) LayoutInflater.From(Context).Inflate(Resource.Layout.TextInputLayout, null);
            if (!string.IsNullOrWhiteSpace(Element.AutomationId))
            {
                layout.EditText.ContentDescription = Element.AutomationId;
            }

            SetDefaultHintColorStateList(layout.EditText.HintTextColors);
            SetDefaultTextColorStateList(layout.EditText.TextColors);
            return layout;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XfxEntry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // unsubscribe
                EditText.FocusChange -= ControlOnFocusChange;
            }

            if (e.NewElement != null)
            {
                var ctrl = CreateNativeControl();
                SetNativeControl(ctrl);

                SetIsPassword();
                SetText();
                SetHintText();
                SetTextColor();
                SetPlaceholderColor();
                SetKeyboard();
                SetHorizontalTextAlignment();
                SetErrorText();
                SetFont();

                Focusable = true;
                Control.ErrorEnabled = true;
                EditText.ShowSoftInputOnFocus = true;

                // Subscribe
                EditText.FocusChange += ControlOnFocusChange;
                EditText.AddTextChangedListener(this);
                EditText.SetOnEditorActionListener(this);
                EditText.ImeOptions = ImeAction.Done;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == XfxEntry.ErrorTextProperty.PropertyName)
            {
                SetErrorText();
            }

            if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
            {
                SetIsPassword();
            }

            if (e.PropertyName == Entry.TextProperty.PropertyName)
            {
                SetText();
            }

            if (e.PropertyName == Entry.TextColorProperty.PropertyName)
            {
                SetTextColor();
            }

            if (e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
            {
                SetPlaceholderColor();
            }

            if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
            {
                SetKeyboard();
            }

            if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
            {
                SetHorizontalTextAlignment();
            }

            if (e.PropertyName == Entry.FontAttributesProperty.PropertyName ||
                e.PropertyName == Entry.FontFamilyProperty.PropertyName ||
                e.PropertyName == Entry.FontSizeProperty.PropertyName)
            {
                SetFont();
            }
        }

        private void ControlOnFocusChange(object sender, FocusChangeEventArgs args)
        {
            if (args.HasFocus)
            {
                var manager = (InputMethodManager) Application.Context.GetSystemService(Context.InputMethodService);

                EditText.PostDelayed(() =>
                {
                    EditText.RequestFocus();
                    manager.ShowSoftInput(EditText, 0);
                },
                    100);
                // TODO : Florell, Chase (Contractor) 02/21/17 focus
            }
        }

        private void SetText()
        {
            if (EditText.Text != Element.Text)
            {
                EditText.Text = Element.Text;
                if (EditText.IsFocused)
                {
                    EditText.SetSelection(EditText.Text.Length);
                }
            }
        }

        private void SetHintText()
        {
            Control.Hint = Element.Placeholder;
        }

        private void SetPlaceholderColor()
        {
            if (Element.PlaceholderColor == Color.Default)
            {
                EditText.SetHintTextColor(_defaultHintColor);
            }
            else
            {
                EditText.SetHintTextColor(Element.PlaceholderColor.ToAndroid());
            }
        }

        private void SetTextColor()
        {
            if (Element.TextColor == Color.Default)
            {
                EditText.SetTextColor(_defaultTextColor);
            }
            else
            {
                EditText.SetTextColor(Element.TextColor.ToAndroid());
            }
        }

        private void SetKeyboard()
        {
            EditText.InputType = Element.Keyboard.ToNative();
        }

        private void SetHorizontalTextAlignment()
        {
            switch (Element.HorizontalTextAlignment)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    EditText.Gravity = GravityFlags.CenterHorizontal;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    EditText.Gravity = GravityFlags.Right;
                    break;
                default:
                    EditText.Gravity = GravityFlags.Left;
                    break;
            }
        }

        protected void HideKeyboard()
        {
            var manager = (InputMethodManager) Application.Context.GetSystemService(Context.InputMethodService);
            manager.HideSoftInputFromWindow(EditText.WindowToken, 0);
        }

        private void SetFont()
        {
            Control.Typeface = Element.ToTypeface();
            EditText.SetTextSize(ComplexUnitType.Sp, (float) Element.FontSize);
        }

        private void SetErrorText()
        {
            if (!string.IsNullOrEmpty(Element.ErrorText))
            {
                Control.ErrorEnabled = true;
                Control.Error = Element.ErrorText;
            }
            else
            {
                Control.Error = null;
                Control.ErrorEnabled = false;
            }
        }

        private void SetIsPassword()
        {
            EditText.InputType = Element.IsPassword
                ? InputTypes.TextVariationPassword | InputTypes.ClassText
                : EditText.InputType;
        }
    }
}