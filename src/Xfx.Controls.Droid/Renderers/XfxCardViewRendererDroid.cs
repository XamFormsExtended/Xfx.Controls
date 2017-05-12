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
        public XfxCardViewRendererDroid() : base(Forms.Context)
        {
        }

        public VisualElementPackager Packager { get; private set; }
        private XfxCardView XCard => (XfxCardView) Element;
        public VisualElementTracker Tracker { get; private set; }
        public ViewGroup ViewGroup => this;
        public VisualElement Element { get; private set; }
        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        public void SetElement(VisualElement element)
        {
            var oldElement = Element;

            if (oldElement != null)
            {
                oldElement.PropertyChanged -= HandlePropertyChanged;
            }

            Element = element;
            if (Element != null)
            {
                Element.PropertyChanged += HandlePropertyChanged;
            }

            ViewGroup.RemoveAllViews();
            Tracker = new VisualElementTracker(this);

            Packager = new VisualElementPackager(this);
            Packager.Load();

            UseCompatPadding = true;
            var view = Element as XfxCardView;
            if (view == null) return;

            UpdatePadding();
            UpdateRadius();
            UpdateBackgroundColor();
            ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(oldElement, Element));
        }

        public SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            // this was never constructed!?!? 
            //_packed.Measure(widthConstraint, heightConstraint);

            //Measure child here and determine size
            return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight));
        }

        public void UpdateLayout()
        {
            Tracker?.UpdateLayout();
        }


        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ContentView.ContentProperty.PropertyName)
                UpdateLayout();
            else if (e.PropertyName == Xamarin.Forms.Layout.PaddingProperty.PropertyName)
                UpdatePadding();
            else if (e.PropertyName == XfxCardView.CornerRadiusProperty.PropertyName)
                UpdateRadius();
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                UpdateBackgroundColor();
        }

        private void UpdateBackgroundColor()
        {
            SetCardBackgroundColor(XCard.BackgroundColor.ToAndroid());
        }

        private void UpdateRadius()
        {
            Radius = XCard.CornerRadius;
        }

        private void UpdatePadding()
        {
            SetContentPadding((int) XCard.Padding.Left,
                (int) XCard.Padding.Top,
                (int) XCard.Padding.Right,
                (int) XCard.Padding.Bottom);
        }
    }
}