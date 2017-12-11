using System;
using System.ComponentModel;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xfx;
using Xfx.Controls.iOS.Controls;
using Xfx.Controls.iOS.Renderers;

[assembly: ExportRenderer(typeof(XfxCardView), typeof(XfxCardViewRendererTouch))]

namespace Xfx.Controls.iOS.Renderers
{
    public class XfxCardViewRendererTouch : UICardView, IVisualElementRenderer
    {
        public VisualElementTracker Tracker { get; private set; }
        public VisualElementPackager Packager { get; private set; }
        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
        public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;
        public VisualElement Element { get; private set; }
        public UIView NativeView => this;
        public UIViewController ViewController => null;
        public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint) => NativeView.GetSizeRequest(widthConstraint, heightConstraint, 44.0, 44.0);
        public void SetElementSize(Size size) => Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
        protected XfxCardView CardView => (XfxCardView)Element;
        protected virtual void OnElementChanged(object sender, VisualElementChangedEventArgs args) { }
        protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args) { }
        private void SetElevation() => SetElevation(CardView.Elevation);
        private void SetCardBackgroundColor() => BackgroundColor = CardView.BackgroundColor.ToUIColor();

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

            this.RemoveAllSubviews();
            Tracker = new VisualElementTracker(this);
            Packager = new VisualElementPackager(this);
            Packager.Load();
            var view = Element as XfxCardView;
            if (view == null) return;
            SetCardBackgroundColor();
            SetElevation();
            RaiseElementChanged(new VisualElementChangedEventArgs(oldElement, Element));
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var view = (XfxCardView)Element;

            if (e.PropertyName == "Content")
            {
                //Tracker.UpdateLayout ();
            }
            else if (
                (e.PropertyName == VisualElement.WidthProperty.PropertyName) ||
                (e.PropertyName == VisualElement.HeightProperty.PropertyName) ||
                (e.PropertyName == VisualElement.XProperty.PropertyName) ||
                (e.PropertyName == VisualElement.YProperty.PropertyName) ||
                (e.PropertyName == XfxCardView.CornerRadiusProperty.PropertyName) ||
                (e.PropertyName == XfxCardView.ElevationProperty.PropertyName))
            {
                Element.Layout(Element.Bounds);

                var radius = ((XfxCardView)Element).CornerRadius;
                var bound = Element.Bounds;
                var rect = new CGRect(bound.X, bound.Y, bound.Width, bound.Height);
                var elevation = ((XfxCardView)Element).Elevation;
                DrawBorder(rect, radius);
            }
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                SetCardBackgroundColor();
            else if (e.PropertyName == XfxCardView.ElevationProperty.PropertyName)
                SetElevation();
            RaiseElementPropertyChanged(e);
        }

        private void RaiseElementPropertyChanged(PropertyChangedEventArgs args)
        {
            ElementPropertyChanged?.Invoke(this, args);
            OnElementPropertyChanged(this, args);
        }

        private void RaiseElementChanged(VisualElementChangedEventArgs args)
        {
            ElementChanged?.Invoke(this, args);
            OnElementChanged(this, args);
        }
    }

    static class Extensions
    {
        internal static void RemoveAllSubviews(this UIView super)
        {
            if (super == null)
                return;
            for (var i = 0; i < super.Subviews.Length; i++)
            {
                var subview = super.Subviews[i];
                subview.RemoveFromSuperview();
            }
        }
    }
}