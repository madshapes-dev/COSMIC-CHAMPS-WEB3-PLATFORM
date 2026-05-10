using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace CosmicChamps.Preview
{
    public class EntryPoint : MonoBehaviour
    {
        private async void Start ()
        {
            await Addressables.LoadSceneAsync (Scenes.Level, LoadSceneMode.Additive);
            await Addressables.LoadSceneAsync (Scenes.ClientBattle, LoadSceneMode.Additive);
            var shipControllers = FindObjectsByType<ShipFlyController> (FindObjectsSortMode.None);
            foreach (var shipController in shipControllers)
            {
                shipController.LandShip ();
            }
        }
    }
}