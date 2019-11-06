using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using urlshorten.Models;

namespace urlshorten.Controllers
{
    public class ConfirmController : Controller
    {
        
        private readonly URLShortenDBContext _context;

        public ConfirmController(URLShortenDBContext context)
        {
            _context = context;
        }
        
        // GET: Confirm

        public ActionResult Index()
        {
            if(Request.Form.Any())
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
        public ActionResult Edit(UrlViewModel uv)
        {
            try
            {
                // TODO: Add update logic here

                
                    //perhaps index the records via URL hash for quick lookup in db
                    
                    if(!_context.UrlViewModels.Any(e => e.Id == uv.UrlHash))
                    {
                        uv.Id = uv.UrlHash;

                        uv.User = "DriveBy";

                        uv.Title = "InitialSave";

                        _context.Add(uv);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return RedirectToAction("Index","Home", new { duplicate = true });
                    }
                

                return RedirectToAction("Index","Home");
            }
            catch(Exception ex)
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