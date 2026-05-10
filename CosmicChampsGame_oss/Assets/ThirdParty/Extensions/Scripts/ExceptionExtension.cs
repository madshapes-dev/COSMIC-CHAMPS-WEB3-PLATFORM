using System;

namespace ThirdParty.Extensions
{
    public static class ExceptionExtension
    {
        public static string ExtractMessage (this Exception e)
        {
            switch (e)
            {
                case AggregateException aggregateException:
                    return aggregateException.InnerException.ExtractMessage ();

                default:
                    return e.Message;
            }
        }

        public static T ExtractException<T> (this Exception e) where T : Exception =>
            e switch
            {
                null => null,
                T t => t,
                _ => e.InnerException.ExtractException<T> ()
            };
    }
}