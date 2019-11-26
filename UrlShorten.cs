using System;
using System.Linq;
using System.Text;

namespace urlshorten
{
    sealed class UrlShorten : IDisposable
    {
        internal const string alphanum = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        internal const int alphabase = 62;

        internal StringBuilder shortid = new StringBuilder();

        internal int idecode;

        public string ShortenedUrl { get; set; }

        public UrlShorten(string url = "")
            : base()
        {
            if(url != "")
                ShortenedUrl = Encode(Decode(url));
        }

        public string Encode(int idx)
        {
            do
            {
                shortid.Append(alphanum[idx % alphabase]);
                idx = Math.Abs(idx /= alphabase);

            } while (idx > 0);

            return new string(shortid.ToString().Reverse().ToArray());
        }

        public int Decode(String str)
        {
            foreach (var chr in str)
            {
                idecode *= alphabase;
                idecode = Math.Abs(idecode += alphanum.IndexOf(chr));
            }

            return idecode;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    shortid = new StringBuilder();
                    idecode = new int();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~UrlShorten()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion


    }
}
