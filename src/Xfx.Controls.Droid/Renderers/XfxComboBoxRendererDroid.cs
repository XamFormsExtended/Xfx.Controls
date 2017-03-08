using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xfx;
using Xfx.Controls.Droid.Renderers;

[assembly: ExportRenderer(typeof (XfxComboBox), typeof (XfxComboBoxRendererDroid))]

namespace Xfx.Controls.Droid.Renderers
{
    public class XfxComboBoxRendererDroid : XfxEntryRendererDroid
    {
        private AutoCompleteTextView AutoComplete => (AutoCompleteTextView) Control.EditText;

        protected override TextInputLayout CreateNativeControl()
        {
            var layout = (TextInputLayout) LayoutInflater.From(Context).Inflate(Resource.Layout.AutoCompleteTextInputLayout, null);
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
                AutoComplete.ItemClick -= AutoCompleteOnItemSelected;
            }

            if (e.NewElement != null)
            {
                // subscribe
                SetItemsSource();
                SetThreshold();
                AutoComplete.ItemClick += AutoCompleteOnItemSelected;
            }
        }


        private void AutoCompleteOnItemSelected(object sender, AdapterView.ItemClickEventArgs args)
        {
            var view = (AutoCompleteTextView) sender;
            var selectedItemArgs = new SelectedItemChangedEventArgs(view.Text);
            var element = (Xfx.XfxComboBox) Element;
            element.OnItemSelectedInternal(Element, selectedItemArgs);
            HideKeyboard();
            // TODO : Florell, Chase (Contractor) 02/15/17 SET FOCUS
        }

        private void SetThreshold()
        {
            var element = (Xfx.XfxComboBox) Element;
            AutoComplete.Threshold = element.Threshold;
        }

        private void SetItemsSource()
        {
            var element = (Xfx.XfxComboBox) Element;
            if (element.ItemsSource == null) return;

            var observable = element.ItemsSource as ObservableCollection<string>;
            if (observable != null)
            {
                observable.CollectionChanged -= ItemsSourceOnCollectionChanged;
            }

            ResetAdapter(element);

            if (observable != null)
            {
                observable.CollectionChanged += ItemsSourceOnCollectionChanged;
            }
        }

        private void ItemsSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var element = (Xfx.XfxComboBox) Element;
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

        protected override void Dispose(bool disposing)
        {
            var element = (Xfx.XfxComboBox) Element;
            if (element.ItemsSource == null || !element.ItemsSource.Any()) return;

            var observable = element.ItemsSource as ObservableCollection<string>;
            if (observable != null)
            {
                observable.CollectionChanged -= ItemsSourceOnCollectionChanged;
            }
            base.Dispose(disposing);
        }
    }
}