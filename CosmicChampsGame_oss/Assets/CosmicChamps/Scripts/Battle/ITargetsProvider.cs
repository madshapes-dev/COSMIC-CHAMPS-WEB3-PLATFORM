using System.Collections.Generic;
using CosmicChamps.Battle.Data;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;

namespace CosmicChamps.Battle
{
    public interface ITargetsProvider
    {
        void GetTargetsFor (IUnit unit, List<ITarget> targets);
        void GetTargetsFor (PlayerTeam team, List<ITarget> targets);
    }
}