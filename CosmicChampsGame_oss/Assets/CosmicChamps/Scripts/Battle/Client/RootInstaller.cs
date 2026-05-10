using CosmicChamps.Battle.UI;
using CosmicChamps.Battle.Units;
using CosmicChamps.UI;
using ThirdParty.Extensions;
using ThirdParty.Extensions.Attributes;
using UnityEngine;
using Zenject;

namespace CosmicChamps.Battle.Client
{
    public class RootInstaller : MonoInstaller
    {
        [SerializeField]
        private Camera _camera;

        [SerializeField, ArrayElementTitle (CameraPlaceholder.PlayerPositionField)]
        private CameraPlaceholder[] _cameraPlaceholders;

        [SerializeField]
        private HUDPresenter _hudPresenter;

        [SerializeField]
        private CardPresenter _cardPresenterPrefab;

        [SerializeField]
        private CountdownPresenter _countdownPresenter;

        [SerializeField]
        private BattleResultPresenter _battleResultPresenter;

        [SerializeField]
        private PopupMessagesPresenter _popupMessagesPresenter;

        [SerializeField]
        private ForfeitPopup _forfeitPopup;

        [SerializeField]
        private PopupMessagePresenter _popupMessagePrefab;

        [SerializeField]
        private BaseUnitNetworkBehaviour _baseUnitPrefab;

        [SerializeField]
        private BaseUnitNetworkBehaviour _towerUnitPrefab;

        [SerializeField]
        private BaseHPPrefabs _baseHpPrefabs;

        [SerializeField]
        private Emojis _emojis;

        [SerializeField]
        private RectTransform _unitHpBarContainer;

        [SerializeField]
        private UnitHPBar _unitHpBarPrefab;

        [SerializeField]
        private EmojiButton _emojiButtonPrefab;

        [SerializeField]
        private Transform _emojiButtonsContainer;

        public override void InstallBindings ()
        {
            #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
            Container
                .BindInterfacesAndSelfTo<BattleService> ()
                .AsSingle ()
                .NonLazy ();

            Container
                .BindAsSingle<CameraService> ()
                .WithArguments (_cameraPlaceholders, _camera);

            Container.BindInstance<BaseHPPrefabs> (_baseHpPrefabs);
            Container.BindInstance<Emojis> (_emojis);
            Container.BindAsSingle<ITargetsProvider, DummyTargetsProvider> ();
            Container.BindInstance<CountdownPresenter> (_countdownPresenter);
            Container.BindInstance<HUDPresenter> (_hudPresenter);
            Container.BindInstance<BattleResultPresenter> (_battleResultPresenter);
            Container.BindInstance<PopupMessagesPresenter> (_popupMessagesPresenter);
            Container.BindInstance<ForfeitPopup> (_forfeitPopup);
            Container.BindPoolablePresenterPool<CardPresenter, CardPresenter.Factory> (_cardPresenterPrefab);
            Container
                .BindFactory<PopupMessagePresenter, PopupMessagePresenter.Factory> ()
                .FromMonoPoolableMemoryPool (
                    x => x
                        .FromComponentInNewPrefab (_popupMessagePrefab)
                        .UnderTransform (_popupMessagesPresenter.transform));
            Container.BindAsSingle<RegularUnitNetworkBehaviour.ClientFactory> ();
            Container.BindAsSingle<RegularUnitNetworkBehaviour.PreviewFactory> ();
            Container
                .BindAsSingle<BaseUnitNetworkBehaviour.ClientFactory> ()
                .WithArguments (new BaseUnitNetworkBehaviour.FactoryArgs (_baseUnitPrefab, _towerUnitPrefab));

            Container
                .Bind<ITimeProvider> ()
                .To<UnityRealtimeSinceStartupTimeProvider> ()
                .AsSingle ();

            Container
                .BindFactory<AbstractUnitHPBar, UnitHPBarFactory> ()
                .FromMonoPoolableMemoryPool (
                    x => x
                        .To<AbstractUnitHPBar> ()
                        .FromComponentInNewPrefab (_unitHpBarPrefab)
                        .UnderTransform (_unitHpBarContainer));

            Container
                .BindFactory<EmojiButton, EmojiButton.Factory> ()
                .FromComponentInNewPrefab (_emojiButtonPrefab);

            Container.AddDisabler ();
            #endif
        }
    }
}