using System;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xfx.Controls.iOS
{
    /// <summary>
    /// Used to add Gesture Recognizers and other Forms features to a Renderer.
    /// </summary>
    public class XfxVisualElementManager : IDisposable
    {
        private EventTracker _events;
        private VisualElementRendererFlags _flags;
        private VisualElementPackager _packager;
        private VisualElementTracker _tracker;

        public bool AutoPackage => (_flags & VisualElementRendererFlags.AutoPackage) != 0;

        public bool AutoTrack => (_flags & VisualElementRendererFlags.AutoTrack) != 0;

        protected bool IsDisposed => _flags.HasFlag(VisualElementRendererFlags.Disposed);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed || !disposing)
            {
                return;
            }

            _flags |= VisualElementRendererFlags.Disposed;

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

        public void Init(IVisualElementRenderer renderer, VisualElementRendererFlags flags = VisualElementRendererFlags.AutoPackage | VisualElementRendererFlags.AutoTrack)
        {
            _flags = flags;
            _packager = new VisualElementPackager(renderer);
            _events = new EventTracker(renderer);
            _tracker = new VisualElementTracker(renderer);
            _tracker.NativeControlUpdated += OnNativeControlUpdated;

            if (AutoTrack)
            {
                if (renderer is UIView uiView)
                    _events.LoadEvents(uiView);
                else
                    throw new InvalidRendererException("IVisualElementRenderer must be of type 'UIView'");
            }

            if (AutoPackage)
                _packager?.Load();
        }

        protected virtual void UpdateNativeWidget()
        {
        }

        private void OnNativeControlUpdated(object sender, EventArgs e) => UpdateNativeWidget();
    }
}