using System;
using CosmicChamps.HomeScreen.Model;
using CosmicChamps.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.HomeScreen.UI
{
    public class ShipCardPresenter : AbstractPoolablePresenter<
        ShipCardPresenterModel,
        ShipCardPresenter.Callbacks,
        ShipCardPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action<DeckCardPresenter> OnClicked;

            public Callbacks (Action<DeckCardPresenter> onClicked)
            {
                OnClicked = onClicked;
            }
        }
        
        public class Factory : AbstractFactory
        {
        }

        [SerializeField]
        private Image _avatar;

        [SerializeField]
        private TextMeshProUGUI _nameCaption;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private RectTransform _empty;

        [SerializeField]
        private RectTransform _card;
        
    }
}