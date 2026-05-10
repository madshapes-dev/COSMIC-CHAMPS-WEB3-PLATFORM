using System;
using System.Threading.Tasks;
using UniRx;

namespace ThirdParty.Extensions
{
    public static class TaskExtensions
    {
        private class TaskObservable : IObservable<Unit>
        {
            private readonly Subject<Unit> subject = new();
            private readonly Task task;
            private bool isRunning;

            public TaskObservable (Task task)
            {
                this.task = task;
            }

            private async void RunTask ()
            {
                if (isRunning)
                    return;

                isRunning = true;

                try
                {
                    await task;

                    if (task.IsFaulted)
                        throw task.Exception ?? new Exception ("Unknown");

                    subject.OnNext (Unit.Default);
                    subject.OnCompleted ();
                } catch (Exception e)
                {
                    subject.OnError (e);
                }
            }

            public IDisposable Subscribe (IObserver<Unit> observer)
            {
                RunTask ();
                return subject.Subscribe (observer);
            }
        }

        private class TaskObservable<T> : IObservable<T>
        {
            private readonly Subject<T> subject = new();
            private readonly Task<T> task;
            private bool isRunning;

            public TaskObservable (Task<T> task)
            {
                this.task = task;
            }

            private async void RunTask ()
            {
                if (isRunning)
                    return;

                isRunning = true;

                try
                {
                    var result = await task;

                    if (task.IsFaulted)
                        throw task.Exception ?? new Exception ("Unknown");

                    subject.OnNext (result);
                    subject.OnCompleted ();
                } catch (Exception e)
                {
                    subject.OnError (e);
                }
            }

            public IDisposable Subscribe (IObserver<T> observer)
            {
                RunTask ();
                return subject.Subscribe (observer);
            }
        }

        public static IObservable<Unit> AsObservable (this Task task)
        {
            return new TaskObservable (task);
        }

        public static IObservable<T> AsObservable<T> (this Task<T> task)
        {
            return new TaskObservable<T> (task);
        }
    }
}