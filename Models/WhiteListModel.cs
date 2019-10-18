using System;

namespace urlshorten.Models
{
    public class WhiteListModel
    {
        public int Id { get; set; }

        public string Source { get; set; }

        public Uri Url { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

    }
}