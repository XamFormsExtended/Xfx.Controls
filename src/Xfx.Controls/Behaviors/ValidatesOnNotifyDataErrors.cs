using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace Xfx.Behaviors
{
    public class ValidatesOnNotifyDataErrors : BindableBehavior<BindableObject>
    {
        private IXfxValidatableElement _element;

        /// <summary>
        /// Gets or sets the property name used for validation
        /// </summary>
        public string PropertyName { private get; set; }

        protected override void OnAttachedTo(BindableObject bindable)
        {
            base.OnAttachedTo(bindable);
            _element = (IXfxValidatableElement)bindable;
            bindable.BindingContextChanged += BindableOnBindingContextChanged;
        }

        protected override void OnDetachingFrom(BindableObject bindable)
        {
            base.OnDetachingFrom(bindable);
            var vm = bindable.BindingContext as INotifyDataErrorInfo;
            if (vm != null)
            {
                vm.ErrorsChanged -= ModelOnErrorsChanged;
            }
            bindable.BindingContextChanged -= BindableOnBindingContextChanged;
            _element = null;
        }

        private void BindableOnBindingContextChanged(object sender, EventArgs eventArgs)
        {
            var element = (BindableObject)sender;
            var vm = element.BindingContext as INotifyDataErrorInfo;
            if (vm != null)
            {
                vm.ErrorsChanged += ModelOnErrorsChanged;
            }
        }

        private void ModelOnErrorsChanged(object sender, DataErrorsChangedEventArgs args)
        {
            var model = (INotifyDataErrorInfo)sender;
            if(string.IsNullOrWhiteSpace(PropertyName)) throw new ArgumentNullException($"{nameof(PropertyName)} cannot be null");
            if (args.PropertyName != PropertyName) return;

            var error = model.GetErrors(args.PropertyName)?.Cast<string>().First();
            _element.ErrorText = error;
        }
    }
}