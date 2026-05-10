using CosmicChamps.HomeScreen.UI;
using CosmicChamps.UI;
using ThirdParty.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace CosmicChamps.HomeScreen
{
    public class RootInstaller : MonoInstaller
    {
        [FormerlySerializedAs ("_signInPresenter")]
        [Header ("Sign Presenters"), SerializeField]
        private EmailSignInPresenter _emailSignInPresenter;

        [SerializeField]
        private SignUpPresenter _signUpPresenter;

        [SerializeField]
        private SignPresentersGroup _signPresentersGroup;

        [SerializeField]
        private ResetPasswordStep1Presenter _resetPasswordStep1Presenter;

        [SerializeField]
        private ResetPasswordStep2Presenter _resetPasswordStep2Presenter;

        [SerializeField]
        private LandingPresenter _landingPresenter;

        [SerializeField]
        private SignInPresenter _signInPresenter;

        [Header ("Game Presenters"), SerializeField]
        private GamePresenter _gamePresenter;

        [SerializeField]
        private WaitingPresenter _waitingPresenter;

        /*[SerializeField]
        private ConnectToWalletPresenter _connectToWalletPresenter;*/

        [SerializeField]
        private ProfilePresenter _profilePresenter;

        [SerializeField]
        private GamePresentersGroup _gamePresentersGroup;

        [SerializeField]
        private DisclaimerPopup _disclaimerPopup;

        [SerializeField]
        private CardInfoPresenter _cardInfoPresenter;

        [SerializeField]
        private NewsPresenter _newsPresenter;

        [SerializeField]
        private CompleteSignupPresenter _completeSignupPresenter;

        [FormerlySerializedAs ("_connectWalletPresenter")]
        [SerializeField]
        private WalletsPresenter _walletsPresenter;

        [SerializeField]
        private BindEmailPresenter _bindEmailPresenter;

        [SerializeField]
        private GuestAccountConnectWalletPopup _guestAccountConnectWalletPopup;

        [SerializeField]
        private HintPresenter _hintPresenter;

        [Header ("Inventory"), SerializeField]
        private InventoryPresenter _inventoryPresenter;

        [SerializeField]
        private InventoryIconPresenter _inventoryIconPresenterPrefab;

        [SerializeField]
        private BattleRewardPresenter _battleRewardPresenter;

        [Header ("Decks"), SerializeField]
        private DeckPresenter _deckPresenter;

        [SerializeField]
        private DeckSwitcherToggle _deckSwitcherTogglePrefab;

        [SerializeField]
        private DeckCardPresenter _deckCardPresenterPrefab;

        [Header ("Sign Up Validator"), SerializeField]
        private SignUpValidator.Errors _signUpValidatorErrors;

        public override void InstallBindings ()
        {
            Container.BindInstance<LandingPresenter> (_landingPresenter);
            Container.BindInstance<SignInPresenter> (_signInPresenter);
            Container.BindInstance<EmailSignInPresenter> (_emailSignInPresenter);
            Container.BindInstance<SignUpPresenter> (_signUpPresenter);
            Container.BindInstance<SignPresentersGroup> (_signPresentersGroup);
            Container.BindInstance<ResetPasswordStep1Presenter> (_resetPasswordStep1Presenter);
            Container.BindInstance<ResetPasswordStep2Presenter> (_resetPasswordStep2Presenter);
            Container.BindInstance<GamePresenter> (_gamePresenter);
            Container.BindInstance<WaitingPresenter> (_waitingPresenter);
            Container.BindInstance<GamePresentersGroup> (_gamePresentersGroup);
            Container.BindInstance<DisclaimerPopup> (_disclaimerPopup);
            Container.BindInstance<WalletsPresenter> (_walletsPresenter);
            Container.BindInstance<ProfilePresenter> (_profilePresenter);
            Container.BindInstance<DeckPresenter> (_deckPresenter);
            Container.BindInstance<CardInfoPresenter> (_cardInfoPresenter);
            Container.BindInstance<NewsPresenter> (_newsPresenter);
            Container.BindInstance<CompleteSignupPresenter> (_completeSignupPresenter);
            Container.BindInstance<BindEmailPresenter> (_bindEmailPresenter);
            Container.BindInstance<InventoryPresenter> (_inventoryPresenter);
            Container.BindInstance<GuestAccountConnectWalletPopup> (_guestAccountConnectWalletPopup);
            Container.BindInstance<BattleRewardPresenter> (_battleRewardPresenter);
            Container.BindInstance<HintPresenter> (_hintPresenter);
            Container.Bind<SignUpValidator> ().AsSingle ().WithArguments (_signUpValidatorErrors);
            Container.BindPoolablePresenterPool<DeckCardPresenter, DeckCardPresenter.Factory> (_deckCardPresenterPrefab);
            Container.BindPoolablePresenterPool<DeckSwitcherToggle, DeckSwitcherToggle.Factory> (_deckSwitcherTogglePrefab);
            Container.BindPoolablePresenterPool<InventoryIconPresenter, InventoryIconPresenter.Factory> (
                _inventoryIconPresenterPrefab);
            Container.AddDisabler ();
        }
    }
}