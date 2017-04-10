using System.Reflection;
using Xamarin.Forms;

namespace Xfx.Extensions
{
    public static class XamarinFormsExtensions
    {
        public static T GetInternalField<T>(this BindableObject element, string propertyKeyName) where T:class
        {
            // reflection stinks, but hey, what can you do?
            var pi = element.GetType().GetField(propertyKeyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var key = (T) pi?.GetValue(element);

            return key;
        }
    }
}