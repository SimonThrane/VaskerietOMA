﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class WashTime
    {
        public int ID { get; set; }

        public DateTime Time { get; set; }

        public bool IsBooked { get; set; }
        
        public string Machine { get; set; }
    }



}