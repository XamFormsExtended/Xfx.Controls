using System;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xfx.Controls.iOS
{
    public class XfxVisualElementManager : IDisposable
    {
        private EventTracker _events;
        private VisualElementRendererFlags _flags = VisualElementRendererFlags.AutoPackage | VisualElementRendererFlags.AutoTrack;
        private VisualElementPackager _packager;
        private VisualElementTracker _tracker;

        public bool AutoPackage
        {
            get => (_flags & VisualElementRendererFlags.AutoPackage) != 0;
            set
            {
                if (value)
                    _flags |= VisualElementRendererFlags.AutoPackage;
                else
                    _flags &= ~VisualElementRendererFlags.AutoPackage;
            }
        }

        public bool AutoTrack
        {
            get => (_flags & VisualElementRendererFlags.AutoTrack) != 0;
            set
            {
                if (value)
                    _flags |= VisualElementRendererFlags.AutoTrack;
                else
                    _flags &= ~VisualElementRendererFlags.AutoTrack;
            }
        }

        public void Dispose()
        {
            if (_events != null)
            {
                _events.Dispose();
                _events = null;
            }
            if (_tracker != null)
            {
                _tracker.Dispose();
                _tracker = null;
            }
            if (_packager != null)
            {
                _packager.Dispose();
                _packager = null;
            }
        }

        public void Init(IVisualElementRenderer renderer)
        {
            _tracker = new VisualElementTracker(renderer);
            _tracker.NativeControlUpdated += OnNativeControlUpdated;

            _packager = new VisualElementPackager(renderer);
            _events = new EventTracker(renderer);

            if (AutoPackage)
                _packager?.Load();

            if (AutoTrack)
                if (renderer is UIView uiView)
                    _events.LoadEvents(uiView);
                else
                    throw new InvalidRendererException("IVisualElementRenderer must be of type 'UIView'");
        }

        protected virtual void UpdateNativeWidget()
        {
        }

        private void OnNativeControlUpdated(object sender, EventArgs e) => UpdateNativeWidget();
    }
}