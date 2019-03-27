using System;
using Xamarin.Forms;
using Xfx.Behaviors;

namespace Xfx
{
    public class XfxBinding
    {
        public static readonly BindableProperty ValidatesOnDataErrorsProperty =  BindableProperty.CreateAttached(
            "ValidatesOnDataErrors", 
            typeof(bool), 
            typeof(XfxEntry), 
            false, propertyChanged: OnValidatesOnDataErrorsChanged);

        public static bool GetValidatesOnDataErrors(BindableObject view)
        {
            return (bool)view.GetValue(ValidatesOnDataErrorsProperty);
        }

        public static void SetValidatesOnDataErrors(BindableObject view, bool value)
        {
            view.SetValue(ValidatesOnDataErrorsProperty, value);
        }

        private static void OnValidatesOnDataErrorsChanged(BindableObject view, object oldvalue, object newvalue)
        {
            if (!Convert.ToBoolean(newvalue)||!(view is IXfxValidatableElement)) return;

            var binding = view.GetBinding(Entry.TextProperty);
            var boundPropertyName = binding.Path;

            ((VisualElement)view).Behaviors.Add(new ValidatesOnNotifyDataErrors{PropertyName = boundPropertyName});
        }
    }
}