using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace Xfx.Controls.Example
{
    public class _MainPage : ContentPage
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text),
            typeof (string),
            typeof (App),
            default(string),
            propertyChanged: TextPropertyChanged);

        public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText),
            typeof (string),
            typeof (App),
            default(string));

        public static readonly BindableProperty EmailSuggestionsProperty = BindableProperty.Create(nameof(EmailSuggestions),
            typeof (ObservableCollection<string>),
            typeof (_MainPage),
            new ObservableCollection<string>());

        public _MainPage()
        {
            // yeah yeah, you should probably have a separate ViewModel/PageModel, 
            // but for the sake of simplicity I'll use this
            BindingContext = this;

            var combo = new XfxComboBox
            {
                Placeholder = "Enter your email address",
                TextColor = Color.Yellow
            };

            combo.ItemSelected += delegate { DisplayAlert($"You Selected {Text}", "well done!", "close"); };

            var entry = new XfxEntry
            {
                Placeholder = "Normal Entry",
                TextColor = Color.White
            };

            // BINDINGS
            // entry bindings
            entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text)));

            // combobox bindings
            combo.SetBinding(Entry.TextProperty, new Binding(nameof(Text)));
            combo.SetBinding(XfxComboBox.SortingAlgorithmProperty, new Binding(nameof(SortingAlgorithm)));
            combo.SetBinding(XfxEntry.ErrorTextProperty, new Binding(nameof(ErrorText)));
            combo.SetBinding(XfxComboBox.ItemsSourceProperty, new Binding(nameof(EmailSuggestions)));

            Content = new ScrollView
            {
                Padding = 8,
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {combo, entry}
                }
            };
        }

        // you can customize your sorting algorithim to however you want it to work.
        private Func<string, ICollection<string>, ICollection<string>> SortingAlgorithm { get; } = (text, values) => values
            .Where(x => x.ToLower().StartsWith(text.ToLower()))
            .OrderBy(x => x)
            .ToList();

        /// <summary>
        ///     Email Suggestions collection . This is a bindable property.
        /// </summary>
        public ObservableCollection<string> EmailSuggestions
        {
            get { return (ObservableCollection<string>) GetValue(EmailSuggestionsProperty); }
            set { SetValue(EmailSuggestionsProperty, value); }
        }

        /// <summary>
        ///     Error Text . This is a bindable property.
        /// </summary>
        public string ErrorText
        {
            get { return (string) GetValue(ErrorTextProperty); }
            set { SetValue(ErrorTextProperty, value); }
        }

        /// <summary>
        /// Text . This is a bindable property.
        /// </summary>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void TextPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var app = (_MainPage) bindable;
            // make sure we have the latest string.
            var text = newvalue.ToString();

            // don't validate like this, only for demo purposes.
            app.ErrorText = string.IsNullOrEmpty(text) ? "Text cannot be empty" : "";

            // here's a list of email address suffixes that we're going to use
            var emails = new[]
            {
                "@gmail.com",
                "@hotmail.com",
                "@me.com",
                "@outlook.com"
            };

            // if the text is empty or already contains an @ symbol, don't update anything.
            if (string.IsNullOrEmpty(text) || text.Contains("@")) return;

            /*
            note, you could use some sort of FastObservableCollection if 
            you want the notification to only happen a single time.
            i'm not doing that here because I'm trying to K.I.S.S this example
            for reference: http://stackoverflow.com/a/13303245/124069
           */

            // clear the old suggestions, you're starting over. This also can be more efficient, 
            // I'll leave that for you to figure out.
            app.EmailSuggestions.Clear();

            // side note: for loops will add a tiny performance boost over foreach
            for (var i = 0; i < emails.Length; i++)
            {
                // add the new suggestion to your collection.
                app.EmailSuggestions.Add($"{text}{emails[i]}");
            }
        }
    }
}