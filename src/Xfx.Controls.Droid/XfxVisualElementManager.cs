using System;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using Xfx.Controls.Droid.Forms.Internals;

namespace Xfx.Controls.Droid
{
    [Android.Runtime.Preserve (AllMembers = true)]
    public class XfxVisualElementManager : IDisposable
    {
        private VisualElementPackager _packager;
        private VisualElementRendererFlags _flags;
        private VisualElementTracker _tracker;
        private GestureManager _gestureManager;

        public bool AutoPackage => (_flags & VisualElementRendererFlags.AutoPackage) != 0;

        public bool AutoTrack => (_flags & VisualElementRendererFlags.AutoTrack) != 0;

        protected bool IsDisposed => _flags.HasFlag(VisualElementRendererFlags.Disposed);
        public VisualElementTracker Tracker => _tracker;

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

        public void Init(IVisualElementRenderer renderer, VisualElementRendererFlags flags = VisualElementRendererFlags.AutoPackage|VisualElementRendererFlags.AutoTrack)
        {

            _flags = flags;
            _packager = new VisualElementPackager(renderer);
            _tracker = new VisualElementTracker(renderer);
            _gestureManager = new GestureManager(renderer);

            if (AutoPackage)
                _packager?.Load();
        }

        public void UpdateLayout()
        {
            if(AutoTrack)
                _tracker?.UpdateLayout();
        }

        public bool OnTouchEvent(MotionEvent motionEvent) => _gestureManager.OnTouchEvent(motionEvent);
    }
}