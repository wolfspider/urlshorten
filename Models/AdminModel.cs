using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace urlshorten.Models
{
    public class AdminModel
    {
        [Key]
        public int Id { get; set; }

        public string User { get; set; }

        public string Groups { get; set; }

    }
}
