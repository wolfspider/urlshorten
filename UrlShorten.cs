using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urlshorten
{
    sealed class UrlShorten
    {
        internal const string alphanum = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        internal const int alphabase = 62;

        internal StringBuilder shortid = new StringBuilder();

        internal int idecode;

        internal char[] dest;

        public string Encode(int idx)
        {
            do
            {
                shortid.Append(alphanum[idx % alphabase]);
                idx /= alphabase;
                idx = Math.Abs(idx);

            } while (idx > 0);

            dest = new char[shortid.Length];

            shortid.CopyTo(0, dest, 0, shortid.Length);

            return new String(dest.Reverse().ToArray());
        }

        public int Decode(String str)
        {
            foreach (var chr in str)
            {
                idecode *= alphabase;
                idecode += alphanum.IndexOf(chr);
            }
            
            return idecode;
        }
    }
}
