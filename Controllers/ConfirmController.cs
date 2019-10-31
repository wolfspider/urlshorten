using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using urlshorten.Models;

namespace urlshorten.Controllers
{
    public class ConfirmController : Controller
    {
        // GET: Confirm

        public ActionResult Index()
        {
            UrlViewModel uv = new UrlViewModel()
            { 

                Address = Request.Form["Address"],
                ShortAddress = Request.Form["ShortAddress"],
                UrlHash = int.Parse(Request.Form["UrlHash"]),
                Created = DateTime.Now
             
            };
           
            return View(uv);
        }

        // GET: Confirm/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Confirm/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Confirm/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Confirm/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Confirm/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Confirm/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Confirm/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}