using System;
using System.Linq;
using CoreGraphics;
using UIKit;

namespace Xfx.Controls.iOS.Controls
{
    public class UICardView : UIView
    {
        private UIView _shadowLayer;
        private float _requestedElevation = -1f;
        public override void MovedToSuperview()
        {
            if (_shadowLayer != null) return;

            _shadowLayer = new UIView { BackgroundColor = UIColor.Red };
            _shadowLayer.Layer.ShadowColor = UIColor.DarkGray.CGColor;
            _shadowLayer.Layer.ShadowOffset = new CGSize(0.0f, 1f);
            _shadowLayer.Layer.ShadowOpacity = 1f;
            _shadowLayer.Layer.ShadowRadius = _requestedElevation < 0f ? 3f:_requestedElevation;
            _shadowLayer.Layer.MasksToBounds = false;
            _shadowLayer.Layer.ShouldRasterize = true;
            _shadowLayer.Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            Superview.InsertSubviewBelow(_shadowLayer, this);
        }

        protected void DrawBorder(CGRect rect, nfloat radius)
        {
            _shadowLayer.Frame = rect;
            _shadowLayer.Layer.CornerRadius = radius;
            //var bounds = _shadowLayer.Layer.Bounds;
            //_shadowLayer.Layer.ShadowPath = UIBezierPath.FromRoundedRect(bounds, radius).CGPath;

            //LayoutMargins = new UIEdgeInsets(20, 40, 40, 20);
            Frame = rect;
            Layer.BorderColor = UIColor.LightGray.CGColor;
            Layer.BorderWidth = 0.1f;
            Layer.MasksToBounds = true;
            Layer.CornerRadius = radius;
            Layer.ShouldRasterize = true;
            Layer.RasterizationScale = UIScreen.MainScreen.Scale;
        }

        public void SetElevation(float elevation)
        {
            _requestedElevation = elevation/2;
            if (_shadowLayer?.Layer?.ShadowRadius == null) return;
            _shadowLayer.Layer.ShadowRadius = elevation / 2;
        }
    }
}