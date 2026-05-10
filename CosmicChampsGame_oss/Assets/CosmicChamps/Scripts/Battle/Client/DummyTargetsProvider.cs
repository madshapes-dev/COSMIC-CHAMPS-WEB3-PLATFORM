using System.Collections.Generic;
using CosmicChamps.Battle.Data;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;

namespace CosmicChamps.Battle.Client
{
    public class DummyTargetsProvider : ITargetsProvider
    {
        public void GetTargetsFor (IUnit unit, List<ITarget> targets)
        {
        }

        public void GetTargetsFor (PlayerTeam team, List<ITarget> targets)
        {
        }
    }
}