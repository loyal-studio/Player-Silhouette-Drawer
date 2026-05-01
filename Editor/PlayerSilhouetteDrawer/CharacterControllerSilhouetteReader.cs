using System.Collections.Generic;
using UnityEngine;

namespace Daisen.Editor
{
    public sealed class CharacterControllerSilhouetteReader : IPlayerSilhouetteReader
    {
        public void FindTargets(List<Component> results)
        {
#if UNITY_2022_2_OR_NEWER
            var ccs = Object.FindObjectsByType<CharacterController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
            var ccs = Object.FindObjectsOfType<CharacterController>(true);
#endif
            for (int i = 0; i < ccs.Length; i++)
            {
                results.Add(ccs[i]);
            }
        }

        public bool TryGetTargetData(Component component, out PlayerSilhouetteTarget targetData)
        {
            if (component is CharacterController cc)
            {
                float standH = cc.height;
                Vector3 feet = cc.transform.position;
                Quaternion rot = cc.transform.rotation;
                
                float centerOffsetY = cc.center.y - standH * 0.5f;
                feet += rot * new Vector3(cc.center.x, centerOffsetY, cc.center.z);

                targetData = new PlayerSilhouetteTarget
                {
                    Component = cc,
                    GameObject = cc.gameObject,
                    FeetPosition = feet,
                    Rotation = rot,
                    StandingHeight = standH
                };
                return true;
            }

            targetData = default;
            return false;
        }
    }
}
