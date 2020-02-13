using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace urlshorten.Models
{
    public class UrlViewModel
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime Modified { get; set; }

        public bool Active { get; set; }

        public int UrlHash { get; set; }

        public string User { get; set; }

        public string Address { get; set; }

        [Display(Name = "Link")]
        public string ShortAddress { get; set; }
        
    }
}