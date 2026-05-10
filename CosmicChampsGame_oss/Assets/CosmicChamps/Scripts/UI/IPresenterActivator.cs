using Cysharp.Threading.Tasks;

namespace CosmicChamps.UI
{
    public interface IPresenterActivator
    {
        UniTask Activate (AbstractPresenter presenter, bool immediate);
        UniTask Deactivate (AbstractPresenter presenter, bool immediate);
        bool IsActive (AbstractPresenter presenter);
    }
}