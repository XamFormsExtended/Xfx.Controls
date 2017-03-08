#Xamarin Forms Extended Controls#

*Description*
Xfx Controls are just a few controls that differ from the baked in Xamarin.Forms Controls.

|               | iOS                | Android            | UWP | Mac |
| ------------- |:------------------:|:------------------:|:---:|:---:|
| XfxEntry      | :white_check_mark: | :white_check_mark: | :x: | :x: |
| XfxComboBox   | :white_check_mark: | :white_check_mark: | :x: | :x: |

-----

 - [Getting Started](#getting-started)
     - [Android](#android)
	 - [iOS](#ios)
 - [Demos](#demos)
 - [Code](#code)
     - [XfxEntry](#xfxentry)
     - [XfxComboBox](#xfxcombobox)
 - [Contributions / Thanks](#contributions--thanks)
 - [License](#license)

-----

##Getting Started##

###Android###

In your MainActivity, initialize XfxControls just before initializing Xamarin Forms

```csharp
XfxControls.Init();
global::Xamarin.Forms.Forms.Init(this, bundle);
```

*note: XfxEntry and XfxComboBox REQUIRES your app to use an AppCompat theme.*

###iOS###

In your AppDelegate, initialize XfxControls just before initializing Xamarin Forms

```csharp
XfxControls.Init();
global::Xamarin.Forms.Forms.Init();
```

###Demos###

![](https://github.com/XamFormsExtended/Xfx.Controls/raw/master/resources/xfx.controls.ios.gif)
![](https://github.com/XamFormsExtended/Xfx.Controls/raw/master/resources/xfx.controls.droid.gif)

###Code###

Declaration is exactly the same as a Xamarin.Forms.Entry, with some added properties

####XfxEntry####

```xml
<!-- XfxEntry-->
<xfx:XfxEntry Placeholder="Enter your name"
              Text="{Binding Name}"
              ErrorText="{Binding NameErrorText}" />
```

When the `ErrorText` property is set, the ErrorText will display, otherwise if it is null or empty, it's removed.

####XfxComboBox####

```xml
<!-- XfxComboBox-->
<xfx:XfxComboBox Placeholder="Enter your email address"
                Text="{Binding EmailAddress}"
                ItemsSource="{Binding EmailSuggestions}"
                SortingAlgorithm="{Binding SortingAlgorithm}"/>
```
The XfxComboBox extends the XfxEntry and therefore also includes the `ErrorText` property.  
Beyond that there is an `ItemsSource` property, `SelectedItem` property, and a `SortingAlgorithm` property.  
The first two are pretty self explanitory, but here's an example of how you can set the `SortingAlgorithm`

```csharp
public class MyViewModel : INotifyPropertyChanged
{
	public Func<string, ICollection<string>, ICollection<string>> SortingAlgorithm { get; } = (text, values) => values
		.Where(x => x.ToLower().StartsWith(text.ToLower()))
		.OrderBy(x => x)
		.ToList();
}
```


##Contributions / Thanks##

 - [@MarcBruins](https://github.com/MarcBruins)  for: [MBAutoComplete](https://github.com/MarcBruins/MBAutoComplete)
 - [@gshackles](https://github.com/gshackles) for: [FloatLabeledEntry](https://github.com/gshackles/FloatLabeledEntry)

##License##

Licensed MIT, please review the [license](https://github.com/XamFormsExtended/Xfx.Controls/blob/master/LICENSE) file.
