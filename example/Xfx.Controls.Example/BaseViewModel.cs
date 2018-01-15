using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Xfx.Controls.Example
{
    public abstract class BaseViewModel : BindableObject, INotifyDataErrorInfo
    {
        public static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly(nameof(IsValid),
            typeof(bool),
            typeof(BaseViewModel),
            default(bool));

        protected readonly Dictionary<string, IList<string>> Errors = new Dictionary<string, IList<string>>();

        /// <summary>
        ///     ReadOnly IsValid summary. This is a bindable property.
        /// </summary>
        public bool IsValid
        {
            get => (bool) GetValue(IsValidPropertyKey.BindableProperty);
            set => SetValue(IsValidPropertyKey, value);
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            IList<string> errorsForName;
            Errors.TryGetValue(propertyName, out errorsForName);
            return errorsForName;
        }

        public bool HasErrors => Errors.Any(kv => kv.Value != null && kv.Value.Count > 0);


        public abstract void ValidateProperty([CallerMemberName] string propertyName = null);

        protected void RaiseErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}