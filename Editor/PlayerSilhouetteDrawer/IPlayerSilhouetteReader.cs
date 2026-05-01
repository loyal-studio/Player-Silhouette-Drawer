using System.Collections.Generic;
using UnityEngine;

namespace LOYAL.Editor
{
    public interface IPlayerSilhouetteReader
    {
        void FindTargets(List<Component> results);
        bool TryGetTargetData(Component component, out PlayerSilhouetteTarget targetData);
    }
}
