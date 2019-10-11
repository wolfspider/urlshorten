using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urlshorten
{
    sealed class UrlShorten : IDisposable
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
            
            return Math.Abs(idecode);
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
                }

                shortid = new StringBuilder();
                idecode = new int();
                dest = new char[0];

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UrlShorten()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
