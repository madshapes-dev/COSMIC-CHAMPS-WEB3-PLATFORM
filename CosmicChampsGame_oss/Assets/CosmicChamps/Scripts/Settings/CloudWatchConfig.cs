using System;
using UnityEngine;

namespace CosmicChamps.Settings
{
    [Serializable]
    public class CloudWatchConfig
    {
        [SerializeField]
        private string _logGroup;

        [SerializeField]
        private int _batchSizeLimit = 100;

        [SerializeField]
        private int _queueSizeLimit = 10000;

        [SerializeField]
        private int _period = 10;

        [SerializeField]
        private bool _createLogGroup = true;

        public string LogGroup => _logGroup;

        public int BatchSizeLimit => _batchSizeLimit;

        public int QueueSizeLimit => _queueSizeLimit;

        public int Period => _period;

        public bool CreateLogGroup => _createLogGroup;
    }
}