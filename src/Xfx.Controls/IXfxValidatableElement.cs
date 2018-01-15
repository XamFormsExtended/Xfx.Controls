using System;
using Xamarin.Forms;

namespace Xfx
{
    public interface IXfxValidatableElement
    {
        string ErrorText { get; set; }
        event EventHandler<TextChangedEventArgs> ErrorTextChanged;
    }
}