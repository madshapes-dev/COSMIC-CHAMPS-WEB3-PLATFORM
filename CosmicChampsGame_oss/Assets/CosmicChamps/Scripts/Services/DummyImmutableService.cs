using Cysharp.Threading.Tasks;

namespace CosmicChamps.Services
{
    public class DummyImmutableService : IImmutableService
    {
        public UniTask Initialize () => throw new System.NotImplementedException ();
        public UniTask Login () => throw new System.NotImplementedException ();
        public UniTask RequestAccounts () => throw new System.NotImplementedException ();
        public UniTask<string> GetAccessToken () => throw new System.NotImplementedException ();
        public UniTask Logout () => throw new System.NotImplementedException ();
    }
}