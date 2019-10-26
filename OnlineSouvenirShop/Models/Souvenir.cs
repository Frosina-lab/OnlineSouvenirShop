using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineSouvenirShop.Models
{
    public class Souvenir
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Category { get; set; }
        public int Price { get; set; }
        public bool Sale { get; set; }
        public bool Recommended { get; set; }
        public int? NewPrice { get; set; }
        public int NumSamples { get; set; }
        public string Folder { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Deleted { get; set; }
    }
}