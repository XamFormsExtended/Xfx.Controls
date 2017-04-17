using System;
using System.ComponentModel;
using Android.Support.V7.Widget;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xfx;
using Xfx.Controls.Droid.Renderers;

[assembly: ExportRenderer(typeof(XfxCardView), typeof(XfxCardViewRendererDroid))]
namespace Xfx.Controls.Droid.Renderers
{
    public class XfxCardViewRendererDroid : CardView, IVisualElementRenderer
    {
        private ViewGroup _packed; // James Montemagno, why is this here?

        public XfxCardViewRendererDroid() : base(Forms.Context)
        {
        }

        public VisualElementPackager Packager { get; private set; }

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        public void SetElement(VisualElement element)
        {
            var oldElement = Element;

            if (oldElement != null)
                oldElement.PropertyChanged -= HandlePropertyChanged;

            Element = element;
            if (Element != null)
                Element.PropertyChanged += HandlePropertyChanged;

            ViewGroup.RemoveAllViews();
            Tracker = new VisualElementTracker(this);

            Packager = new VisualElementPackager(this);
            Packager.Load();

            UseCompatPadding = true;

            var view =  Element as XfxCardView;
            if (view == null) return;

            SetContentPadding((int)view.Padding.Left,
                (int)view.Padding.Top,
                (int)view.Padding.Right,
                (int)view.Padding.Bottom);

            Radius = view.CornerRadius;

            SetCardBackgroundColor(view.BackgroundColor.ToAndroid());
            ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(oldElement, Element));
        }

        public SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            // James Montemagno, is this not null?
            _packed.Measure(widthConstraint, heightConstraint);

            //Measure child here and determine size
            return new SizeRequest(new Size(_packed.MeasuredWidth, _packed.MeasuredHeight));
        }

        public void UpdateLayout()
        {
            Tracker?.UpdateLayout();
        }

        public VisualElementTracker Tracker { get; private set; }

        public ViewGroup ViewGroup => this;

        public VisualElement Element { get; private set; }


        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var view = (XfxCardView) Element;
            if (e.PropertyName == ContentView.ContentProperty.PropertyName)
                Tracker.UpdateLayout();
            else if (e.PropertyName == Xamarin.Forms.Layout.PaddingProperty.PropertyName)
                SetContentPadding((int)view.Padding.Left,
                    (int)view.Padding.Top,
                    (int)view.Padding.Right,
                    (int)view.Padding.Bottom);
            else if (e.PropertyName == XfxCardView.CornerRadiusProperty.PropertyName)
                Radius = view.CornerRadius;
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                SetCardBackgroundColor(view.BackgroundColor.ToAndroid());
        }
    }
}