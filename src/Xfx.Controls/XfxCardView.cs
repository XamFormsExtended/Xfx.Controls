using Xamarin.Forms;

namespace Xfx
{
    public class XfxCardView : ContentView
    {
        public XfxCardView()
        {
            BackgroundColor = Color.White;
            Padding = 0;
            HorizontalOptions=LayoutOptions.Fill;
            VerticalOptions=LayoutOptions.Start;
        }
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius),
            typeof(float),
            typeof(XfxCardView),
            3.0f);

        /// <summary>
        ///    Corner Radius. This is a bindable property.
        /// </summary>
        public float CornerRadius
        {
            get { return (float)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }


        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            return Content == null ? 
                new SizeRequest(new Size(100, 100)) : 
                base.OnMeasure(widthConstraint, heightConstraint);
        }
    }
}