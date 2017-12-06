using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Graphics.Drawable;
using Android.Text;
using Android.Text.Method;
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
using Xfx.Extensions;
using Application = Android.App.Application;
using Color = Xamarin.Forms.Color;
using AColor = Android.Graphics.Color;
using FormsAppCompat = Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(XfxEntry), typeof(XfxEntryRendererDroid))]

namespace Xfx.Controls.Droid.Renderers
{
    public class XfxEntryRendererDroid : FormsAppCompat.ViewRenderer<XfxEntry, TextInputLayout>,
        ITextWatcher,
        TextView.IOnEditorActionListener
    {
        private bool _hasFocus;

        public XfxEntryRendererDroid(Context context) : base(context)
        {
            AutoPackage = false;
        }

        protected EditText EditText => Control.EditText;

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            if ((actionId == ImeAction.Done) || ((actionId == ImeAction.ImeNull) && (e.KeyCode == Keycode.Enter)))
            {
                Control.ClearFocus();
                HideKeyboard();
                ((IEntryController)Element).SendCompleted();
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
            if (string.IsNullOrWhiteSpace(Element.Text) && (s.Length() == 0)) return;
            ((IElementController)Element).SetValueFromRenderer(Entry.TextProperty, s.ToString());
        }

        protected override TextInputLayout CreateNativeControl()
        {
            var textInputLayout = new TextInputLayout(Context);
            var editText = new TextInputEditText(Context);
            textInputLayout.AddView(editText);
            return textInputLayout;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XfxEntry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
                if (Control != null)
                    EditText.FocusChange -= ControlOnFocusChange;

            if (e.NewElement != null)
            {
                var ctrl = CreateNativeControl();
                SetNativeControl(ctrl);

                if (!string.IsNullOrWhiteSpace(Element.AutomationId))
                    EditText.ContentDescription = Element.AutomationId;

                //_defaultHintColor = EditText.HintTextColors;
                //_defaultTextColor = EditText.TextColors;
                //_defaultBackgroundColor = EditText.BackgroundTintList;

                Focusable = true;
                Control.HintEnabled = true;
                Control.HintAnimationEnabled = true;
                EditText.ShowSoftInputOnFocus = true;

                // Subscribe
                EditText.FocusChange += ControlOnFocusChange;
                EditText.AddTextChangedListener(this);
                EditText.SetOnEditorActionListener(this);
                EditText.ImeOptions = ImeAction.Done;

                SetLabelAndUnderlineColor();
                SetText();
                SetInputType();
                SetHintText();
                //SetTextColor();
                SetHorizontalTextAlignment();
                SetErrorText();
                SetFloatingHintEnabled();
                SetIsEnabled();
                SetErrorDisplay();
                SetFont();
                SetLabelAndUnderlineColor();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Entry.PlaceholderProperty.PropertyName)
                SetHintText();
            else if (e.PropertyName == XfxEntry.ErrorTextProperty.PropertyName)
                SetErrorText();
            else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName ||
                e.PropertyName == InputView.KeyboardProperty.PropertyName)
                SetInputType();
            else if (e.PropertyName == Entry.TextProperty.PropertyName)
                SetText();
            else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
                SetHorizontalTextAlignment();
            else if (e.PropertyName == XfxEntry.FloatingHintEnabledProperty.PropertyName)
                SetFloatingHintEnabled();
            else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                SetIsEnabled();
            else if ((e.PropertyName == Entry.FontAttributesProperty.PropertyName) ||
                     (e.PropertyName == Entry.FontFamilyProperty.PropertyName) ||
                     (e.PropertyName == Entry.FontSizeProperty.PropertyName))
                SetFont();
            else if (e.PropertyName == XfxEntry.ActivePlaceholderColorProperty.PropertyName ||
                     e.PropertyName == Entry.PlaceholderColorProperty.PropertyName ||
                     e.PropertyName == Entry.TextColorProperty.PropertyName)
                SetLabelAndUnderlineColor();
        }

        private void ControlOnFocusChange(object sender, FocusChangeEventArgs args)
        {
            _hasFocus = args.HasFocus;

            var defaultColor = GetPlaceholderColor();
            var activeColor = GetActivePlaceholderColor();
            if (args.HasFocus)
            {
                var manager = (InputMethodManager)Application.Context.GetSystemService(Context.InputMethodService);

                EditText.PostDelayed(() =>
                    {
                        EditText.RequestFocus();
                        manager.ShowSoftInput(EditText, 0);
                    },
                    100);
            }

            var isFocusedPropertyKey = Element.GetInternalField<BindablePropertyKey>("IsFocusedPropertyKey");
            ((IElementController)Element).SetValueFromRenderer(isFocusedPropertyKey, args.HasFocus);
            SetUnderlineColor(_hasFocus ? defaultColor : activeColor);
        }

