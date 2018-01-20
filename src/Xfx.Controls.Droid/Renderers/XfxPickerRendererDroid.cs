using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xfx.Controls.Droid.Forms.Internals;
using Xfx.Extensions;
using FormsAppCompat = Xamarin.Forms.Platform.Android.AppCompat;
using Object = Java.Lang.Object;
using Orientation = Android.Widget.Orientation;
using AView = Android.Views.View;
using AColor = Android.Graphics.Color;

namespace Xfx.Controls.Droid.Renderers
{
    public class XfxPickerRendererDroid : FormsAppCompat.ViewRenderer<XfxPicker, TextInputLayout>
    {
        private ColorStateList _defaultHintColor;
        private AlertDialog _dialog;
        private bool _isDisposed;
        private TextColorSwitcher _textColorSwitcher;

        public XfxPickerRendererDroid(Context context) : base(context)
        {
            AutoPackage = false;
        }

        private EditText EditText => Control.EditText;

        private void RowsCollectionChanged(object sender, EventArgs e)
        {
            UpdatePicker();
        }

        protected override TextInputLayout CreateNativeControl()
        {
            var layout = new TextInputLayout(Context)
            {
                Focusable = false,
                Clickable = false,
                LongClickable = false
            };

            var editText = new AppCompatEditText(Context)
            {
                Focusable = true,
                Clickable = true,
                Tag = this,
                OnFocusChangeListener = PickerListener.Instance,
                LongClickable = false,
                ShowSoftInputOnFocus = false
            };
            
            editText.SetOnClickListener(PickerListener.Instance);
            if (!string.IsNullOrWhiteSpace(Element.AutomationId))
            {
                editText.ContentDescription = Element.AutomationId;
            }

            _defaultHintColor = editText.HintTextColors;
            layout.AddView(editText);
            return layout;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XfxPicker> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                ((INotifyCollectionChanged)e.OldElement.Items).CollectionChanged -= RowsCollectionChanged;
            }

            if (e.NewElement != null)
            {
                ((INotifyCollectionChanged)e.NewElement.Items).CollectionChanged += RowsCollectionChanged;

                var ctrl = CreateNativeControl();
                if (Control == null)
                {
                    _textColorSwitcher = new TextColorSwitcher(ctrl.EditText.TextColors);
                    SetNativeControl(ctrl);
                }

                UpdatePicker();
                SetHintText();
                SetTextColor();
                SetPlaceholderColor();
                SetErrorText();
                SetFloatingHintEnabled();
                SetLabelAndUnderlineColor();

                // Set at the end on purpose
                Control.ErrorEnabled = true;
            }
        }

        protected virtual void SetLabelAndUnderlineColor()
        {
            var defaultColor = GetLabelColor();
            var activeColor = GetActiveLabelColor();

            SetHintLabelDefaultColor(defaultColor);
            SetHintLabelActiveColor(activeColor);
            SetUnderlineColor(Control.IsFocused ? activeColor : defaultColor);
        }

        public void SetFloatingHintEnabled()
        {
            Control.HintEnabled = Element.FloatingHintEnabled;
            Control.HintAnimationEnabled = Element.FloatingHintEnabled;
        }
        
        private void SetHintLabelActiveColor(AColor color)
        {
            var hintText = Control.Class.GetDeclaredField("mFocusedTextColor");
            hintText.Accessible = true;
            hintText.Set(Control, new ColorStateList(new int[][] { new[] { 0 } }, new int[] { color }));
        }

        private void SetUnderlineColor(AColor color)
        {
            var element = (ITintableBackgroundView)EditText;
            element.SupportBackgroundTintList = ColorStateList.ValueOf(color);
        }

        private void SetHintLabelDefaultColor(AColor color)
        {
            var hint = Control.Class.GetDeclaredField("mDefaultTextColor");
            hint.Accessible = true;
            hint.Set(Control, new ColorStateList(new int[][] { new[] { 0 } }, new int[] { color }));
        }
        protected AColor GetLabelColor() => Element.LabelColor.ToAndroid(Color.FromHex("#80000000"));

