using System;
using CosmicChamps.Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class QuickStart1 : MonoBehaviour
{
    [SerializeField]
    private ShipFlyController[] _controllers;

    void Start ()
    {
        foreach (var controller in _controllers)
        {
            controller.LandImmediate();
        }
        /*_controller.LandImmediate ();
        await UniTask.Delay (TimeSpan.FromSeconds (3f));
        _controller.Explode (BaseUnitType.Base);
        await UniTask.Delay (TimeSpan.FromSeconds (4f));
        _controller.Explode (BaseUnitType.Turret);
        await UniTask.Delay (TimeSpan.FromSeconds (4f));
        _controller.Explode (BaseUnitType.Shield);*/
    }
}