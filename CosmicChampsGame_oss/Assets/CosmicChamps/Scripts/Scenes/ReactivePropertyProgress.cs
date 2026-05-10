using System;
using UniRx;

namespace CosmicChamps
{
    public class ReactivePropertyProgress : IReadOnlyReactiveProperty<float>, IProgress<float>
    {
        private readonly FloatReactiveProperty reactiveProperty = new();

        public float Value => reactiveProperty.Value;
        public bool HasValue => reactiveProperty.HasValue;

        public void Report (float value) => reactiveProperty.Value = value;
        public IDisposable Subscribe (IObserver<float> observer) => reactiveProperty.Subscribe (observer);
    }
}