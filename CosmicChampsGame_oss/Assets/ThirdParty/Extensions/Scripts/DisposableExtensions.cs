using System;

namespace ThirdParty.Extensions
{
    public static class DisposableExtensions
    {
        public static void DisposeAndReset (ref IDisposable disposable)
        {
            disposable?.Dispose ();
            disposable = null;
        }
    }
}