using System;
using System.ComponentModel;
using System.Drawing;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xfx;
using Xfx.Controls.iOS.Controls;
using Xfx.Controls.iOS.Extensions;
using Xfx.Controls.iOS.Renderers;
using Color = Xamarin.Forms.Color;

[assembly: ExportRenderer(typeof(XfxEntry), typeof(XfxEntryRenderer_Touch))]

namespace Xfx.Controls.iOS.Renderers
{
    public class XfxEntryRenderer_Touch : ViewRenderer<XfxEntry, FloatLabeledTextField>
    {
        private readonly CGColor _defaultLineColor = Color.FromHex("#666666").ToCGColor();
        private FloatLabeledTextField _nativeView;
        private UIColor _defaultPlaceholderColor;
        private UIColor _defaultTextColor;
        private readonly CGColor _editingUnderlineColor = UIColor.Blue.CGColor;
        private bool _hasFocus;
        private bool _hasError;

        private IElementController ElementController => Element as IElementController;

        private new FloatLabeledTextField NativeView
        {
            get { return _nativeView ?? (_nativeView = InitializeNativeView()); }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XfxEntry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // unsubscribe
                if (e.OldElement != null)
                {
                    NativeView.EditingDidBegin -= OnEditingDidBegin;
                    NativeView.EditingDidEnd -= OnEditingDidEnd;
                    NativeView.EditingChanged -= ViewOnEditingChanged;
                }
            }

            if (e.NewElement != null)
            {
                SetNativeControl(NativeView);

                SetIsPassword();
                SetText();
                SetHintText();
                SetTextColor();
                SetBackgroundColor();
                SetPlaceholderColor();
                SetKeyboard();
                SetHorizontalTextAlignment();
                SetErrorText();
                SetFont();

                NativeView.ErrorTextIsVisible = true;
                NativeView.EditingDidBegin += OnEditingDidBegin;
                NativeView.EditingDidEnd += OnEditingDidEnd;
                NativeView.EditingChanged += ViewOnEditingChanged;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Entry.PlaceholderProperty.PropertyName)
            {
                SetHintText();
            }
            else if (e.PropertyName == XfxEntry.ErrorTextProperty.PropertyName)
            {
                SetErrorText();
            }
            else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
            {
                SetTextColor();
            }
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                SetBackgroundColor();
            }
            else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
            {
                SetIsPassword();
            }
            else if (e.PropertyName == Entry.TextProperty.PropertyName)
            {
                SetText();
            }
            else if (e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
            {
                SetPlaceholderColor();
            }
            else if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
            {
                SetKeyboard();
            }
            else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
            {
                SetHorizontalTextAlignment();
            }
            else if (e.PropertyName == Entry.FontAttributesProperty.PropertyName ||
                     e.PropertyName == Entry.FontFamilyProperty.PropertyName ||
                     e.PropertyName == Entry.FontSizeProperty.PropertyName)
            {
                SetFont();
            }
        }

        private void OnEditingDidEnd(object sender, EventArgs eventArgs)
        {
            // TODO : Florell, Chase (Contractor) 02/21/17 unfocus
            _hasFocus = false;
            NativeView.UnderlineColor = GetUnderlineColorForState();
        }

        private void OnEditingDidBegin(object sender, EventArgs eventArgs)
        {
            // TODO : Florell, Chase (Contractor) 02/21/17 focus
            _hasFocus = true;
            NativeView.UnderlineColor = GetUnderlineColorForState();
        }

        private FloatLabeledTextField InitializeNativeView()
        {
            var view = new FloatLabeledTextField();
            if (!string.IsNullOrWhiteSpace(Element.AutomationId))
            {
                SetAutomationId(Element.AutomationId);
            }
            _defaultPlaceholderColor = view.FloatingLabelTextColor;
            _defaultTextColor = view.TextColor;
            return view;
        }

        private void ViewOnEditingChanged(object sender, EventArgs eventArgs)
        {
            ElementController?.SetValueFromRenderer(Entry.TextProperty, Control.Text);
        }

        private void SetErrorText()
        {
            _hasError = !string.IsNullOrEmpty(Element.ErrorText);
            NativeView.UnderlineColor = GetUnderlineColorForState();
            NativeView.ErrorTextIsVisible = _hasError;
            NativeView.ErrorText = Element.ErrorText;
        }

        private CGColor GetUnderlineColorForState()
        {
            if (_hasError) return UIColor.Red.CGColor;
            return _hasFocus ? _editingUnderlineColor : _defaultLineColor;
        }

        private void SetBackgroundColor()
        {
            NativeView.BackgroundColor = Element.BackgroundColor.ToUIColor();
            NativeView.UnderlineColor = _defaultLineColor;
        }

        private void SetText()
        {
            if (NativeView.Text != Element.Text)
            {
                NativeView.Text = Element.Text;
            }
        }

        private void SetIsPassword()
        {
            NativeView.SecureTextEntry = Element.IsPassword;
        }

        private void SetHintText()
        {
            NativeView.Placeholder = Element.Placeholder;
        }

        private void SetPlaceholderColor()
        {
            NativeView.FloatingLabelTextColor = Element.PlaceholderColor == Color.Default ?
                _defaultPlaceholderColor :
                Element.PlaceholderColor.ToUIColor();
        }

        private void SetTextColor()
        {
            NativeView.TextColor = Element.TextColor == Color.Default ? _defaultTextColor : Element.TextColor.ToUIColor();
        }

        private void SetFont()
        {
            NativeView.Font = Element.ToUIFont();
        }

        private void SetHorizontalTextAlignment()
        {
            switch (Element.HorizontalTextAlignment)
            {
                case TextAlignment.Center:
                    Control.TextAlignment = UITextAlignment.Center;
                    break;
                case TextAlignment.End:
                    Control.TextAlignment = UITextAlignment.Right;
                    break;
                default:
                    Control.TextAlignment = UITextAlignment.Left;
                    break;
            }
        }

        private void SetKeyboard()
        {
            var kbd = Element.Keyboard.ToNative();
            NativeView.KeyboardType = kbd;
            Control.InputAccessoryView = kbd == UIKeyboardType.NumberPad ? NumberpadAccessoryView() : null;
            NativeView.ShouldReturn = delegate { return InvokeCompleted(); };
        }

        private UIToolbar NumberpadAccessoryView()
        {
            return new UIToolbar(new RectangleF(0.0f, 0.0f, (float)Control.Frame.Size.Width, 44.0f))
            {
                Items = new[]
                {
                    new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                    new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {InvokeCompleted();})
                }
            };
        }

        private bool InvokeCompleted()
        {
            Control.ResignFirstResponder();
            ((IEntryController)Element).SendCompleted();
            return true;
        }
    }
}