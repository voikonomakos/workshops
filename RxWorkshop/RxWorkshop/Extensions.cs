using System;

namespace RxWorkshop
{
    public static class Extensions
    {
        public static IDisposable SubscribeToConsole<T>(this IObservable<T> observable)
        {
            var subscription = observable.Subscribe(x => Console.WriteLine(x),
                exception => Console.WriteLine($"Error: {exception.Message}"));

            return subscription;
        }
    }
}
