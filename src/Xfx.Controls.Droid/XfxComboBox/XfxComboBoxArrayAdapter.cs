using System;
using System.Collections.Generic;
using Android.Content;
using Android.Widget;

namespace Xfx.Controls.Droid.XfxComboBox
{
    internal class XfxComboBoxArrayAdapter : ArrayAdapter
    {
        private readonly IList<string> _objects;
        private readonly Func<string, ICollection<string>, ICollection<string>> _sortingAlgorithm;

        public XfxComboBoxArrayAdapter(
            Context context,
            int textViewResourceId,
            List<string> objects,
            Func<string, ICollection<string>, ICollection<string>> sortingAlgorithm) : base(context, textViewResourceId, objects)
        {
            _objects = objects;
            _sortingAlgorithm = sortingAlgorithm;
        }
        
        public override Filter Filter
        {
            get
            {
                return new XfxComboBoxFilter(_sortingAlgorithm) { Adapter = this, Originals = _objects };
            }
        }
    }
}