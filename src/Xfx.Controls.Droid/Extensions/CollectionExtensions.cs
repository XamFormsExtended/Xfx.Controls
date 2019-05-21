using System.Collections;
using ArrayList = Java.Util.ArrayList;

namespace Xfx.Controls.Droid.Extensions
{
    public static class CollectionExtensions
    {    
        [Android.Runtime.Preserve (AllMembers = true)]
        public static ArrayList ToArrayList(this ICollection input)
        {
            return input.Count == 0 ? new ArrayList() : new ArrayList(input);
        }
    }
}