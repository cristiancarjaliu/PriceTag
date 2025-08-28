using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTag.App.Services
{
    public class Product
    {
        public string Name { get; set; } = "";
        public string Price { get; set; } = "";
        public string Barcode { get; set; } = "";
        public string Producer { get; set; } = "";
    }
}
