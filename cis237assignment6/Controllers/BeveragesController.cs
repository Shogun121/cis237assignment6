using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237assignment6.Models;

namespace cis237assignment6.Controllers
{
    [Authorize]
    public class BeveragesController : Controller
    {
        private BeverageRCooleyEntities db = new BeverageRCooleyEntities();

        // GET: Beverages
        public ActionResult Index()
        {
            //Set up a variable to hold the Beverage data-set.
            DbSet<Beverage> BeverageToFind =db.Beverages;

            //Setup some strings that might hold data from the session.
            //If the session is empty we can still use them as default values.
            string filterId = "";
            string filterName = "";
            string filterPack = "";
            string filterMin = "";
            string filterMax = "";
            string filterActive = "";

            //Define a min and a max for price.
            int min = 0;
            int max = 100;

            //Check to see if there is a variable in the session, if there is assign it to the
            //variable we setup to hold the value.
            if(Session["id"]!=null && !String.IsNullOrWhiteSpace((string)Session["id"]))
            {
                filterId = (string)Session["id"];
            }
            if(Session["name"]!=null && !String.IsNullOrWhiteSpace((string)Session["name"]))
            {
                filterName = (string)Session["name"];
            }
            if (Session["pack"] != null && !String.IsNullOrWhiteSpace((string)Session["pack"]))
            {
                filterName = (string)Session["pack"];
            }
            if (Session["min"] != null && !String.IsNullOrWhiteSpace((string)Session["min"]))
            {
                filterMin = (string)Session["min"];
                min = Int32.Parse(filterMin);
            }
            if (Session["Max"] != null && !String.IsNullOrWhiteSpace((string)Session["Max"]))
            {
                filterMax = (string)Session["Max"];
                min = Int32.Parse(filterMax);
            }
            if (Session["active"] != null && !String.IsNullOrWhiteSpace((string)Session["active"]))
            {
                filterName = (string)Session["active"];
            }

            //Do the filter on the Beverage Item set. Use the where that we used before.
            //When using EF this time send it more lamda expressions to narrow it down.
            //Since we set the values for each of the filter values we can count on this running with no 
            //errors.
            IEnumerable<Beverage> filtered = BeverageToFind.Where(Beverage => Beverage.price >= min &&
                                                                  Beverage.price <= max &&
                                                                  Beverage.id.Contains(filterId) &&
                                                                  Beverage.name.Contains(filterName)&&
                                                                  Beverage.pack.Contains(filterPack)&&
                                                                  Beverage.active.Equals(filterActive));

            //Place the string representation of the values in the session into the ViewBag
            //so that they can be retrieved and displayed on the view.
            ViewBag.filterMake = filterId;
            ViewBag.filterDesc = filterName;
            ViewBag.filterPack = filterPack;
            ViewBag.filterMin = filterMin;
            ViewBag.filterMax = filterMax;
            ViewBag.filterActive = filterActive;

            //Return the view with a filtered selection of the beverages.
            return View(filtered);
            //return View(db.Beverages.ToList());
        }

        // GET: Beverages/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // GET: Beverages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Beverages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Beverages.Add(beverage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(beverage);
        }

        // GET: Beverages/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: Beverages/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter()
        {
            //Get the form data that was sent out of the request object.
            //The key that is used as key to get the data matches the name property of the form control.
            //For us this is the first parameter.
            String id = Request.Form.Get("id");
            String name = Request.Form.Get("name");
            String pack=Request.Form.Get("pack");
            String min = Request.Form.Get("min");
            String max = Request.Form.Get("max");
            String active = Request.Form.Get("active");

            //Store the form data in the session so we can retrieve it later on the filter.
            Session["id"] = "";
            Session["name"] = "";
            Session["pack"] = "";
            Session["min"] = "";
            Session["max"] = "";
            Session["active"] = "";

            //redirect the user to the index page we will do the work of filtering in the index method.
            return RedirectToAction("Index");
        }
    }
}
