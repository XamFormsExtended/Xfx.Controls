using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace Xfx.Controls.iOS.Extensions
{
    public static class UIViewExtensions
    {
        // literally ripped this off from X.Forms source.
        public static T FindDescendantView<T>(this UIView view) where T : UIView
        {
            var queue = new Queue<UIView>();
            queue.Enqueue(view);

            while (queue.Count > 0)
            {
                var descendantView = queue.Dequeue();

                var result = descendantView as T;
                if (result != null)
                    return result;

                for (var i = 0; i < descendantView.Subviews.Length; i++)
                    queue.Enqueue(descendantView.Subviews[i]);
            }

            return null;
        }
        public static UIWindow GetTopWindow(this UIApplication app)
        {
            return app.Windows.Reverse().FirstOrDefault(x => x.WindowLevel == UIWindowLevel.Normal && !x.Hidden);
        }

        public static UIView GetTopView(this UIApplication app)
        {
            return app.GetTopWindow().Subviews.Last();
        }

        public static UIViewController GetTopViewController(this UIApplication app)
        {
            var viewController = app.KeyWindow.RootViewController;
            while (viewController.PresentedViewController != null)
                viewController = viewController.PresentedViewController;

            return viewController;
        }
    }
}