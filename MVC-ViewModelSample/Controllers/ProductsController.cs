using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_ViewModelSample.Models;
using PagedList;

namespace MVC_ViewModelSample.Controllers
{
    public class ProductsController : Controller
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: Products
        /// <summary>
        /// 產品列表
        /// </summary>
        /// <param name="search">搜尋產品名稱和分類</param>
        /// <param name="categoryNameSearch">下拉式選單搜尋分類</param>
        /// <param name="sortOrder">檢示頁面排序</param>
        /// <param name="page">分頁</param>
        /// <param name="currentFilter">當前搜尋的名稱</param>
        /// <param name="currentFilterCategoryName">當前的下拉選單分類</param>
        /// <returns>View(products.ToPagedList(pageNumber, pageSize=6))</returns>
        public ActionResult Index(string search, string categoryNameSearch, string sortOrder,
            int? page, string currentFilter, string currentFilterCategoryName)
        {

            if (search != null || categoryNameSearch != null)
            {
                page = 1;
            }
            else
            {
                search = currentFilter;
                categoryNameSearch = currentFilterCategoryName;
            }

            ViewBag.CurrentFilter = search;
            ViewBag.CurrentFilterCategory = categoryNameSearch;

            //加入搜尋分類產品，UI 使用DropDownList呈現
            var categoryList = new List<string>();
            var categorName = from c in db.Products
                              orderby c.Categories.CategoryName
                              select c.Categories.CategoryName;

            categoryList.AddRange(categorName.Distinct());

            ViewBag.categoryNameSearch = new SelectList(categoryList);

            var products = from p in db.Products
                           select p;

            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products
                    .Where(s => s.ProductName.Contains(search) 
                                       || s.Categories.CategoryName.Contains(search));
            }
            
            if (!string.IsNullOrWhiteSpace(categoryNameSearch))
            {
                products = products.Where(p => p.Categories.CategoryName == categoryNameSearch);
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrWhiteSpace(sortOrder) ? "name_desc" : "";

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }
         
            int pageSize = 6;
            // 運算式表示在它含有值時會傳回值 page，或在 page 為 null 時傳回 1。
            int pageNumber = (page ?? 1);

            return View(products.ToPagedList(pageNumber, pageSize));
            
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Products products)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(products);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
            return View(products);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Products products)
        {
            if (ModelState.IsValid)
            {
                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
            return View(products);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = db.Products.Find(id);
            db.Products.Remove(products);
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
    }
}
