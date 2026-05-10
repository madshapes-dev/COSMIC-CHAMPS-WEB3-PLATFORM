using System;
using CosmicChamps.Battle.Data;
using CosmicChamps.UI;
using DG.Tweening;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.Battle.UI
{
    public class EnergyBarPresenter : AbstractPresenter<EnergyBarPresenter.Model, Unit, EnergyBarPresenter>
    {
        public readonly struct Model
        {
            public readonly PlayerEnergy Energy;

            public Model (PlayerEnergy energy)
            {
                Energy = energy;
            }
        }

        [SerializeField]
        private Image _bar;

        [SerializeField]
        private TextMeshProUGUI _caption;

        [SerializeField]
        private TextMeshProUGUI _fullEnergyCaption;

        [SerializeField]
        private float _captionFadeDuration = 0.2f;

        [SerializeField]
        private float _energySpendDuration = 0.2f;

        private IDisposable _fillTween;
        private IDisposable _fullEnergyCaptionTween;

        private void OnEnergy (float value)
        {
            var energy = model.Energy;
            var normalizedEnergyTargetValue = (value + energy.GrowRate * _energySpendDuration) / energy.MaxValue;

            _fillTween?.Dispose ();
            _fillTween = DOTween
                .Sequence ()
                .Append (_bar.DOFillAmount (normalizedEnergyTargetValue, _energySpendDuration))
                .Append (_bar.DOFillAmount (1f, (energy.MaxValue - normalizedEnergyTargetValue) / energy.GrowRate))
                .AsDisposable ()
                .AddTo (_modelDisposables);
        }

        private void OnIntEnergy (int value)
        {
            _caption.AnimateTextChangeThroughFade (value.ToString (), _captionFadeDuration);

            _fullEnergyCaptionTween?.Dispose ();
            var fullEnergyCaptionAlpha = _fullEnergyCaption.color.a;
            if (value == model.Energy.MaxValue && fullEnergyCaptionAlpha.EqualsWithEps (0f))
                _fullEnergyCaptionTween = _fullEnergyCaption
                    .DOFade (1f, _captionFadeDuration)
                    .AsDisposable ()
                    .AddTo (_modelDisposables);

            if (value < model.Energy.MaxValue && fullEnergyCaptionAlpha.EqualsWithEps (1f))
                _fullEnergyCaptionTween = _fullEnergyCaption
                    .DOFade (0f, _captionFadeDuration)
                    .AsDisposable ()
                    .AddTo (_modelDisposables);
        }

        protected override void Refresh ()
        {
            base.Refresh ();

            var reactiveEnergy = model
                .Energy
                .ObserveEveryValueChanged (x => x.Value)
                .ToReactiveProperty ();

            reactiveEnergy
                .Subscribe (OnEnergy)
                .AddTo (_modelDisposables);

            reactiveEnergy
                .Select (Mathf.FloorToInt)
                .ToReactiveProperty ()
                .Subscribe (OnIntEnergy)
                .AddTo (_modelDisposables);

            _fullEnergyCaption.color = _fullEnergyCaption.color.WithA (0f);
        }
    }
}