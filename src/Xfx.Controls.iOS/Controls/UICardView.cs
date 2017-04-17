using System;
using CoreGraphics;
using UIKit;

namespace Xfx.Controls.iOS.Controls
{
    public class UICardView : UIView
    {
        private readonly UIView _shadowLayer;
        private bool _isAdded;

        public UICardView()
        {
            _shadowLayer = new UIView {BackgroundColor = UIColor.Red};
            _shadowLayer.Layer.ShadowColor = UIColor.LightGray.CGColor;
            _shadowLayer.Layer.ShadowOffset = new CGSize(0.0f, 2.0f);
            _shadowLayer.Layer.ShadowOpacity = 0.5f;
            _shadowLayer.Layer.MasksToBounds = false;
            _shadowLayer.Layer.BorderColor = UIColor.LightGray.CGColor;
            _shadowLayer.Layer.BorderWidth = 0.1f;

            Layer.MasksToBounds = true;
        }

        public override void MovedToSuperview()
        {
            if (_isAdded) return;
            Superview.InsertSubviewBelow(this, _shadowLayer);
            _isAdded = true;
        }

        protected void DrawBorder(CGRect rect, nfloat radius)
        {
            _shadowLayer.Frame = rect;
            _shadowLayer.Layer.ShadowRadius = radius;
            _shadowLayer.Layer.CornerRadius = radius;

            Frame = new UIEdgeInsets(20, 40, 40, 20).InsetRect(rect);
            Layer.CornerRadius = radius;
        }
    }
}