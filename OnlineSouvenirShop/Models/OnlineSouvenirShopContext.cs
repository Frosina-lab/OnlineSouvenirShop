using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace OnlineSouvenirShop.Models
{
    public class OnlineSouvenirShopContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public OnlineSouvenirShopContext() : base("name=OnlineSouvenirShopContext")
        {
        }

        public System.Data.Entity.DbSet<OnlineSouvenirShop.Models.Souvenir> Souvenirs { get; set; }

        public System.Data.Entity.DbSet<OnlineSouvenirShop.Models.Category> Categories { get; set; }

        public System.Data.Entity.DbSet<OnlineSouvenirShop.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<OnlineSouvenirShop.Models.BoughtSouvenirs> BoughtSouvenirs { get; set; }
    }
}