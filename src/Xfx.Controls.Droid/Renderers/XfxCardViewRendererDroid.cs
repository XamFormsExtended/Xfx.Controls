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

        protected XfxCardView CardView => (XfxCardView)Element;
        public VisualElementPackager Packager { get; private set; }
        public Android.Views.View View => this;
        public VisualElementTracker Tracker { get; private set; }
        public VisualElement Element { get; private set; }
        [Obsolete("ViewGroup is obsolete as of version 2.3.5. Please use View instead.")]
        public ViewGroup ViewGroup => this;

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
        public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

        public void SetElement(VisualElement element)
        {
            var oldElement = Element;

            if (oldElement != null)
                oldElement.PropertyChanged -= HandlePropertyChanged;

            Element = element;
            if (Element != null)
                Element.PropertyChanged += HandlePropertyChanged;

            //sizes to match the forms view
            //updates properties, handles visual element properties
            Tracker = new VisualElementTracker(this);
            Packager = new VisualElementPackager(this);
            Packager.Load();
            UseCompatPadding = true;

            SetContentPadding((int)CardView.Padding.Left,
                (int)CardView.Padding.Top,
                (int)CardView.Padding.Right,
                (int)CardView.Padding.Bottom);

            Radius = CardView.CornerRadius;

            SetCardBackgroundColor(CardView.BackgroundColor.ToAndroid());
            RaiseElementChanged(new VisualElementChangedEventArgs(oldElement, Element));
        }

        public void SetLabelFor(int? id)
        {
        }

        public SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            // James Montemagno, is this not null?
            View.Measure(widthConstraint, heightConstraint);

            //Measure child here and determine size
            return new SizeRequest(new Size(View.MeasuredWidth, View.MeasuredHeight));
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content")
                Tracker.UpdateLayout();
            else if (e.PropertyName == Xamarin.Forms.Layout.PaddingProperty.PropertyName)
                SetContentPadding((int)CardView.Padding.Left,
                    (int)CardView.Padding.Top,
                    (int)CardView.Padding.Right,
                    (int)CardView.Padding.Bottom);
            else if (e.PropertyName == XfxCardView.CornerRadiusProperty.PropertyName)
                Radius = CardView.CornerRadius;
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                SetCardBackgroundColor(CardView.BackgroundColor.ToAndroid());
            RaiseElementPropertyChanged(e);
        }

        public void UpdateLayout() => Tracker?.UpdateLayout();

        private void RaiseElementChanged(VisualElementChangedEventArgs args) => ElementChanged?.Invoke(this, args);

        private void RaiseElementPropertyChanged(PropertyChangedEventArgs args) => ElementPropertyChanged?.Invoke(this, args);
    }
}