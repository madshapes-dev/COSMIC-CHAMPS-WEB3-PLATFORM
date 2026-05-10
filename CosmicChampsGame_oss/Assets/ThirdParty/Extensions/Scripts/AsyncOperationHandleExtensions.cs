using System;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ThirdParty.Extensions
{
    public static class AsyncOperationHandleExtensions
    {
        private class AsyncOperationHandleObservable : IObservable<AsyncOperationHandle>
        {
            private readonly Subject<AsyncOperationHandle> subject = new();
            private readonly AsyncOperationHandle asyncOperationHandle;
            private readonly bool releaseHandle;
            private bool isRunning;

            public AsyncOperationHandleObservable (AsyncOperationHandle asyncOperationHandle, bool releaseHandle)
            {
                this.asyncOperationHandle = asyncOperationHandle;
                this.releaseHandle = releaseHandle;
            }

            private async void RunTask ()
            {
                if (isRunning)
                    return;

                isRunning = true;

                try
                {
                    await asyncOperationHandle.Task;

                    switch (asyncOperationHandle.Status)
                    {
                        case AsyncOperationStatus.None:
                            throw new Exception ("Wrong handle status");
                        case AsyncOperationStatus.Failed:
                            throw asyncOperationHandle.OperationException;
                        case AsyncOperationStatus.Succeeded:
                            subject.OnNext (asyncOperationHandle);
                            subject.OnCompleted ();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException ();
                    }
                } catch (Exception e)
                {
                    subject.OnError (e);
                } finally
                {
                    if (releaseHandle)
                        Addressables.Release (asyncOperationHandle);
                }
            }

            public IDisposable Subscribe (IObserver<AsyncOperationHandle> observer)
            {
                RunTask ();
                return subject.Subscribe (observer);
            }
        }

        private class AsyncOperationHandleObservable<T> : IObservable<AsyncOperationHandle<T>>
        {
            private readonly Subject<AsyncOperationHandle<T>> subject = new();
            private readonly AsyncOperationHandle<T> asyncOperationHandle;
            private readonly bool releaseHandle;
            private bool isRunning;

            public AsyncOperationHandleObservable (AsyncOperationHandle<T> asyncOperationHandle, bool releaseHandle)
            {
                this.asyncOperationHandle = asyncOperationHandle;
                this.releaseHandle = releaseHandle;
            }

            private async void RunTask ()
            {
                if (isRunning)
                    return;

                isRunning = true;

                try
                {
                    await asyncOperationHandle.Task;

                    switch (asyncOperationHandle.Status)
                    {
                        case AsyncOperationStatus.None:
                            throw new Exception ("Wrong handle status");
                        case AsyncOperationStatus.Failed:
                            throw asyncOperationHandle.OperationException;
                        case AsyncOperationStatus.Succeeded:
                            subject.OnNext (asyncOperationHandle);
                            subject.OnCompleted ();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException ();
                    }
                } catch (Exception e)
                {
                    subject.OnError (e);
                } finally
                {
                    if (releaseHandle && asyncOperationHandle.IsValid ())
                        Addressables.Release (asyncOperationHandle);
                }
            }

            public IDisposable Subscribe (IObserver<AsyncOperationHandle<T>> observer)
            {
                if (asyncOperationHandle.IsValid () && asyncOperationHandle.IsDone)
                {
                    try
                    {
                        switch (asyncOperationHandle.Status)
                        {
                            case AsyncOperationStatus.None:
                                throw new Exception ("Wrong handle status");
                            case AsyncOperationStatus.Failed:
                                throw asyncOperationHandle.OperationException;
                            case AsyncOperationStatus.Succeeded:
                                observer.OnNext (asyncOperationHandle);
                                observer.OnCompleted ();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException ();
                        }
                    } catch (Exception e)
                    {
                        observer.OnError (e);
                    } finally
                    {
                        if (releaseHandle && asyncOperationHandle.IsValid ())
                            Addressables.Release (asyncOperationHandle);
                    }

                    return subject.Subscribe (observer);
                }

                RunTask ();
                return subject.Subscribe (observer);
            }
        }

        public static IObservable<AsyncOperationHandle> AsObservable (
            this AsyncOperationHandle asyncOperationHandle,
            bool releaseHandle = false) =>
            //
            new AsyncOperationHandleObservable (asyncOperationHandle, releaseHandle);

        public static IObservable<AsyncOperationHandle<T>> AsObservable<T> (
            this AsyncOperationHandle<T> asyncOperationHandle,
            bool releaseHandle = false) =>
            //
            new AsyncOperationHandleObservable<T> (asyncOperationHandle, releaseHandle);

        public static ReadOnlyReactiveProperty<float> AsProgressReactiveProperty (
            this AsyncOperationHandle asyncOperationHandle) =>
            //
            Observable
                .EveryUpdate ()
                .Select (
                    _ => asyncOperationHandle.IsValid ()
                        ? asyncOperationHandle.GetDownloadStatus ().Percent
                        : asyncOperationHandle.IsDone
                            ? 1f
                            : 0f)
                .Distinct ()
                .ToReadOnlyReactiveProperty ();

        public static ReadOnlyReactiveProperty<float> AsProgressReactiveProperty<T> (
            this AsyncOperationHandle<T> asyncOperationHandle) =>
            //
            Observable
                .EveryUpdate ()
                .Select (_ => asyncOperationHandle.GetDownloadStatus ().Percent)
                .Distinct ()
                .ToReadOnlyReactiveProperty ();
    }
}