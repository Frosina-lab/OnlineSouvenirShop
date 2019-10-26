using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineSouvenirShop.Models
{
    public class BoughtSouvenirs
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserAddress { get; set; }
        public int SouvenirId { get; set; }
        public bool InBucket { get; set; }
        public bool Buy { get; set; }
        public bool Delivered { get; set; }

        public virtual Souvenir Souvenir { get; set; }
    }
}