using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Android.Support.Design.Widget;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xfx;
using Xfx.Controls.Droid.Renderers;

[assembly: ExportRenderer(typeof(XfxComboBox), typeof(XfxComboBoxRendererDroid))]

namespace Xfx.Controls.Droid.Renderers
{
    public class XfxComboBoxRendererDroid : XfxEntryRendererDroid
    {
        private AutoCompleteTextView AutoComplete => (AutoCompleteTextView)Control.EditText;

        protected override TextInputLayout CreateNativeControl()
        {
            var textInputLayout = new TextInputLayout(Context);
            var autoCompleteTextView = new AutoCompleteTextView(Context);
            textInputLayout.AddView(autoCompleteTextView);
            return textInputLayout;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XfxEntry> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                // unsubscribe
                AutoComplete.ItemClick -= AutoCompleteOnItemSelected;
                var elm = (Xfx.XfxComboBox)e.OldElement;
                elm.CollectionChanged -= ItemsSourceCollectionChanged;
            }

            if (e.NewElement != null)
            {
                // subscribe
                SetItemsSource();
                SetThreshold();
                KillPassword();
                AutoComplete.ItemClick += AutoCompleteOnItemSelected;
                var elm = (Xfx.XfxComboBox)e.NewElement;
                elm.CollectionChanged += ItemsSourceCollectionChanged;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
                KillPassword();
            if (e.PropertyName == Xfx.XfxComboBox.ItemsSourceProperty.PropertyName)
                SetItemsSource();
            else if (e.PropertyName == Xfx.XfxComboBox.ThresholdProperty.PropertyName)
                SetThreshold();
        }

        private void AutoCompleteOnItemSelected(object sender, AdapterView.ItemClickEventArgs args)
        {
            var view = (AutoCompleteTextView)sender;
            var selectedItemArgs = new SelectedItemChangedEventArgs(view.Text);
            var element = (Xfx.XfxComboBox)Element;
            element.OnItemSelectedInternal(Element, selectedItemArgs);
            HideKeyboard();
            // TODO : Florell, Chase (Contractor) 02/15/17 SET FOCUS
        }

        private void SetThreshold()
        {
            var element = (Xfx.XfxComboBox)Element;
            AutoComplete.Threshold = element.Threshold;
        }

        private void SetItemsSource()
        {
            var element = (Xfx.XfxComboBox)Element;
            if (element.ItemsSource == null) return;

            ResetAdapter(element);
        }

        private void KillPassword()
        {
            if (Element.IsPassword)
                throw new NotImplementedException("Cannot set IsPassword on a XfxComboBox");
        }

        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var element = (Xfx.XfxComboBox)Element;
            ResetAdapter(element);
        }

        private void ResetAdapter(Xfx.XfxComboBox element)
        {
            var adapter = new XfxComboBox.XfxComboBoxArrayAdapter(Context,
                Android.Resource.Layout.SimpleDropDownItem1Line,
                element.ItemsSource.ToList(),
                element.SortingAlgorithm);
            AutoComplete.Adapter = adapter;
            adapter.NotifyDataSetChanged();
        }
    }
}