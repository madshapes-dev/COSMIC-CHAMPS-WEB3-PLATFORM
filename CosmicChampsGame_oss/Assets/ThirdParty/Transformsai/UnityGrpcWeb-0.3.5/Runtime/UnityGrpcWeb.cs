using System;
using System.Net.Http;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace TransformsAI.Unity.Grpc.Web
{
    /*public static class UnityGrpcWeb
    {
        public static GrpcChannel MakeChannel (
            string address,
            GrpcWebMode mode = GrpcWebMode.GrpcWebText,
            GrpcChannelOptions options = null,
            bool disposeOnExitPlayMode = true,
            bool useLogger = false) =>
            MakeChannel (new Uri (address), mode, options, disposeOnExitPlayMode, useLogger);

        public static GrpcChannel MakeChannel (
            Uri address,
            GrpcWebMode mode = GrpcWebMode.GrpcWebText,
            GrpcChannelOptions options = null,
            bool disposeOnExitPlayMode = true,
            bool useLogger = false)
        {
            options ??= new GrpcChannelOptions ();

            if (options.HttpClient == null)
            {
                #if UNITY_WEBGL && !UNITY_EDITOR
                var handler = UnityGrpcWebHandler.Create (mode);
                #else
                var handler =  
                #endif
                
                options.HttpClient = new HttpClient (handler);
            }
            
            if (useLogger && options.LoggerFactory == null) options.LoggerFactory = new UnityLoggerFactory ();

            var channel = GrpcChannel.ForAddress (address, options);
            #if UNITY_EDITOR

            if (disposeOnExitPlayMode)
            {
                UnityEditor.EditorApplication.playModeStateChanged += change =>
                {
                    if (change == UnityEditor.PlayModeStateChange.ExitingPlayMode) channel.Dispose ();
                };
            }
            #endif
            return channel;
        }
    }*/
}