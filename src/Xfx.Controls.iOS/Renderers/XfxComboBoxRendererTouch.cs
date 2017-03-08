using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using MBAutoComplete;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xfx;
using Xfx.Controls.iOS.Extensions;
using Xfx.Controls.iOS.Renderers;
using Color = Xamarin.Forms.Color;

[assembly: ExportRenderer(typeof(XfxComboBox), typeof(XfxComboBoxRendererTouch))]

namespace Xfx.Controls.iOS.Renderers
{
    public class XfxComboBoxRendererTouch : ViewRenderer<XfxComboBox, MbAutoCompleteTextField>
    {
        private readonly CGColor _defaultLineColor = Color.Gray.ToCGColor();
        private UIColor _defaultPlaceholderColor;
        private UIColor _defaultTextColor;
        private MbAutoCompleteTextField _nativeView;
        private readonly CGColor _editingUnderlineColor = Color.Blue.ToCGColor();
        private bool _hasFocus;
        private bool _hasError;

        public XfxComboBoxRendererTouch()
        {
            // ReSharper disable once VirtualMemberCallInContructor
            Frame = new RectangleF(0, 20, 320, 40);
        }

        private new MbAutoCompleteTextField NativeView => _nativeView ?? (_nativeView = InitializeNativeView());

        private IElementController ElementController => Element as IElementController;

        private MbAutoCompleteTextField InitializeNativeView()
        {
            var view = new MbAutoCompleteTextField
            {
                AutoCompleteViewSource = new MbAutoCompleteDefaultDataSource(),
                SortingAlgorithm = Element.SortingAlgorithm
            };
            if (!string.IsNullOrWhiteSpace(Element.AutomationId))
            {
                SetAutomationId(Element.AutomationId);
            }
            view.AutoCompleteViewSource.Selected += AutoCompleteViewSourceOnSelected;

            _defaultPlaceholderColor = view.FloatingLabelTextColor;
            _defaultTextColor = view.TextColor;
            return view;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            var scrollView = GetParentScrollView(Control);
            var ctrl = UIApplication.SharedApplication.GetTopViewController();
            NativeView.Draw(ctrl, Layer, scrollView);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XfxComboBox> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                // unsubscribe
                NativeView.EditingDidBegin -= OnEditingDidBegin;
                NativeView.EditingChanged -= OnEditingChanged;
                NativeView.EditingDidEnd -= OnEditingDidEnd;
                NativeView.AutoCompleteViewSource.Selected -= AutoCompleteViewSourceOnSelected;
            }

            if (e.NewElement != null)
            {
                SetNativeControl(NativeView);
                SetText();
                SetPlaceholderText();
                SetTextColor();
                SetBackgroundColor();
                SetPlaceholderColor();
                SetKeyboard();
                SetErrorText();
                SetHorizontalAlignment();
                SetItemsSource();
                SetThreshold();
                SetFont();

                NativeView.ErrorTextIsVisible = true;
                NativeView.EditingDidBegin += OnEditingDidBegin;
                NativeView.EditingChanged += OnEditingChanged;
                NativeView.EditingDidEnd += OnEditingDidEnd;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
            {
                if (Element.IsPassword)
                    throw new NotImplementedException("Cannot set IsPassword on a XfxComboBox");
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

            if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
            {
                SetKeyboard();
            }

            if (e.PropertyName == XfxComboBox.ItemsSourceProperty.PropertyName)
            {
                SetItemsSource();
            }

            if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
            {
                SetHorizontalAlignment();
            }

            if (e.PropertyName == XfxEntry.ErrorTextProperty.PropertyName)
            {
                SetErrorText();
            }

            if (e.PropertyName == XfxComboBox.ThresholdProperty.PropertyName)
            {
                SetThreshold();
            }

            if (e.PropertyName == Entry.FontAttributesProperty.PropertyName ||
                e.PropertyName == Entry.FontFamilyProperty.PropertyName ||
                e.PropertyName == Entry.FontSizeProperty.PropertyName)
            {
                SetFont();
            }
        }

        private void SetFont()
        {
            _nativeView.Font = Element.ToUIFont();
        }

        private void SetThreshold()
        {
            NativeView.Threshold = Element.Threshold;
        }

        private void SetItemsSource()
        {
            var items = Element.ItemsSource.ToList();
            NativeView.UpdateItems(items);
        }

        private void SetHorizontalAlignment()
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

        private void SetBackgroundColor()
        {
            NativeView.BackgroundColor = Element.BackgroundColor.ToUIColor();
            NativeView.UnderlineColor = _defaultLineColor;
        }

        private void SetErrorText()
        {
            _hasError = !string.IsNullOrEmpty(Element.ErrorText);
            NativeView.UnderlineColor = GetUnderlineColorForState();
            NativeView.ErrorTextIsVisible = _hasError;
            NativeView.ErrorText = Element.ErrorText;
        }

        private void SetPlaceholderColor()
        {
            NativeView.FloatingLabelTextColor = Element.PlaceholderColor == Color.Default ?
                _defaultPlaceholderColor :
                Element.PlaceholderColor.ToUIColor();
        }

        private void SetTextColor()
        {
            NativeView.TextColor = Element.TextColor == Color.Default
                ? _defaultTextColor
                : Element.TextColor.ToUIColor();
        }

        private void SetPlaceholderText()
        {
            NativeView.Placeholder = Element.Placeholder;
        }

        private void SetText()
        {
            if (NativeView.Text != Element.Text)
            {
                NativeView.Text = Element.Text;
            }
        }

        private UIScrollView GetParentScrollView(UIView element)
        {
            if (element.Superview == null) return null;
            var scrollView = element.Superview as UIScrollView;
            return scrollView ?? GetParentScrollView(element.Superview);
        }

        private CGColor GetUnderlineColorForState()
        {
            if (_hasError) return UIColor.Red.CGColor;
            return _hasFocus ? _editingUnderlineColor : _defaultLineColor;
        }

        private void AutoCompleteViewSourceOnSelected(object sender, SelectedItemChangedEventArgs args)
        {
            Element.OnItemSelectedInternal(Element, args);
            // TODO : Florell, Chase (Contractor) 02/15/17 SET FOCUS
        }

        private void OnEditingChanged(object sender, EventArgs eventArgs)
        {
            ElementController?.SetValueFromRenderer(Entry.TextProperty, Control.Text);
        }

        private void OnEditingDidEnd(object sender, EventArgs eventArgs)
        {
            _hasFocus = false;
            NativeView.UnderlineColor = GetUnderlineColorForState();
        }

        private void OnEditingDidBegin(object sender, EventArgs eventArgs)
        {
            _hasFocus = true;
            NativeView.UnderlineColor = GetUnderlineColorForState();
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