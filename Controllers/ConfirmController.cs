using System;
using System.Collections.Generic;
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
                    Title = Request.Form["Title"],
                    Address = Request.Form["Address"],
                    ShortAddress = Request.Form["ShortAddress"],
                    UrlHash = int.Parse(Request.Form["UrlHash"])                  
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
               
                List<string> wList = _context.WhiteListModel
                .Select(w => w.Url.ToString()).ToList();

                foreach (var address in wList)
                {
                    if(uv.Address.Contains(address))
                        return RedirectToAction("Index", "Home", new { wlist = true });
                }

                //perhaps index the records via URL hash for quick lookup in db

                if (!_context.UrlViewModels.Any(e => e.UrlHash == uv.UrlHash))
                {

                    if (User.Identity.IsAuthenticated && User.Identity.Name != null)
                    {
                        uv.User = User.Identity.Name;
                    }
                    else
                    {
                        uv.User = "DriveBy";
                    }

                    uv.Title = uv.Title;

                    //active is for disabling these if insecure..

                    uv.Active = true;

                    uv.Modified = DateTime.Now;

                    //Full URL is just there for display but we only want the code in DB    

                    uv.ShortAddress = uv.ShortAddress.Replace("https://acbo.cc/", "");

                    _context.Add(uv);
                    _context.SaveChanges();
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { duplicate = true });
                }


                return RedirectToAction("Index", "Home", new { success = true, redir = uv.ShortAddress });
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                return RedirectToAction("Index","Home");
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