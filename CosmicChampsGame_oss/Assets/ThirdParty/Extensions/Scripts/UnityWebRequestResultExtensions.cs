using UnityEngine.Networking;

namespace ThirdParty.Extensions
{
    public static class UnityWebRequestResultExtensions
    {
        public static bool IsError (this UnityWebRequest.Result result) =>
            result == UnityWebRequest.Result.ConnectionError ||
            result == UnityWebRequest.Result.ProtocolError ||
            result == UnityWebRequest.Result.DataProcessingError;
    }
}