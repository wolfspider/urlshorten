using System;
using System.ComponentModel.DataAnnotations;

namespace urlshorten.Models
{
    public class UrlViewModel
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public bool Active { get; set; }

        public int UrlHash { get; set; }

        public string User { get; set; }

        public string Address { get; set; }

        public string ShortAddress { get; set; }
        
    }
}