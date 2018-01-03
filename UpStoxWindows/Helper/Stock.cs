using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;

namespace UpStoxWindows.Helper
{
    public class Stock
    {
        public string Exchange { get; set; }
        public string Symbol { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        //   public string Date { get; set; }
        //   public string Time { get; set; }
        public string Open { get; set; }
        public string Close { get; set; }
        public string Call { get; set; }
        public string Status { get; set; }
        public string Volume { get; set; }
        public string CP { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public double BuyPrice { get; set; }
        public double SellPrice { get; set; }

        public string StockString { get; set; }
        public double NetQuantity { get; set; }
        public double Profit { get; set; }
    }
}
