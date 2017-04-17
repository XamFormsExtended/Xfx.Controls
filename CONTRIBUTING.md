## Contribution Guidelines

-----

First of all, thank you for your willingness to contribute to Xfx.Controls. This is a fun little project that simply adds a little flare to some of the existing Xamarn Forms controls.

### Coding Style

The coding style is quite simple. Please use the Visual Studio defauls for everything (SPACES NOT TABS), and also if you're running Resharper, try your best to get a green checkmark on every file you touch. R#'s default formatting can be acheived on a per-file basis by running `<kbd>ctrl</kbd> + <kbd>alt</kbd> + <kbd>shift</kbd> + <kbd>f</kbd>.

### Renderer Approach

It's really easy to make a mess of custom renderers. The best way to keep your renderers clean and supported is to follow the style used in the [Xamarin.Forms](https://github.com/Xamarin/Xamarin.Forms) repository.

**Focused / Unfocused**  
When building a custom renderer from scratch (`ViewRenderer<TView, TNativeView>`), you'll have to ensure you use the reflection approach to raising the focused/unfocused event.*

```csharp
var isFocusedPropertyKey = Element.GetInternalField<BindablePropertyKey>("IsFocusedPropertyKey");
ElementController.SetValueFromRenderer(isFocusedPropertyKey, true/false);
``` 

**Events**  
A personal preference of mine is to not use the MessagingCenter. Instead I subscribe to `internal` events within the core library (these events are exposed to the other platforms via `InternalsVisibleTo`).
Please avoid using the MessagingCenter in favor of event subscriptions.

DON'T FORGET TO UNSUBSCRIBE FROM EVENTS

### Spelling, Documentation, Comments

I'm absolutely open to taking Pull Requests just to fix spelling, documentation, or comments, providing they add value. 

 - Please avoid unnecessary whitespace commits
 - Please don't fix spelling at the expense of breaking the API. Use the `[ObsoleteAttribute]` if you find a method, property, or enum value that is spelled incorrectly, and add the correct spelling beside it.