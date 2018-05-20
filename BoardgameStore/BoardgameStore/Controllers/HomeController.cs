using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BoardgameStore.Models;

namespace BoardgameStore.Controllers
{
    public class HomeController : Controller
    {
        private BoardgameStoreDBEntities db = new BoardgameStoreDBEntities();
        
        public ActionResult Catalog()
        {
            var boardgame = db.Boardgame.Include(b => b.Author).Include(b => b.Complexity).Include(b => b.Publisher);
            return View(boardgame.ToList());
        }
        
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
        
        public ActionResult Create()
        {
            List<SelectListItem> items = new SelectList(db.Author, "author_id", "author_name").ToList();
            items.Insert(0, (new SelectListItem { Text = null, Value = null, Selected=true}));
            ViewBag.author_id = items;
            items = new SelectList(db.Complexity, "complexity_id", "complexity_name").ToList();
            items.Insert(0, (new SelectListItem { Text = null, Value = null, Selected=true}));
            ViewBag.complexity_id = items;
            items = new SelectList(db.Publisher, "publisher_id", "publisher_name").ToList();
            items.Insert(0, (new SelectListItem { Text = null, Value = null, Selected=true}));
            ViewBag.publisher_id = items;
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "boardgame_name,min_time,max_time,min_players,max_players,years,language,boardgame_number,boardgame_cost,independence,images_paths,preview_description,full_description,boardgame_equipment,boardgame_id,author_id,publisher_id,complexity_id")] Boardgame boardgame, HttpPostedFileBase image = null)
        {
            bool checkCoverLength = true;
            bool checkCoverType = true;
            List<SelectListItem> items;
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    bool flag = false;
                    string type = image.ContentType.Split(new char[] { '/' })[0];
                    if (type.Equals("image"))
                    {
                        if (image.ContentLength <= 2000000)
                        {
                            string covername = "";
                            do
                            {
                                type = System.IO.Path.GetFileName(image.FileName).Split(new char[] { '.' })[System.IO.Path.GetFileName(image.FileName).Split(new char[] { '.' }).Length - 1];
                                covername = Path.GetRandomFileName() + "." + type;
                                if (System.IO.File.Exists("~/Content/" + covername)) flag = true;
                            } while (flag);
                            image.SaveAs(Server.MapPath("~/Content/" + covername));
                            boardgame.cover_path = covername;
                        }
                        else
                        {
                            checkCoverLength = false;
                        }
                    }
                    else
                    {
                        checkCoverType = false;
                    }
                }
                db.Boardgame.Add(boardgame);
                db.SaveChanges();
                if (!checkCoverType)
                {
                    ViewBag.CoverType = true;
                    items = new SelectList(db.Author, "author_id", "author_name", boardgame.author_id).ToList();
                    items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
                    ViewBag.author_id = items;
                    items = new SelectList(db.Complexity, "complexity_id", "complexity_name", boardgame.complexity_id).ToList();
                    items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
                    ViewBag.complexity_id = items;
                    items = new SelectList(db.Publisher, "publisher_id", "publisher_name", boardgame.publisher_id).ToList();
                    items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
                    ViewBag.publisher_id = items;
                    return View("Edit", boardgame);
                }
                else if (!checkCoverLength)
                {
                    ViewBag.CoverLength = true;
                    items = new SelectList(db.Author, "author_id", "author_name", boardgame.author_id).ToList();
                    items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
                    ViewBag.author_id = items;
                    items = new SelectList(db.Complexity, "complexity_id", "complexity_name", boardgame.complexity_id).ToList();
                    items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
                    ViewBag.complexity_id = items;
                    items = new SelectList(db.Publisher, "publisher_id", "publisher_name", boardgame.publisher_id).ToList();
                    items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
                    ViewBag.publisher_id = items;
                    return View("Edit", boardgame);
                }
                return RedirectToAction("Catalog");
            }

            items = new SelectList(db.Author, "author_id", "author_name", boardgame.author_id).ToList();
            items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
            ViewBag.author_id = items;
            items = new SelectList(db.Complexity, "complexity_id", "complexity_name", boardgame.complexity_id).ToList();
            items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
            ViewBag.complexity_id = items;
            items = new SelectList(db.Publisher, "publisher_id", "publisher_name", boardgame.publisher_id).ToList();
            items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
            ViewBag.publisher_id = items; return View(boardgame);
        }
        
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
            List<SelectListItem> items = new SelectList(db.Author, "author_id", "author_name", boardgame.author_id).ToList();
            items.Insert(0, (new SelectListItem { Text = null, Value = null,  }));
            ViewBag.author_id = items;
            items = new SelectList(db.Complexity, "complexity_id", "complexity_name", boardgame.complexity_id).ToList();
            items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
            ViewBag.complexity_id = items;
            items = new SelectList(db.Publisher, "publisher_id", "publisher_name", boardgame.publisher_id).ToList();
            items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
            ViewBag.publisher_id = items;
            return View(boardgame);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "boardgame_name,min_time,max_time,min_players,max_players,cover_path,years,language,boardgame_number,boardgame_cost,independence,images_paths,preview_description,full_description,boardgame_equipment,boardgame_id,author_id,publisher_id,complexity_id")] Boardgame boardgame, HttpPostedFileBase image = null)
        {
            bool checkCoverLength = true;
            bool checkCoverType = true;
            List<SelectListItem> items;
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    bool flag = false;
                    string type = image.ContentType.Split(new char[] { '/' })[0];
                    if (type.Equals("image"))
                    {
                        if (image.ContentLength <= 2000000)
                        {
                            string covername = "";
                            do
                            {
                                type = System.IO.Path.GetFileName(image.FileName).Split(new char[] { '.' })[System.IO.Path.GetFileName(image.FileName).Split(new char[] { '.' }).Length - 1];
                                covername = Path.GetRandomFileName() + "." + type;
                                if (System.IO.File.Exists("~/Content/" + covername)) flag = true;
                            } while (flag);
                            image.SaveAs(Server.MapPath("~/Content/" + covername));
                            boardgame.cover_path = covername;
                        }
                        else
                        {
                            checkCoverLength = false;
                        }
                    }
                    else
                    {
                        checkCoverType = false;
                    }
                }
                db.Entry(boardgame).State = EntityState.Modified;
                db.SaveChanges();
                if (!checkCoverType)
                {
                    ViewBag.CoverType = true;

                    items = new SelectList(db.Author, "author_id", "author_name", boardgame.author_id).ToList();
                    items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
                    ViewBag.author_id = items;
                    items = new SelectList(db.Complexity, "complexity_id", "complexity_name", boardgame.complexity_id).ToList();
                    items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
                    ViewBag.complexity_id = items;
                    items = new SelectList(db.Publisher, "publisher_id", "publisher_name", boardgame.publisher_id).ToList();
                    items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
                    ViewBag.publisher_id = items;
                    return View(boardgame);
                }
                else if (!checkCoverLength)
                {
                    ViewBag.CoverLength = true;

                    items = new SelectList(db.Author, "author_id", "author_name", boardgame.author_id).ToList();
                    items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
                    ViewBag.author_id = items;
                    items = new SelectList(db.Complexity, "complexity_id", "complexity_name", boardgame.complexity_id).ToList();
                    items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
                    ViewBag.complexity_id = items;
                    items = new SelectList(db.Publisher, "publisher_id", "publisher_name", boardgame.publisher_id).ToList();
                    items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
                    ViewBag.publisher_id = items;
                    return View(boardgame);
                }
                return RedirectToAction("Catalog", db.Boardgame);
            }

            items = new SelectList(db.Author, "author_id", "author_name", boardgame.author_id).ToList();
            items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
            ViewBag.author_id = items;
            items = new SelectList(db.Complexity, "complexity_id", "complexity_name", boardgame.complexity_id).ToList();
            items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
            ViewBag.complexity_id = items;
            items = new SelectList(db.Publisher, "publisher_id", "publisher_name", boardgame.publisher_id).ToList();
            items.Insert(0, (new SelectListItem { Text = null, Value = null, }));
            ViewBag.publisher_id = items;
            return View(boardgame);
        }

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
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Boardgame boardgame = db.Boardgame.Find(id);
            if (boardgame.cover_path != null && boardgame.cover_path != "")
            {
                System.IO.File.Delete(Server.MapPath("~/Content/" + boardgame.cover_path));
            }
            db.Boardgame.Remove(boardgame);
            db.SaveChanges();
            return RedirectToAction("Catalog", db.Boardgame);
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
            ViewBag.Sum = db.User.Find(1).Basket.Sum(b => b.product_number * b.product_number);
            return View("Basket", db.User.Find(1).Basket);
        }

        public ActionResult Basket()
        {
            ViewBag.Sum = db.User.Find(1).Basket.Sum(b => b.Boardgame.boardgame_cost*b.product_number);
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
            return RedirectToAction("Basket", db.User.Find(1).Basket);
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
            return RedirectToAction("Basket", db.User.Find(1).Basket);
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
            return RedirectToAction("Basket", db.User.Find(1).Basket);
        }
    }
}