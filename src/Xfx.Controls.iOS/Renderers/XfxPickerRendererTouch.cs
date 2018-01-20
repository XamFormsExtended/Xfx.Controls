using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xfx.Controls.iOS.Controls;
using Xfx.Extensions;
using Color = Xamarin.Forms.Color;
using Picker = Xamarin.Forms.Picker;
using VisualElement = Xamarin.Forms.VisualElement;

namespace Xfx.Controls.iOS.Renderers
{
    public class XfxPickerRendererTouch : ViewRenderer<XfxPicker, FloatLabeledTextField>
    {
        private UIColor _defaultTextColor;
        private bool _hasError;
        private bool _hasFocus;
        private FloatLabeledTextField _nativeView;
        private UIPickerView _picker;
        private new FloatLabeledTextField NativeView => _nativeView ?? (_nativeView = InitializeNativeView());
        private IElementController ElementController => Element;

        private FloatLabeledTextField InitializeNativeView()
        {
            var entry = new FloatLabeledTextField();

            entry.EditingDidBegin += OnStarted;
            entry.EditingDidEnd += OnEnded;
            entry.EditingChanged += OnEditing;

            var width = (float)UIScreen.MainScreen.Bounds.Width;
            var toolbar =
                new UIToolbar(new RectangleF(0, 0, width, 44))
                {
                    BarStyle = UIBarStyle.Default,
                    Translucent = true
                };
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) =>
            {
                var s = (PickerSource)_picker.Model;
                if (s.SelectedIndex == -1 && Element.Items != null && Element.Items.Count > 0)
                    UpdatePickerSelectedIndex(0);
                UpdatePickerFromModel(s);
                entry.ResignFirstResponder();
            });

            toolbar.SetItems(new[] { spacer, doneButton }, false);

            entry.InputView = _picker;
            entry.InputAccessoryView = toolbar;

