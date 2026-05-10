using System.Collections.Generic;
using System.Collections.ObjectModel;
using UniRx.Toolkit;

namespace ThirdParty.Extensions.Components
{
    public abstract class TrackingObjectPool<T> : ObjectPool<T> where T : UnityEngine.Component
    {
        private readonly List<T> rented = new();

        public ReadOnlyCollection<T> Rented => rented.AsReadOnly ();

        protected override void OnBeforeRent (T instance)
        {
            base.OnBeforeRent (instance);
            rented.Add (instance);
        }

        protected override void OnBeforeReturn (T instance)
        {
            base.OnBeforeReturn (instance);
            rented.Remove (instance);
        }

        public void ReturnRented ()
        {
            foreach (var instance in rented.ToArray ())
            {
                Return (instance);
            }
        }
    }
}