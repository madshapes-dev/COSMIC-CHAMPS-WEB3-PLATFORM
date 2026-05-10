using Cysharp.Threading.Tasks;

namespace CosmicChamps.Services
{
    public interface IImmutableService
    {
        UniTask Initialize ();
        UniTask Login ();
        UniTask RequestAccounts ();
        UniTask<string> GetAccessToken ();
        UniTask Logout ();
    }
}