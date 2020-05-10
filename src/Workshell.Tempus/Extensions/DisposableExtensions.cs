using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    internal static class DisposableExtensions
    {
        #region Methods

        public static void Dispose(this IDisposable disposable, Action action)
        {
            disposable.Dispose();
            action?.Invoke();
        }

        #endregion
    }
}
