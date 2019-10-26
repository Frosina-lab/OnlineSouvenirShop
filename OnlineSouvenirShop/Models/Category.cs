using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineSouvenirShop.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Deleted { get; set; }
    }
}