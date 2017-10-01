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

        public static readonly BindableProperty ElevationProperty = BindableProperty.Create(nameof(Elevation),
            typeof(float),
            typeof(XfxCardView),
            -1f);

        /// <summary>
        ///    Corner Radius. This is a bindable property. Default is 3.0f
        /// </summary>
        public float CornerRadius
        {
            get { return (float)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Distance drop shadow will be from the CardView. This is a bindalbe property. Default is -1f
        /// </summary>
        /// <value>The elevation.</value>
        public float Elevation
        {
            get { return (float)GetValue(ElevationProperty); }
            set{SetValue(ElevationProperty, value);}
        }


        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            return Content == null ? 
                new SizeRequest(new Size(100, 100)) : 
                base.OnMeasure(widthConstraint, heightConstraint);
        }
    }
}