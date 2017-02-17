using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;

namespace VaskerietOMA.ViewModel
{
    public class TopListViewModel
    {
        public List<TopListEntry> Entries { get; set; }
        public string Name { get; set; }

        public TopListViewModel()
        {
            Name = "SimonTest";
            Entries=new List<TopListEntry>();
        }

    }

    public class TopListEntry
    {
        public int RoomNumber { get; set; }
        public int Count { get; set; }

    }
}