using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xfx;

namespace MBAutoComplete
{
    public abstract class MbAutoCompleteViewSource : UITableViewSource
    {
        public ICollection<string> Suggestions { get; set; } = new List<string>();

        public MbAutoCompleteTextField AutoCompleteTextField { get; set; }

        public abstract void UpdateSuggestions(ICollection<string> suggestions);

        public abstract override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath);

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Suggestions.Count;
        }

        public event EventHandler<XfxSelectedItemChangedEventArgs> Selected;

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            AutoCompleteTextField.Text = Suggestions.ElementAt(indexPath.Row);
            AutoCompleteTextField.AutoCompleteTableView.Hidden = true;
            AutoCompleteTextField.ResignFirstResponder();

            int index = (int)indexPath.Item;
            var item = Suggestions.ToList()[index];

            Selected?.Invoke(tableView, new XfxSelectedItemChangedEventArgs(item, index));
            // don't call base.RowSelected
        }
    }
}