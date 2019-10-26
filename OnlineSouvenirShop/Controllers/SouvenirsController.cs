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
    public class SouvenirsController : ApiController
    {
        private OnlineSouvenirShopContext db = new OnlineSouvenirShopContext();

        // GET: api/Souvenirs
        public IQueryable<Souvenir> GetSouvenirs()
        {
            return db.Souvenirs.Where(c => c.Deleted == false).OrderByDescending(c => c.Timestamp);
        }

        [Route("api/SouvenirsFront")]
        [HttpGet]
        public IQueryable<Souvenir> GetSouvenirsFront()
        {
            return db.Souvenirs.Where(c => c.Deleted == false).OrderByDescending(c => c.Timestamp).Take(3);
        }

        [Route("api/SouvenirsByCategory/{id}")]
        [HttpGet]
        public IQueryable<Souvenir> GetSouvenirsByCategory(int id)
        {
            return db.Souvenirs.Where(c => c.Deleted == false && c.Category == id).OrderByDescending(c => c.Timestamp);
        }
        [Route("api/SouvenirsOnSale")]
        [HttpGet]
        public IQueryable<Souvenir> GetSouvenirsOnSale()
        {
            return db.Souvenirs.Where(c => c.Deleted == false && c.Sale == true).OrderByDescending(c => c.Timestamp);
        }

        [Route("api/SetRecommended")]
        [HttpPost]
        public IHttpActionResult SetReccomended(ChbInfoModel chbmodel)
        {
            Souvenir souvenir = (from c in db.Souvenirs where c.Id == chbmodel.ChbCheckedId select c).FirstOrDefault();
            if (chbmodel.ChbChecked)
                souvenir.Recommended = true;
            else souvenir.Recommended = false;
            db.SaveChanges();
            return Ok();
        }

        [Route("api/RecommendedSouvenirs")]
        [HttpGet]
        public IQueryable<Souvenir> GetRecommendedSouvenirs()
        {
            return db.Souvenirs.Where(c => c.Deleted == false && c.Recommended == true).OrderByDescending(c => c.Timestamp);
        }

        // GET: api/Souvenirs/5
        [ResponseType(typeof(Souvenir))]
        public async Task<IHttpActionResult> GetSouvenir(int id)
        {
            Souvenir souvenir = await db.Souvenirs.FindAsync(id);
            if (souvenir == null)
            {
                return NotFound();
            }

            return Ok(souvenir);
        }

        // PUT: api/Souvenirs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSouvenir(int id, Souvenir souvenir)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != souvenir.Id)
            {
                return BadRequest();
            }
            souvenir.Timestamp = DateTime.Now;
            db.Entry(souvenir).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SouvenirExists(id))
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

        // POST: api/Souvenirs
        [ResponseType(typeof(Souvenir))]
        public async Task<IHttpActionResult> PostSouvenir(Souvenir souvenir)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            souvenir.Timestamp = DateTime.Now;
            db.Souvenirs.Add(souvenir);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = souvenir.Id }, souvenir);
        }

        // DELETE: api/Souvenirs/5
        [ResponseType(typeof(Souvenir))]
        public async Task<IHttpActionResult> DeleteSouvenir(int id)
        {
            Souvenir souvenir = await db.Souvenirs.FindAsync(id);
            if (souvenir == null)
            {
                return NotFound();
            }

            db.Souvenirs.Remove(souvenir);
            await db.SaveChangesAsync();

            return Ok(souvenir);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SouvenirExists(int id)
        {
            return db.Souvenirs.Count(e => e.Id == id) > 0;
        }
    }
    public class ChbInfoModel
    {
        public bool ChbChecked { get; set; }
        public int ChbCheckedId { get; set; }
    }
}