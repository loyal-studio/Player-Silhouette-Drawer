using System.Collections.Generic;
using UnityEngine;

namespace Daisen.Editor
{
    public struct PlayerSilhouetteTarget
    {
        public Component Component;
        public GameObject GameObject;
        public Vector3 FeetPosition;
        public Quaternion Rotation;
        public float StandingHeight;
    }

    public interface IPlayerSilhouetteReader
    {
        void FindTargets(List<Component> results);
        bool TryGetTargetData(Component component, out PlayerSilhouetteTarget targetData);
    }

    public interface IPlayerSilhouetteModule
    {
        void UpdateCache(IReadOnlyList<PlayerSilhouetteTarget> targets, PlayerSilhouetteSettings settings);
        void Draw(PlayerSilhouetteTarget target, PlayerSilhouetteSettings settings);
    }
}
