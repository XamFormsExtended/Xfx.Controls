using System.Reflection;
using Xamarin.Forms;

namespace Xfx.Extensions
{
    public static class XamarinFormsExtensions
    {
        public static BindablePropertyKey GetInternalPropertyKey(this BindableObject element, string propertyKeyName)
        {
            // reflection stinks, but hey, what can you do?
            var pi = element.GetType().GetField(propertyKeyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var key = pi?.GetValue(element) as BindablePropertyKey;

            return key;
        }
    }
}