        private AColor GetActiveLabelColor() => Element.ActiveLabelColor.ToAndroid(global::Android.Resource.Attribute.ColorAccent, Context);
        
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Picker.TitleProperty.PropertyName)
                SetHintText();

            if (e.PropertyName == XfxPicker.ErrorTextProperty.PropertyName)
                SetErrorText();

            if (e.PropertyName == XfxPicker.LabelColorProperty.PropertyName)
                SetPlaceholderColor();

            if (e.PropertyName == Picker.TitleProperty.PropertyName)
                UpdatePicker();

            if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
                UpdatePicker();

            if (e.PropertyName == Picker.TextColorProperty.PropertyName)
                SetTextColor();
        }

        private void SetHintText()
        {
            Control.Hint = Element.Title;
        }

        private void SetPlaceholderColor()
        {
            if (Element.LabelColor == Color.Default)
                EditText.SetHintTextColor(_defaultHintColor);
            else
                EditText.SetHintTextColor(Element.LabelColor.ToAndroid());
        }

        private void SetTextColor()
        {
            _textColorSwitcher?.UpdateTextColor(Control.EditText, Element.TextColor);
        }

        private void SetErrorText()
        {
            System.Diagnostics.Debug.WriteLine($"Error Text: {Element.ErrorText}");
            if (!string.IsNullOrEmpty(Element.ErrorText))
            {
                System.Diagnostics.Debug.WriteLine($"Error Text Enabled");

                Control.ErrorEnabled = true;
                Control.Error = Element.ErrorText;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Error Text Disabled");

                Control.Error = null;
                Control.ErrorEnabled = false;
            }
        }

        private void OnClick()
        {
            var model = Element;
            var isFocusedPropertyKey = Element.GetInternalField<BindablePropertyKey>("IsFocusedPropertyKey");

            var picker = new NumberPicker(Context);
            if (model.Items != null && model.Items.Any())
            {
                picker.MaxValue = model.Items.Count - 1;
                picker.MinValue = 0;
                picker.SetDisplayedValues(model.Items.ToArray());
                picker.WrapSelectorWheel = false;
                picker.DescendantFocusability = DescendantFocusability.BlockDescendants;
                picker.Value = model.SelectedIndex;
            }

            var layout = new LinearLayout(Context) { Orientation = Orientation.Vertical };
            layout.AddView(picker);

            ((IElementController)Element).SetValueFromRenderer(isFocusedPropertyKey, true);

            var builder = new AlertDialog.Builder(Context);
            builder.SetView(layout);
            builder.SetTitle(model.Title ?? "");
            builder.SetNegativeButton(Android.Resource.String.Cancel, (s, a) =>
            {
                ((IElementController)Element).SetValueFromRenderer(isFocusedPropertyKey, false);
                // It is possible for the Content of the Page to be changed when Focus is changed.
                // In this case, we'll lose our Control.
                Control?.ClearFocus();
                _dialog = null;
            });
            builder.SetPositiveButton(Android.Resource.String.Ok, (s, a) =>
            {
                ((IElementController)Element).SetValueFromRenderer(Picker.SelectedIndexProperty, picker.Value);
                // It is possible for the Content of the Page to be changed on SelectedIndexChanged. 
                // In this case, the Element & Control will no longer exist.
                if (Element != null)
                {
                    if (model.Items.Count > 0 && Element.SelectedIndex >= 0)
                        Control.EditText.Text = model.Items[Element.SelectedIndex];
                    ((IElementController)Element).SetValueFromRenderer(isFocusedPropertyKey, false);
                    // It is also possible for the Content of the Page to be changed when Focus is changed.
                    // In this case, we'll lose our Control.
                    Control?.ClearFocus();
                }
                _dialog = null;
            });

            _dialog = builder.Create();
            _dialog.DismissEvent += (sender, args) =>
            {
                ((IElementController)Element).SetValueFromRenderer(isFocusedPropertyKey, false);
            };
            _dialog.Show();
        }

        private void UpdatePicker()
        {
            Control.Hint = Element.Title;

            var oldText = Control.EditText.Text;

            if (Element.SelectedIndex == -1 || Element.Items == null)
                Control.EditText.Text = null;
            else
                Control.EditText.Text = Element.Items[Element.SelectedIndex];

            if (oldText != Control.EditText.Text)
                ((IVisualElementController)Element).NativeSizeChanged();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                _isDisposed = true;
                ((INotifyCollectionChanged)Element.Items).CollectionChanged -= RowsCollectionChanged;
            }

            base.Dispose(disposing);
        }

        private class PickerListener : Object, IOnClickListener, IOnFocusChangeListener
        {
            public static readonly PickerListener Instance = new PickerListener();

            private PickerListener() { }

            public void OnClick(AView v)
            {
                var renderer = v.Tag as XfxPickerRendererDroid;
                renderer?.OnClick();
            }

            public void OnFocusChange(AView v, bool hasFocus)
            {
                if (!hasFocus) return;
                var renderer = v.Tag as XfxPickerRendererDroid;
                renderer?.OnClick();
            }
        }
    }
}