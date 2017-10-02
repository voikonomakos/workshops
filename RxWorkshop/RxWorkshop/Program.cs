using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace RxWorkshop
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
              
                var monitor = new Monitor();
                IDisposable subscription = Example1(monitor);

                string value;
                
                do
                {
                    Console.WriteLine("Enter:");
                    value = Console.ReadLine();
                    int id;
                   
                    if (int.TryParse(value, out id) && id > 0)
                    {
                        var userLocation = GetLocation(id);
                        monitor.RaiseEvent(userLocation);
                    }

                    if (value == "u")
                    {
                        subscription.Dispose();
                    }
                } while (value != "0");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        /// <summary>
        /// Subscription
        /// </summary>
        /// <param name="monitor"></param>
        /// <returns></returns>
        public static IDisposable Example1(Monitor monitor)
        {
            IObservable<UserLocation> locationStream = GetLocationStream(monitor);
            var consoleSubscription =  locationStream.SubscribeToConsole();

            return consoleSubscription;
        }

        /// <summary>
        /// Grouping by user id.
        /// </summary>
        /// <param name="monitor"></param>
        /// <returns></returns>
        public static IDisposable Example2(Monitor monitor)
        {
            IObservable<UserLocation> locationStream = GetLocationStream(monitor);
            return locationStream.GroupBy(x => x.Id).Subscribe(obs => obs.SubscribeToConsole());
        }

        /// <summary>
        /// Distinct location change per user
        /// </summary>
        /// <param name="monitor"></param>
        /// <returns></returns>
        public static IDisposable Example3(Monitor monitor)
        {
            IObservable<UserLocation> locationStream = GetLocationStream(monitor);
            return locationStream.GroupBy(x => x.Id).Subscribe(
                obs => obs.DistinctUntilChanged()
                          .SubscribeToConsole());
        }

        /// <summary>
        /// Apply throttling
        /// </summary>
        /// <param name="monitor"></param>
        /// <returns></returns>
        public static IDisposable Example4(Monitor monitor)
        {
            IObservable<UserLocation> locationStream = GetLocationStream(monitor);
            return locationStream.GroupBy(x => x.Id).Subscribe(
                obs => obs.DistinctUntilChanged()
                          .Throttle(TimeSpan.FromSeconds(1))
                          .SubscribeToConsole());
        }

        /// <summary>
        /// Combine obaervables to compute speed
        /// </summary>
        /// <param name="monitor"></param>
        /// <returns></returns>
        public static IDisposable Example5(Monitor monitor)
        {
            IObservable<UserLocation> locationStream = GetLocationStream(monitor);
            IObservable<long> timerObservable = Observable.Interval(TimeSpan.FromMinutes(1));
            return locationStream.GroupBy(x => x.Id).Subscribe(
                obs =>
                    obs.CombineLatest(timerObservable, (location, _) => location)
                       .Buffer(2, 1)
                       .Subscribe(ComputeSpeed));
        }

        /// <summary>
        /// Concurrency model
        /// </summary>
        private static void Example6()
        {

            IObservable<string> observable = Observable.Create<string>(observer =>
            {
                Console.WriteLine($"Calling on next from Thread {Thread.CurrentThread.ManagedThreadId}");
                observer.OnNext("First");
                Thread.Sleep(1000);
                observer.OnNext("Second");
                Thread.Sleep(1000);
                observer.OnNext("Third");
                Thread.Sleep(1000);
                observer.OnCompleted();

                return Disposable.Empty;
            });

            Console.WriteLine($"After observale creation. Thread: {Thread.CurrentThread.ManagedThreadId}");

            var subscriber = observable.
                                    //SubscribeOn(ThreadPoolScheduler.Instance).
                                    //ObserveOn(TaskPoolScheduler.Default).
                                    Subscribe(Process);
            Thread .Sleep(1500);
            subscriber.Dispose();
            Console.WriteLine($"After observale subscription. Thread: {Thread.CurrentThread.ManagedThreadId}");
        }

        private static void ComputeSpeed(IList<UserLocation> locations)
        {
        }

        /// <summary>
        /// Events to Observable
        /// </summary>
        /// <param name="monitor"></param>
        /// <returns></returns>
        private static IObservable<UserLocation> GetLocationStream(Monitor monitor)
        {
            IObservable<EventPattern<UserLocation>> eventsStream = Observable.FromEventPattern<UserLocation>(
                                                    h => monitor.LocationUpdated += h,
                                                    h => monitor.LocationUpdated -= h);

            return eventsStream.Select(x => x.EventArgs);
        }

        /// <summary>
        /// Simulate time consuming operation
        /// </summary>
        /// <param name="value"></param>
        private static void Process(string value)
        {
            Console.WriteLine($"Processing value [{value}]......... Thread {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        public static UserLocation GetLocation(int id)
        {
            var random = new Random();
            double latitude = random.Next(30, 40);
            double longtitude = random.Next(25, 40);

            return new UserLocation(id, latitude, longtitude);
        }
    }
}
   

   