        private AColor GetPlaceholderColor() => Element.PlaceholderColor.ToAndroid(Color.FromHex("#80000000"));

        private AColor GetActivePlaceholderColor() => Element.ActivePlaceholderColor.ToAndroid(global::Android.Resource.Attribute.ColorAccent, Context);

        protected virtual void SetLabelAndUnderlineColor()
        {
            var defaultColor = GetPlaceholderColor();
            var activeColor = GetActivePlaceholderColor();
            
            SetHintLabelDefaultColor(defaultColor);
            SetHintLabelActiveColor(activeColor);
            SetUnderlineColor(_hasFocus ? activeColor : defaultColor);

            //By Setting the value as 0, the color of the cursor will be same as the text color.
            var intPtrtextViewClass = JNIEnv.FindClass(typeof(EditText));
            var mCursorDrawableResProperty = JNIEnv.GetFieldID(intPtrtextViewClass, "mCursorDrawableRes", "I");
            JNIEnv.SetField(EditText.Handle, mCursorDrawableResProperty, 0);
        }

        private void SetUnderlineColor(AColor color)
        {
            var background = EditText.Background; // get current EditText drawable 
            background.SetColorFilter(color, PorterDuff.Mode.SrcAtop); // change the drawable color
            DrawableCompat.SetTint(background, color);
            EditText.SetBackground(background); // set the new drawable to EditText
        }

        private void SetHintLabelActiveColor(AColor activeColor)
        {
            var hintText = Control.Class.GetDeclaredField("mFocusedTextColor");
            hintText.Accessible = true;
            hintText.Set(Control, new ColorStateList(new int[][] { new[] { 0 } }, new int[] { activeColor }));
        }

        private void SetHintLabelDefaultColor(AColor inactiveColor)
        {
            var defaultColor = string.IsNullOrEmpty(EditText.Text) ? Color.FromHex("80000000") : Color.Black;
            var hint = Control.Class.GetDeclaredField("mDefaultTextColor");
            hint.Accessible = true;
            hint.Set(Control, new ColorStateList(new int[][] { new[] { 0 } }, new int[] { inactiveColor }));
        }

        private void SetText()
        {
            if (EditText.Text != Element.Text)
            {
                EditText.Text = Element.Text;
                if (EditText.IsFocused)
                    EditText.SetSelection(EditText.Text.Length);
            }
        }

        private void SetHintText()
        {
            Control.Hint = Element.Placeholder;
        }

        //private void SetHintTextColor()
        //{
        //    //if (Element.PlaceholderColor == Color.Default)
        //    //    EditText.SetHintTextColor(_defaultHintColor);
        //    //else
        //    //    EditText.SetHintTextColor(Element.PlaceholderColor.ToAndroid());
        //}

        //private void SetTextColor()
        //{
        //    if (Element.TextColor == Color.Default)
        //        EditText.SetTextColor(_defaultTextColor);
        //    else
        //        EditText.SetTextColor(Element.TextColor.ToAndroid());
        //}

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

        public void SetFloatingHintEnabled()
        {
            Control.HintEnabled = Element.FloatingHintEnabled;
        }

        public void SetErrorDisplay()
        {
            switch (Element.ErrorDisplay)
            {
                case ErrorDisplay.None:
                    Control.ErrorEnabled = false;
                    break;
                case ErrorDisplay.Underline:
                    Control.ErrorEnabled = true;
                    break;
            }
        }

        protected void HideKeyboard()
        {
            var manager = (InputMethodManager)Application.Context.GetSystemService(Context.InputMethodService);
            manager.HideSoftInputFromWindow(EditText.WindowToken, 0);
        }

        private void SetFont()
        {
            var tf = Element.ToTypeface();
            EditText.Typeface = Control.Typeface = tf;
            EditText.SetTextSize(ComplexUnitType.Sp, (float)Element.FontSize);
        }

        private void SetErrorText()
        {
            if (!string.IsNullOrEmpty(Element.ErrorText))
            {
                Control.Error = Element.ErrorText;
            }
            else
            {
                Control.Error = null;
            }
        }

        private void SetIsEnabled()
        {
            EditText.Enabled = Element.IsEnabled;
        }

        private void SetInputType()
        {
            EditText.InputType = Element.Keyboard.ToInputType();
            if (Element.IsPassword && (EditText.InputType & InputTypes.ClassText) == InputTypes.ClassText)
            {
                EditText.TransformationMethod = new PasswordTransformationMethod();
                EditText.InputType = EditText.InputType | InputTypes.TextVariationPassword;
            }
            if (Element.IsPassword && (EditText.InputType & InputTypes.ClassNumber) == InputTypes.ClassNumber)
            {
                EditText.TransformationMethod = new PasswordTransformationMethod();
                EditText.InputType = EditText.InputType | InputTypes.NumberVariationPassword;
            }
        }
    }
}