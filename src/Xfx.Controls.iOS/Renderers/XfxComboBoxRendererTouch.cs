using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using MBAutoComplete;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xfx;
using Xfx.Controls.iOS.Controls;
using Xfx.Controls.iOS.Extensions;
using Xfx.Controls.iOS.Renderers;

[assembly: ExportRenderer(typeof(XfxComboBox), typeof(XfxComboBoxRendererTouch))]

namespace Xfx.Controls.iOS.Renderers
{
    public class XfxComboBoxRendererTouch : XfxEntryRendererTouch
    {
        private MbAutoCompleteTextField NativeControl => (MbAutoCompleteTextField)Control;
        private XfxComboBox ComboBox => (XfxComboBox)Element;

        public XfxComboBoxRendererTouch()
        {
            // ReSharper disable once VirtualMemberCallInContructor
            Frame = new RectangleF(0, 20, 320, 40);
        }

        protected override FloatLabeledTextField CreateNativeControl()
        {
            var element = (XfxComboBox)Element;
            var view = new MbAutoCompleteTextField
            {
                AutoCompleteViewSource = new MbAutoCompleteDefaultDataSource(),
                SortingAlgorithm = element.SortingAlgorithm
            };
            view.AutoCompleteViewSource.Selected += AutoCompleteViewSourceOnSelected;
            return view;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            var scrollView = GetParentScrollView(Control);
            var ctrl = UIApplication.SharedApplication.GetTopViewController();
            NativeControl.Draw(ctrl, Layer, scrollView);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XfxEntry> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                // unsubscribe
                NativeControl.AutoCompleteViewSource.Selected -= AutoCompleteViewSourceOnSelected;
                var elm = (XfxComboBox)e.OldElement;
                elm.CollectionChanged -= ItemsSourceCollectionChanged;
            }

            if (e.NewElement != null)
            {
                SetItemsSource();
                SetThreshold();
                KillPassword();

                var elm = (XfxComboBox)e.NewElement;
                elm.CollectionChanged += ItemsSourceCollectionChanged;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
                KillPassword();
            if (e.PropertyName == XfxComboBox.ItemsSourceProperty.PropertyName)
                SetItemsSource();
            else if (e.PropertyName == XfxComboBox.ThresholdProperty.PropertyName)
                SetThreshold();
        }

        private void SetThreshold()
        {
            NativeControl.Threshold = ComboBox.Threshold;
        }

        private void SetItemsSource()
        {
            var items = ComboBox.ItemsSource.ToList();
            NativeControl.UpdateItems(items);
        }

        private void KillPassword()
        {
            if (Element.IsPassword)
                throw new NotImplementedException("Cannot set IsPassword on a XfxComboBox");
        }

        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            SetItemsSource();
        }

        private static UIScrollView GetParentScrollView(UIView element)
        {
            if (element.Superview == null) return null;
            var scrollView = element.Superview as UIScrollView;
            return scrollView ?? GetParentScrollView(element.Superview);
        }

        private void AutoCompleteViewSourceOnSelected(object sender, XfxSelectedItemChangedEventArgs args)
        {
            ComboBox.OnItemSelectedInternal(Element, args);
            // TODO : Florell, Chase (Contractor) 02/15/17 SET FOCUS
        }
    }
}