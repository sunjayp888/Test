using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpStoxWindows.Helper
{
    public class Stock
    {
        public string Exchange { get; set; }
        public string Symbol { get; set; }
        public DateTime DateTime { get; set; }

        //   public string Date { get; set; }
        //   public string Time { get; set; }
        public string Open { get; set; }
        public string Close { get; set; }
        public string Volume { get; set; }
        public string CP { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Call { get; set; }
    }
}
