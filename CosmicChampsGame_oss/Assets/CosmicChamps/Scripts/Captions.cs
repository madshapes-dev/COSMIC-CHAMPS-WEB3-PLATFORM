using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

namespace CosmicChamps
{
    [Serializable]
    public class Captions
    {
        [SerializeField]
        private LocalizedString _playerIdCopied;

        [SerializeField]
        private LocalizedString _profileNickname;

        [SerializeField]
        private LocalizedString _connectWallet;

        [SerializeField]
        private LocalizedString _walletIdCopied;

        [SerializeField]
        private LocalizedString _cardLevel;

        [SerializeField]
        private LocalizedString _battleRewardOldScore;

        [SerializeField]
        private LocalizedString _battleRewardNewScore;

        [SerializeField]
        private LocalizedString _playerLevel;
        
        [SerializeField]
        private LocalizedString _maxLevel;

        [SerializeField]
        private LocalizedString _cardUnlockAt;

        [SerializeField]
        private ErrorCaptions _errors;

        public string PlayerIdCopied => _playerIdCopied.GetLocalizedString ();

        public string ConnectWallet => _connectWallet.GetLocalizedString ();

        public string WalletIdCopied => _walletIdCopied.GetLocalizedString ();

        public string MaxLevel => _maxLevel.GetLocalizedString ();

        public ErrorCaptions Errors => _errors;

        private string FormatSmartString (LocalizedString str, params (string, object)[] args)
        {
            str.Arguments = new object[] { args.ToDictionary (x => x.Item1, x => x.Item2) };
            return str.GetLocalizedString ();
        }

        public string ProfileNickname (params (string, object)[] args) => FormatSmartString (_profileNickname, args);

        public string CardLevel (int level) => FormatSmartString (_cardLevel, ("level", level));

        public string BattleRewardOldScore (int old) => FormatSmartString (
            _battleRewardOldScore,
            ("old", old));

        public string BattleRewardNewScore (int @new) => FormatSmartString (
            _battleRewardNewScore,
            ("new", @new));

        public string PlayerLevel (int level) => FormatSmartString (_playerLevel, ("level", level));

        public string CardUnlockAt (int level) => FormatSmartString (_cardUnlockAt, ("level", level));
    }
}