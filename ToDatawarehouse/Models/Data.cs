using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    class Data
    {
        public int temperature { get; set; }
        public string tempunit { get; set; }
        public int humidity { get; set; }
        public string humidityunit { get; set; }
        public long logtime { get; set; }

        public bool tempAlert { get; set; }

    }
}