            _defaultTextColor = entry.TextColor;
            if (!string.IsNullOrWhiteSpace(Element.AutomationId))
            {
                SetAutomationId(Element.AutomationId);
            }
            _defaultTextColor = entry.TextColor;
            return entry;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XfxPicker> e)
        {
            if (e.OldElement != null)
                ((INotifyCollectionChanged)e.OldElement.Items).CollectionChanged -= RowsCollectionChanged;

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    _picker = new UIPickerView();
                    SetNativeControl(NativeView);
                }

                _picker.Model = new PickerSource(this);

                UpdatePicker();
                UpdateTextColor();
                SetBackgroundColor();
                SetError();

                ((INotifyCollectionChanged)e.NewElement.Items).CollectionChanged += RowsCollectionChanged;
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Picker.TitleProperty.PropertyName)
                UpdatePicker();
            if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
                UpdatePicker();
            if (e.PropertyName == Picker.TextColorProperty.PropertyName ||
                e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                UpdateTextColor();
            if (e.PropertyName == XfxPicker.ErrorTextProperty.PropertyName)
                SetError();
            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                SetBackgroundColor();
            else if (e.PropertyName == XfxPicker.FloatingHintEnabledProperty.PropertyName)
                SetFloatingHintEnabled();
            else if (e.PropertyName == XfxPicker.ActiveLabelColorProperty.PropertyName)
                SetFocusedColor();
        }

        private void SetFocusedColor()
        {
            Control.FloatingLabelActiveTextColor = Element.ActiveLabelColor == Color.Accent
                ? Control.TintColor
                : Element.ActiveLabelColor.ToUIColor();
        }

        public void SetBackgroundColor()
        {
            NativeView.BackgroundColor = Element.BackgroundColor.ToUIColor();
            Control.UnderlineColor = Element.LabelColor.ToCGColor();
        }

        private void SetFloatingHintEnabled()
        {
            Control.FloatingLabelEnabled = Element.FloatingHintEnabled;
        }

        private void SetError()
        {
            _hasError = !string.IsNullOrEmpty(Element.ErrorText);
            NativeView.UnderlineColor = GetUnderlineColorForState();
            NativeView.UnderlineErrorTextIsVisible = _hasError;
            NativeView.ErrorText = Element.ErrorText;
        }

        private CGColor GetUnderlineColorForState()
        {
            if (_hasError) return UIColor.Red.CGColor;
            return _hasFocus
                ? (Element.ActiveLabelColor == Color.Accent
                    ? Control.TintColor.CGColor
                    : Element.ActiveLabelColor.ToCGColor())
                : (Element.LabelColor == Color.Default
                    ? Control.TextColor.CGColor
                    : Element.LabelColor.ToCGColor());
        }

        private void UpdateTextColor()
        {
            var textColor = Element.TextColor;

            if (textColor == Color.Default || !Element.IsEnabled)
                Control.TextColor = _defaultTextColor;
            else
                Control.TextColor = textColor.ToUIColor();
        }

        private void UpdatePicker()
        {
            var selectedIndex = Element.SelectedIndex;
            var items = Element.Items;
            Control.Placeholder = Element.Title;
            var oldText = Control.Text;
            Control.Text = selectedIndex == -1 || items == null ? "" : items[selectedIndex];
            UpdatePickerNativeSize(oldText);
            _picker.ReloadAllComponents();
            if (items == null || items.Count == 0)
                return;

            UpdatePickerSelectedIndex(selectedIndex);
        }

        private void UpdatePickerSelectedIndex(int i)
        {
            var source = (PickerSource)_picker.Model;
            source.SelectedIndex = i;
            source.SelectedItem = i >= 0 ? Element.Items[i] : null;
            _picker.Select(Math.Max(i, 0), 0, true);
        }

        private void OnEditing(object sender, EventArgs e)
        {
            // Reset the TextField's Text so it appears as if typing with a keyboard does not work.
            var selectedIndex = Element.SelectedIndex;
            var items = Element.Items;
            Control.Text = selectedIndex == -1 || items == null ? "" : items[selectedIndex];
        }

        private void OnEnded(object sender, EventArgs e)
        {
            var s = (PickerSource)_picker.Model;
            if (s.SelectedIndex != _picker.SelectedRowInComponent(0))
                _picker.Select(s.SelectedIndex, 0, false);
            _hasFocus = false;
            var isFocusedPropertyKey = Element.GetInternalField<BindablePropertyKey>("IsFocusedPropertyKey");
            ElementController.SetValueFromRenderer(isFocusedPropertyKey, false);
            Control.UnderlineColor = GetUnderlineColorForState();
        }

        private void OnStarted(object sender, EventArgs e)
        {
            _hasFocus = true;
            var isFocusedPropertyKey = Element.GetInternalField<BindablePropertyKey>("IsFocusedPropertyKey");
            ElementController.SetValueFromRenderer(isFocusedPropertyKey, true);
            Control.UnderlineColor = GetUnderlineColorForState();
        }

        private void RowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePicker();
        }

        public void UpdatePickerFromModel(PickerSource s)
        {
            if (Element != null)
            {
                var oldText = Control.Text;
                Control.Text = s.SelectedItem;
                UpdatePickerNativeSize(oldText);
                ElementController.SetValueFromRenderer(Picker.SelectedIndexProperty, s.SelectedIndex);
            }
        }

        private void UpdatePickerNativeSize(string oldText)
        {
            if (oldText != Control.Text)
                ((IVisualElementController)Element).NativeSizeChanged();
        }
    }

    public class PickerSource : UIPickerViewModel
    {
        private bool _disposed;
        private XfxPickerRendererTouch _renderer;

        public PickerSource(XfxPickerRendererTouch renderer)
        {
            _renderer = renderer;
        }

        public int SelectedIndex { get; internal set; }

        public string SelectedItem { get; internal set; }

        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return _renderer.Element.Items?.Count ?? 0;
        }

        public override string GetTitle(UIPickerView picker, nint row, nint component)
        {
            return _renderer.Element.Items[(int)row];
        }

        public override void Selected(UIPickerView picker, nint row, nint component)
        {
            if (_renderer.Element.Items.Count == 0)
            {
                SelectedItem = null;
                SelectedIndex = -1;
            }
            else
            {
                SelectedItem = _renderer.Element.Items[(int)row];
                SelectedIndex = (int)row;
            }

            if (_renderer.Element.On<Xamarin.Forms.PlatformConfiguration.iOS>().UpdateMode() == UpdateMode.Immediately)
                _renderer.UpdatePickerFromModel(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing)
                _renderer = null;

            base.Dispose(disposing);
        }
    }
}