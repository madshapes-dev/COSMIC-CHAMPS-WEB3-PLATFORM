using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions
{
    public static class ReactiveCommandExtensions
    {
        public static ReactiveCommand<T> SubscribeAndBindToDisposable<T> (
            this ReactiveCommand<T> reactiveCommand,
            Action<T> subscription,
            CompositeDisposable disposables)
        {
            reactiveCommand
                .AddTo (disposables)
                .Subscribe (subscription)
                .AddTo (disposables);

            return reactiveCommand;
        }

        public static ReactiveCommand<T> SubscribeAndBindToDisposable<T> (
            this ReactiveCommand<T> reactiveCommand,
            Action<T> subscription,
            Component component)
        {
            reactiveCommand
                .AddTo (component)
                .Subscribe (subscription)
                .AddTo (component);

            return reactiveCommand;
        }

        public static ReactiveCommand SubscribeAndBindToDisposable (
            this ReactiveCommand reactiveCommand,
            Action subscription,
            CompositeDisposable disposables)
        {
            reactiveCommand
                .AddTo (disposables)
                .Subscribe (_ => subscription ())
                .AddTo (disposables);

            return reactiveCommand;
        }

        public static ReactiveCommand SubscribeAndBindToDisposable (
            this ReactiveCommand reactiveCommand,
            Action subscription,
            Component component)
        {
            reactiveCommand
                .AddTo (component)
                .Subscribe (_ => subscription ())
                .AddTo (component);

            return reactiveCommand;
        }

        public static IDisposable BindToOnClick<T> (this IReactiveCommand<T> command, Button button, T value)
        {
            var d1 = command.CanExecute.SubscribeToInteractable (button);
            var d2 = button
                .OnClickAsObservable ()
                .Select (_ => value)
                .SubscribeWithState (command, (x, c) => c.Execute (x));
            return StableCompositeDisposable.Create (d1, d2);
        }

        public static IDisposable BindToOnClickVisible<T> (this IReactiveCommand<T> command, Button button, T value)
        {
            var d1 = command.CanExecute.SubscribeToVisible (button);
            var d2 = button
                .OnClickAsObservable ()
                .Select (_ => value)
                .SubscribeWithState (command, (x, c) => c.Execute (x));
            return StableCompositeDisposable.Create (d1, d2);
        }

        public static IDisposable BindToOnClick<T> (
            this IReactiveCommand<T> command,
            Button button,
            Func<T> valueGetter)
        {
            var d1 = command.CanExecute.SubscribeToInteractable (button);
            var d2 = button
                .OnClickAsObservable ()
                .Select (_ => valueGetter ())
                .SubscribeWithState (command, (x, c) => c.Execute (x));
            return StableCompositeDisposable.Create (d1, d2);
        }

        public static IDisposable BindToOnClickVisible<T> (
            this IReactiveCommand<T> command,
            Button button,
            Func<T> valueGetter)
        {
            var d1 = command.CanExecute.SubscribeToVisible (button);
            var d2 = button
                .OnClickAsObservable ()
                .Select (_ => valueGetter ())
                .SubscribeWithState (command, (x, c) => c.Execute (x));
            return StableCompositeDisposable.Create (d1, d2);
        }
    }
}