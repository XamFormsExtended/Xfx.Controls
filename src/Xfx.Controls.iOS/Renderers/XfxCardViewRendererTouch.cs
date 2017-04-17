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
        public VisualElement Element { get; private set; }
        public UIView NativeView => this;
        public UIViewController ViewController => null;

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
            SetCardBackgroundColor(view.BackgroundColor.ToUIColor());
            ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(oldElement, Element));
        }
        
        public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            var size = NativeView.GetSizeRequest(widthConstraint, heightConstraint, 44.0, 44.0);
            return size;
        }

        public void SetElementSize(Size size)
        {
            Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
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
                (e.PropertyName == XfxCardView.CornerRadiusProperty.PropertyName))
            {
                Element.Layout(Element.Bounds);

                var radius = ((XfxCardView)Element).CornerRadius;
                var bound = Element.Bounds;
                var rect = new CGRect(bound.X, bound.Y, bound.Width, bound.Height);
                DrawBorder(rect, radius);
            }
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                SetCardBackgroundColor(view.BackgroundColor.ToUIColor());
            }
        }

        private void SetCardBackgroundColor(UIColor color)
        {
            BackgroundColor = color;
        }
    }

    internal static class Extensions
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