using System;
using System.Collections.Generic;
using System.Linq;
using Android.Widget;
using Java.Lang;
using Java.Util;
using Xfx.Controls.Droid.Extensions;

namespace Xfx.Controls.Droid.XfxComboBox
{
    internal class XfxComboBoxFilter : Filter
    {
        private readonly Func<string, ICollection<string>, ICollection<string>> _sortingAlgorithm;

        public XfxComboBoxFilter(Func<string, ICollection<string>, ICollection<string>> sortingAlgorithm)
        {
            _sortingAlgorithm = sortingAlgorithm;
        }

        public XfxComboBoxArrayAdapter Adapter { private get; set; }
        public IList<string> Originals { get; set; }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            var results = new FilterResults();
            if (constraint == null || constraint.Length() == 0)
            {
                results.Values = Originals.ToList().ToArrayList();
                results.Count = Originals.Count;
            }
            else
            {
                var values = new ArrayList();
                var sorted = _sortingAlgorithm(constraint.ToString(), Originals).ToList();

                for (var index = 0; index < sorted.Count; index++)
                {
                    var item = sorted[index];
                    values.Add(item);
                }

                results.Values = values;
                results.Count = sorted.Count;
            }

            return results;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            if (results.Count == 0)
                Adapter.NotifyDataSetInvalidated();
            else
            {
                Adapter.Clear();
                var vals = (ArrayList)results.Values;
                foreach (var val in vals.ToArray())
                {
                    Adapter.Add(val);
                }
                Adapter.NotifyDataSetChanged();
            }
        }
    }
}