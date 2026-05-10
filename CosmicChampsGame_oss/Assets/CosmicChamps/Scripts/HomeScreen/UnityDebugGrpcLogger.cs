using System;
using System.Collections.Generic;
using System.Linq;
using CosmicChamps.Settings;
using CosmicChamps.Signals;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using Zenject;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace CosmicChamps.HomeScreen
{
    public class GrpcLoggerScope : IDisposable
    {
        public static GrpcLoggerScope Instance => new();

        public void Dispose ()
        {
        }
    }

    public class UnityDebugGrpcLogger : ILogger
    {
        public class Provider : ILoggerProvider
        {
            public ILogger CreateLogger (string categoryName) => Instance;

            public void Dispose ()
            {
            }
        }

        public static readonly UnityDebugGrpcLogger Instance = new();

        public void Log<TState> (
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            Debug.Log ($"[{logLevel}]: ({eventId}) {formatter (state, exception)}");
        }

        public bool IsEnabled (LogLevel logLevel) => true;

        public IDisposable BeginScope<TState> (TState state) => GrpcLoggerScope.Instance;
    }

    public class NetworkReachabilityGrpcLogger : ILogger
    {
        public class Provider : ILoggerProvider
        {
            private readonly DiContainer _container;
            private NetworkReachabilityGrpcLogger _loggerInstance;

            public Provider (DiContainer container)
            {
                _container = container;
            }

            public ILogger CreateLogger (string categoryName) =>
                _loggerInstance ??= _container.Instantiate<NetworkReachabilityGrpcLogger> ();

            public void Dispose ()
            {
            }
        }

        private readonly NetworkingConfig _networkingConfig;
        private readonly IMessageBroker _messageBroker;

        public NetworkReachabilityGrpcLogger (NetworkingConfig networkingConfig, IMessageBroker messageBroker)
        {
            _networkingConfig = networkingConfig;
            _messageBroker = messageBroker;
        }

        public void Log<TState> (
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            async UniTaskVoid PublishSignal<T> (T message)
            {
                await UniTask.SwitchToMainThread ();
                _messageBroker.Publish (message);
            }

            switch (eventId.Name)
            {
                case "StartingAttempt":
                    var message = formatter (state, exception);
                    var attempt = int.Parse (string.Concat (message.Where (char.IsDigit)));
                    if (attempt > _networkingConfig.BeforeUnreachableNotifyAttempts)
                        PublishSignal (NetworkReachabilitySignal.UnreachableSignal).Forget ();

                    break;

                case "StoppingRetryWorker":
                    PublishSignal (NetworkReachabilitySignal.ReachableSignal).Forget ();
                    break;
            }
        }

        public bool IsEnabled (LogLevel logLevel) => true;

        public IDisposable BeginScope<TState> (TState state) => GrpcLoggerScope.Instance;
    }

    public class ChainGrpcLogger : ILogger
    {
        private readonly ICollection<ILogger> _loggers;

        public ChainGrpcLogger (ICollection<ILogger> loggers)
        {
            _loggers = loggers;
        }

        public void Log<TState> (
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            foreach (var logger in _loggers)
            {
                logger.Log (logLevel, eventId, state, exception, formatter);
            }
        }

        public bool IsEnabled (LogLevel logLevel) => true;

        public IDisposable BeginScope<TState> (TState state) => GrpcLoggerScope.Instance;
    }

    public class GrpcLoggerFactory : ILoggerFactory
    {
        private readonly List<ILoggerProvider> _loggerProviders = new();

        public ILogger CreateLogger (string categoryName) =>
            new ChainGrpcLogger (_loggerProviders.ConvertAll (x => x.CreateLogger (categoryName)));

        public void AddProvider (ILoggerProvider provider) => _loggerProviders.Add (provider);

        public void Dispose ()
        {
            _loggerProviders.Clear ();
        }
    }
}