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

##Contributions / Thanks##

 - [@MarcBruins](https://github.com/MarcBruins)  for: [MBAutoComplete](https://github.com/MarcBruins/MBAutoComplete)
 - [@gshackles](https://github.com/gshackles) for: [FloatLabeledEntry](https://github.com/gshackles/FloatLabeledEntry)

##License##

Licensed MTI, please review the [license](https://github.com/XamFormsExtended/Xfx.Controls/blob/master/LICENSE) file.
