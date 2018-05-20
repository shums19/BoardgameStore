using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BoardgameStore.Models;

namespace BoardgameStore.Controllers
{
    public class HomeController : Controller
    {
        private BoardgameStoreDBEntities db = new BoardgameStoreDBEntities();

        // GET: Boardgames
        public ActionResult Catalog()
        {
            var boardgame = db.Boardgame.Include(b => b.Author).Include(b => b.Complexity).Include(b => b.Publisher);
            return View(boardgame.ToList());
        }

        // GET: Boardgames/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boardgame boardgame = db.Boardgame.Find(id);
            if (boardgame == null)
            {
                return HttpNotFound();
            }
            return View(boardgame);
        }

        // GET: Boardgames/Create
        public ActionResult Create()
        {
            ViewBag.author_id = new SelectList(db.Author, "author_id", "author_name");
            ViewBag.complexity_id = new SelectList(db.Complexity, "complexity_id", "complexity_name");
            ViewBag.publisher_id = new SelectList(db.Publisher, "publisher_id", "publisher_name");
            return View();
        }

        // POST: Boardgames/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "boardgame_name,min_time,max_time,min_players,max_players,cover_path,years,language,boardgame_number,boardgame_cost,independence,images_paths,preview_description,full_description,boardgame_equipment,boardgame_id,author_id,publisher_id,complexity_id")] Boardgame boardgame)
        {
            if (ModelState.IsValid)
            {
                db.Boardgame.Add(boardgame);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.author_id = new SelectList(db.Author, "author_id", "author_name", boardgame.author_id);
            ViewBag.complexity_id = new SelectList(db.Complexity, "complexity_id", "complexity_name", boardgame.complexity_id);
            ViewBag.publisher_id = new SelectList(db.Publisher, "publisher_id", "publisher_name", boardgame.publisher_id);
            return View(boardgame);
        }

        // GET: Boardgames/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boardgame boardgame = db.Boardgame.Find(id);
            if (boardgame == null)
            {
                return HttpNotFound();
            }
            ViewBag.author_id = new SelectList(db.Author, "author_id", "author_name", boardgame.author_id);
            ViewBag.complexity_id = new SelectList(db.Complexity, "complexity_id", "complexity_name", boardgame.complexity_id);
            ViewBag.publisher_id = new SelectList(db.Publisher, "publisher_id", "publisher_name", boardgame.publisher_id);
            return View(boardgame);
        }

        // POST: Boardgames/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "boardgame_name,min_time,max_time,min_players,max_players,cover_path,years,language,boardgame_number,boardgame_cost,independence,images_paths,preview_description,full_description,boardgame_equipment,boardgame_id,author_id,publisher_id,complexity_id")] Boardgame boardgame)
        {
            if (ModelState.IsValid)
            {
                db.Entry(boardgame).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.author_id = new SelectList(db.Author, "author_id", "author_name", boardgame.author_id);
            ViewBag.complexity_id = new SelectList(db.Complexity, "complexity_id", "complexity_name", boardgame.complexity_id);
            ViewBag.publisher_id = new SelectList(db.Publisher, "publisher_id", "publisher_name", boardgame.publisher_id);
            return View(boardgame);
        }

        // GET: Boardgames/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boardgame boardgame = db.Boardgame.Find(id);
            if (boardgame == null)
            {
                return HttpNotFound();
            }
            return View(boardgame);
        }

        // POST: Boardgames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Boardgame boardgame = db.Boardgame.Find(id);
            db.Boardgame.Remove(boardgame);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult IntoBasket(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boardgame boardgame = db.Boardgame.Find(id);
            if (boardgame == null)
            {
                return HttpNotFound();
            }
            User user = db.User.Find(1);

            if (user.Basket != null && user.Basket.Count != 0)
            {
                bool flag = false;
                foreach (Basket basket in user.Basket)
                {
                    if (basket.boardgame_id == id)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag) IncreaseOrder(id);
                else
                {
                    user.Basket.Add(new Models.Basket()
                    {
                        Boardgame = boardgame,
                        User = db.User.Find(1),
                        Status = db.Status.Where(s => s.status_name == "Корзина").First(),
                        product_number = 1
                    });
                }
            }
            else
            {
                user.Basket.Add(new Models.Basket()
                {
                    Boardgame = boardgame,
                    User = db.User.Find(1),
                    Status = db.Status.Where(s => s.status_name == "Корзина").First(),
                    product_number = 1
                });
            }
            db.SaveChanges();
            return View("Basket", db.User.Find(1).Basket);
        }

        public ActionResult Basket()
        {
            return View(db.User.Find(1).Basket);
        }

        public ActionResult ReduceOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Basket basket = db.Basket.Find(id);
            if (basket == null)
            {
                return HttpNotFound();
            }
            basket.product_number -= 1;
            if (basket.product_number == 0)
            {
                db.Basket.Remove(basket);
            }
            db.SaveChanges();
            return View("Basket", db.User.Find(1).Basket);
        }

        public ActionResult IncreaseOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Basket basket = db.Basket.Find(id);
            if (basket == null)
            {
                return HttpNotFound();
            }
            basket.product_number += 1;
            if (basket.product_number == 0)
            {
                db.User.Find(1).Basket.Remove(basket);
            }
            db.SaveChanges();
            return View("Basket", db.User.Find(1).Basket);
        }

        public ActionResult DeleteOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Basket basket = db.Basket.Find(id);
            if (basket == null)
            {
                return HttpNotFound();
            }
            db.Basket.Remove(basket);
            db.SaveChanges();
            return View("Basket", db.User.Find(1).Basket);
        }
    }
}