using System;
using TMPro;
using UniRx;

namespace ThirdParty.Extensions
{
    public static class TMP_InputExtensions
    {
        public static IObservable<Unit> OnValueChangedAsUnitObservable (this TMP_InputField inputField) =>
            inputField
                .onValueChanged
                .AsObservable ()
                .AsUnitObservable ();
    }
}