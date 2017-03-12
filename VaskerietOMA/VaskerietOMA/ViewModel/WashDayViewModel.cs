using System.Collections.Generic;
using System.Linq;
using MyModel.WashTime;

namespace VaskerietOMA.ViewModel
{
    public class WashDayViewModel
    {
        public List<WashTimeViewModel> Left;
        public List<WashTimeViewModel> Right;

        public WashDayViewModel()
        {
        }

        public WashDayViewModel(List<WashTime> times)
        {
            Left = times.Where(m => m.Machine == "Left").OrderBy(c => c.Time.Hour).ToList().Select(c => new WashTimeViewModel(c)).ToList();
            Right = times.Where(m => m.Machine == "Right").OrderBy(c => c.Time.Hour).ToList().Select(c => new WashTimeViewModel(c)).ToList();


        }
    }
}