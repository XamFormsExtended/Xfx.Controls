using System;
using Xamarin.Forms;

namespace Xfx.Behaviors
{
    public class BindableBehavior<T> : Behavior<T> where T: BindableObject
    {
        protected override void OnAttachedTo(BindableObject bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.BindingContextChanged += OnBindingContextChanged(bindable);
        }

        protected override void OnDetachingFrom(BindableObject bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged(bindable);
        }

        private EventHandler OnBindingContextChanged(BindableObject bindable)
        {
            return delegate { BindingContext = bindable.BindingContext; };
        }
    }
}