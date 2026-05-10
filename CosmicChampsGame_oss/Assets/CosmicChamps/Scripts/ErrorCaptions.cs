using System;
using UnityEngine;
using UnityEngine.Localization;

namespace CosmicChamps
{
    [Serializable]
    public class ErrorCaptions
    {
        [SerializeField]
        private LocalizedString _emptyNickname;

        [SerializeField]
        private LocalizedString _walletNotConnected;
        
        [SerializeField]
        private LocalizedString _notEnoughShards;

        [SerializeField]
        private LocalizedString _cardLevelCapReached;

        [SerializeField]
        private LocalizedString _cardAlreadyTopLevel;

        [SerializeField]
        private LocalizedString _emptyPromoCode;

        public string EmptyNickname => _emptyNickname.GetLocalizedString ();
        public string NotEnoughShards => _notEnoughShards.GetLocalizedString ();
        public string WalletNotConnected => _walletNotConnected.GetLocalizedString ();
        public string CardLevelCapReached => _cardLevelCapReached.GetLocalizedString ();
        public string EmptyPromoCode => _emptyPromoCode.GetLocalizedString ();
    }
}