using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace urlshorten.Models
{
    public class WhiteListModel
    {
        [Key]
        public int Id { get; set; }

        public string Source { get; set; }

        public Uri Url { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime Modified { get; set; }

    }
}