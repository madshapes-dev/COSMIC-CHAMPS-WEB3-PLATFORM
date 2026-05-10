using System;
using UnityEngine;

namespace CosmicChamps.Settings
{
    [Serializable]
    public class ServerProcessOptions
    {
        [SerializeField]
        private string _logFile;

        [SerializeField]
        private int _port;

        public ServerProcessOptions (string logFile, int port)
        {
            _logFile = logFile;
            _port = port;
        }

        public string LogFile => _logFile;

        public int Port => _port;
    }
}