using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using OnlineSouvenirShop.Models;

namespace OnlineSouvenirShop.Controllers
{
    public class BoughtSouvenirsController : ApiController
    {
        private OnlineSouvenirShopContext db = new OnlineSouvenirShopContext();

        // GET: api/BoughtSouvenirs
        public IQueryable<BoughtSouvenirs> GetBoughtSouvenirs()
        {
            return db.BoughtSouvenirs;
        }

        // GET: api/BoughtSouvenirs/5
        [ResponseType(typeof(BoughtSouvenirs))]
        public async Task<IHttpActionResult> GetBoughtSouvenirs(int id)
        {
            BoughtSouvenirs boughtSouvenirs = await db.BoughtSouvenirs.FindAsync(id);
            if (boughtSouvenirs == null)
            {
                return NotFound();
            }

            return Ok(boughtSouvenirs);
        }

        // PUT: api/BoughtSouvenirs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBoughtSouvenirs(int id, BoughtSouvenirs boughtSouvenirs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != boughtSouvenirs.Id)
            {
                return BadRequest();
            }

            db.Entry(boughtSouvenirs).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoughtSouvenirsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/BoughtSouvenirs
        [ResponseType(typeof(BoughtSouvenirs))]
        public async Task<IHttpActionResult> PostBoughtSouvenirs(BoughtSouvenirsView souvenir)
        {

            BoughtSouvenirs boughtSouvenir = db.BoughtSouvenirs.Where(c => c.Id == souvenir.Id && c.UserName == souvenir.Name).FirstOrDefault();
            boughtSouvenir.InBucket = false;
            boughtSouvenir.UserAddress = souvenir.Address;
            boughtSouvenir.Souvenir.NumSamples--;

            await db.SaveChangesAsync();

            return Ok();
        }

        [Route("api/AddToBucket")]
        [HttpPost]
        public IHttpActionResult AddToBucket(BoughtSouvenirs souvenir)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BoughtSouvenirs.Add(souvenir);
            db.SaveChanges();
            int souvenirsInBucket = (from c in db.BoughtSouvenirs where c.InBucket == true select c).Count();
            return Ok(souvenirsInBucket);
        }

        [Route("api/GetSouvenirsFromBucket/{name}")]
        [HttpGet]
        public IHttpActionResult GetSouvenirsFromBucket(string name)
        {
            List<BoughtSouvenirs> souvenirsInBucket = (from c in db.BoughtSouvenirs where c.InBucket == true && c.UserName == name select c).ToList();
            List<Souvenir> souvenirs = (from c in souvenirsInBucket select c.Souvenir).ToList();
            int? souvenirPrices = 0;
            foreach (var souvenir in souvenirs)
            {
                if (souvenir.NewPrice != null) souvenirPrices += souvenir.NewPrice;
                else souvenirPrices += souvenir.Price;
            }
            List<SouvenirInBucketDetails> souvenirsInBucketView = (from c in souvenirsInBucket
                                                           select new SouvenirInBucketDetails
                                                           {
                                                               Id = c.Id,
                                                               SouvenirId = c.Souvenir.Id,
                                                               Title = c.Souvenir.Title,
                                                               Folder = c.Souvenir.Folder,
                                                               Price = c.Souvenir.Price,
                                                               Sale = c.Souvenir.Sale,
                                                               NewPrice = c.Souvenir.NewPrice,
                                                               AllPrices = souvenirPrices
                                                           }).ToList();
            return Ok(souvenirsInBucketView);
        }

        [Route("api/RemoveSouvenirFromBucket/{id}")]
        [HttpGet]
        public IHttpActionResult RemoveSouvenirFromBucket(int id)
        {
            BoughtSouvenirs souvenir = (from c in db.BoughtSouvenirs where c.SouvenirId == id select c).FirstOrDefault();
            db.BoughtSouvenirs.Remove(souvenir);
            db.SaveChanges();
            int souvenirsInBucket = (from c in db.BoughtSouvenirs where c.InBucket == true select c).Count();
            return Ok(souvenirsInBucket);
        }
        [Route("api/GetBoughtSouvenirs")]
        [HttpGet]
        public List<BoughtSouvenirsResult> GetNotDeliveredBooks()
        {
            List<BoughtSouvenirsResult> result = (from c in db.BoughtSouvenirs
                                              where c.Buy == true && c.Delivered == false
                                              group c by c.UserName into g
                                              select new BoughtSouvenirsResult
                                              {
                                                  UserName = g.Key,
                                                  UserAddress = g.FirstOrDefault().UserAddress,
                                                  UserSurname = g.FirstOrDefault().UserSurname,
                                                  BoughtSouvenirs = g.Select(b => b.Souvenir.Title).ToList()
                                              }).ToList();
            return result;
        }

        [Route("api/SetDelivered/{name}")]
        [HttpPost]
        public IHttpActionResult SetDeliveredSouvenirs(string name)
        {
            List<BoughtSouvenirs> souvenirs = (from c in db.BoughtSouvenirs
                                       where
              c.UserName == name && c.InBucket == false && c.Buy == true
                                       select c).ToList();
            foreach (var souvenir in souvenirs)
                souvenir.Delivered = true;
            db.SaveChanges();
            return Ok(true);
        }

        [Route("api/GetDeliveredSouvenirs")]
        [HttpGet]
        public List<BoughtSouvenirsResult> GetDeliveredBooks()
        {
            List<BoughtSouvenirsResult> result = (from c in db.BoughtSouvenirs
                                              where c.Delivered == true
                                              group c by c.UserName into g
                                              select new BoughtSouvenirsResult
                                              {
                                                  UserName = g.Key,
                                                  UserAddress = g.FirstOrDefault().UserAddress,
                                                  UserSurname = g.FirstOrDefault().UserSurname,
                                                  BoughtSouvenirs = g.Select(b => b.Souvenir.Title).ToList()
                                              }).ToList();
            return result;
        }

        // DELETE: api/BoughtSouvenirs/5
        [ResponseType(typeof(BoughtSouvenirs))]
        public async Task<IHttpActionResult> DeleteBoughtSouvenirs(int id)
        {
            BoughtSouvenirs boughtSouvenirs = await db.BoughtSouvenirs.FindAsync(id);
            if (boughtSouvenirs == null)
            {
                return NotFound();
            }

            db.BoughtSouvenirs.Remove(boughtSouvenirs);
            await db.SaveChangesAsync();

            return Ok(boughtSouvenirs);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BoughtSouvenirsExists(int id)
        {
            return db.BoughtSouvenirs.Count(e => e.Id == id) > 0;
        }
    }

    public class SouvenirInBucketDetails
    {
        public int Id { get; set; }
        public int SouvenirId { get; set; }
        public string Title { get; set; }
        public string Folder { get; set; }
        public int Price { get; set; }
        public bool Sale { get; set; }
        public int? NewPrice { get; set; }
        public int? AllPrices { get; set; }
    }

    public class BoughtSouvenirsView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class BoughtSouvenirsResult
    {
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserAddress { get; set; }
        public List<string> BoughtSouvenirs { get; set; }
    }
}