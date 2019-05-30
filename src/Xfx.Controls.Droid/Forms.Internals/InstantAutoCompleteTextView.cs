using Android.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;

namespace Xfx.Controls.Droid.Forms.Internals
{

    public class InstantAutoCompleteTextView : AppCompatAutoCompleteTextView
    {
        public bool ShowIfEmpty { get; set; }

        public InstantAutoCompleteTextView(Context context) : base(context)
        {
        }

        private void InstantAutoCompleteTextView_FocusChange(object sender, FocusChangeEventArgs e)
        {
            if (e.HasFocus)
                ShowDropDown();
        }

        public InstantAutoCompleteTextView(Context arg0, IAttributeSet arg1) : base(arg0, arg1) { }

        public InstantAutoCompleteTextView(Context arg0, IAttributeSet arg1, int arg2) : base(arg0, arg1, arg2) { }

        public void SetOpenOnFocus(bool openOnFocus)
        {
            if (openOnFocus) FocusChange += InstantAutoCompleteTextView_FocusChange;
            else FocusChange -= InstantAutoCompleteTextView_FocusChange;
        }

        public override bool EnoughToFilter()
        {
            return ShowIfEmpty || base.EnoughToFilter();
        }

        public void PerformFiltering()
        {
            PerformFiltering(Text, 0);
        }
    }
}