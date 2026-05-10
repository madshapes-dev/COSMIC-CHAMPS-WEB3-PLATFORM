using UnityEngine;
using Zenject;

namespace CosmicChamps
{
    public class CaptionsInstaller : MonoInstaller
    {
        [SerializeField]
        private Captions _captions;

        public override void InstallBindings ()
        {
            Container
                .Bind<Captions> ()
                .FromInstance (_captions);
        }
    }
}