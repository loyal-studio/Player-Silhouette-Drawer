using System.Collections.Generic;
using UnityEngine;

namespace Daisen.Editor
{
    public interface IPlayerSilhouetteReader
    {
        void FindTargets(List<Component> results);
        bool TryGetTargetData(Component component, out PlayerSilhouetteTarget targetData);
    }
}
