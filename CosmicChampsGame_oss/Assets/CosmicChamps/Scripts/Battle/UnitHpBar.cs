using System;
using System.Linq;
using CosmicChamps.Battle.Data;
using CosmicChamps.UI;
using ThirdParty.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CosmicChamps.Battle
{
    public class UnitHPBar : AbstractUnitHPBar
    {
        [Serializable]
        private class Skin
        {
            [SerializeField]
            private PlayerTeam _playerTeam;

            [SerializeField]
            private Sprite _labelBackground;

            [SerializeField]
            private Sprite _bar;

            public PlayerTeam PlayerTeam => _playerTeam;

            public void Apply (Image labelBackground, Image bar)
            {
                labelBackground.sprite = _labelBackground;
                bar.sprite = _bar;
            }
        }

        [FormerlySerializedAs ("_label")]
        [SerializeField]
        private TextMeshProUGUI _caption;

        [SerializeField]
        private Progressbar _progressbar;

        [SerializeField]
        private Image _labelBackground;

        [SerializeField]
        private Image _bar;

        [SerializeField]
        private Skin[] _skins;

        [SerializeField]
        private RectTransform _levelBlock;

        [SerializeField]
        private TextMeshProUGUI _levelCaption;

        private RectTransform _rectTransform;

        private void Awake ()
        {
            _rectTransform = transform as RectTransform;
        }

        public override void SetPlayerTeam (PlayerTeam playerTeam)
        {
            var skin = _skins.FirstOrDefault (x => x.PlayerTeam == playerTeam);
            if (skin == null)
                throw new InvalidOperationException ($"Cannot find skin for team {playerTeam}");

            skin.Apply (_labelBackground, _bar);
        }

        public override void SetValue (UnitHp hp, bool immediate = false)
        {
            _progressbar.SetValue (hp.NormalizedValue);
        }

        public override void Align (Vector3 worldPosition) => _rectTransform.AlignToWorldPosition (worldPosition);

        public override void SetLevel (int level)
        {
            _levelCaption.text = level.ToString ();
            _levelBlock.SetVisible (level > 0);
        }
    }
}