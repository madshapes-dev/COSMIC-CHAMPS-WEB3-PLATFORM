using UnityEngine;

namespace CosmicChamps.Settings
{
    [CreateAssetMenu (menuName = "Cosmic Champs/AWS Credentials")]
    public class AWSCredentials : ScriptableObject
    {
        [SerializeField]
        private string _accessKeyId;

        [SerializeField]
        private string _secretAccessKey;

        public string AccessKeyId => _accessKeyId;

        public string SecretAccessKey => _secretAccessKey;
    }
}