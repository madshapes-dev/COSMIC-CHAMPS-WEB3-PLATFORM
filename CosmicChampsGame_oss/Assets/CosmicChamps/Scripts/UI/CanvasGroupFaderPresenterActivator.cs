using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using ThirdParty.Extensions.CanvasGroupFader;

namespace CosmicChamps.UI
{
    public class CanvasGroupFaderPresenterActivator : IPresenterActivator
    {
        public UniTask Activate (AbstractPresenter presenter, bool immediate) => presenter.FadeIn (immediate).ToUniTask ();

        public UniTask Deactivate (AbstractPresenter presenter, bool immediate) => presenter.FadeOut (immediate).ToUniTask ();

        public bool IsActive (AbstractPresenter presenter) => presenter.IsVisible ();
    }
}