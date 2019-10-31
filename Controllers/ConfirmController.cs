using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var Form = Request.Form;
            
            return View();
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