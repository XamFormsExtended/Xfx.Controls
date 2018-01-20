using System;
using Xamarin.Forms;
using Xfx.Behaviors;

namespace Xfx
{
    public class XfxValidation
    {
        public static readonly BindableProperty ValidatesOnDataErrorsProperty =  BindableProperty.CreateAttached(
            "ValidatesOnDataErrors", 
            typeof(bool), 
            typeof(IXfxValidatableElement), 
            false, propertyChanged: OnValidatesOnDataErrorsChanged);

        public static bool GetValidatesOnDataErrors(BindableObject view) => (bool)view.GetValue(ValidatesOnDataErrorsProperty);

        public static void SetValidatesOnDataErrors(BindableObject view, bool value) => view.SetValue(ValidatesOnDataErrorsProperty, value);

        private static void OnValidatesOnDataErrorsChanged(BindableObject view, object oldvalue, object newvalue)
        {
            if (!Convert.ToBoolean(newvalue)||!(view is IXfxValidatableElement)) return;

            BindableProperty bp = null;
            switch (view.GetType().Name)
            {
                case nameof(XfxEntry):
                    bp = Entry.TextProperty;
                    break;
                case nameof(XfxPicker):
                    bp = Picker.SelectedItemProperty;
                    break;
            }

            var binding = view.GetBinding(bp);
            var boundPropertyName = binding.Path;

            ((VisualElement)view).Behaviors.Add(new ValidatesOnNotifyDataErrors{PropertyName = boundPropertyName});
        }
    }
}