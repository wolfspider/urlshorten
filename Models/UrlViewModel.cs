using System;

namespace urlshorten.Models
{
    public class UrlViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public bool Active { get; set; }

        public int UrlHash { get; set; }

        public string User { get; set; }

        public Uri Address { get; set; }

        public Uri ShortAddress { get; set; }
        
    }
}