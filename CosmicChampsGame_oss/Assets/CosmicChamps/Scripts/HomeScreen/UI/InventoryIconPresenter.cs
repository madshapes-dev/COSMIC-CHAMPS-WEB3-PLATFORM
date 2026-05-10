using CosmicChamps.Common;
using CosmicChamps.Data;
using CosmicChamps.UI;
using CosmicChamps.Utils;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class InventoryIconPresenter : AbstractPoolablePresenter<InventoryIconPresenter.Model, Unit, InventoryIconPresenter>
    {
        public class Factory : AbstractFactory
        {
        }

        public readonly struct Model
        {
            public readonly PlayerCardShards CardShards;

            public Model (PlayerCardShards cardShards)
            {
                CardShards = cardShards;
            }
        }

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private TextMeshProUGUI _amount;

        [SerializeField]
        private Color _availableColor = Color.white;

        [SerializeField]
        private Color _notAvailableColor = Color.grey;

        [Inject]
        private IShardsViewProvider _shardsViewProvider;

        protected override void Refresh ()
        {
            base.Refresh ();

            var cardShards = model.CardShards;

            async UniTaskVoid LoadSprite ()
            {
                var sprite = await _shardsViewProvider.GetShardsIcon (cardShards.Id);
                await _icon.DOSpriteFade (sprite);
            }

            var available = cardShards.Amount > 0;

            _icon.sprite = DOTweenModuleUI.TransparentPixelSprite;
            _icon.color = available ? _availableColor : _notAvailableColor;
            _amount.text = cardShards.Amount.FormatShardsCost ();
            _amount.SetVisible (available);

            LoadSprite ().Forget ();
        }
    }
}