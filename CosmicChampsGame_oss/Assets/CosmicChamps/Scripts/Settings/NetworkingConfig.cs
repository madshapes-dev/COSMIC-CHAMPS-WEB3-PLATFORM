using System;
using UnityEngine;

namespace CosmicChamps.Settings
{
    [Serializable]
    public class NetworkingConfig
    {
        [SerializeField]
        private float _requestTimeout;
        
        [SerializeField]
        private float _requestRetryRate;

        [SerializeField]
        private int _beforeUnreachableNotifyAttempts;

        [SerializeField]
        private bool _logGrpcCalls;

        public float RequestTimeout => _requestTimeout;

        public float RequestRetryRate => _requestRetryRate;

        public int BeforeUnreachableNotifyAttempts => _beforeUnreachableNotifyAttempts;

        public bool LogGrpcCalls => _logGrpcCalls;
    }
}