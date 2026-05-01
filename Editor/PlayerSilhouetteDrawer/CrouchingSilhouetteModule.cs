using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Daisen.Editor
{
    public sealed class CrouchingSilhouetteModule : IPlayerSilhouetteModule
    {
        private double _lastHeightsUpdate;
        private readonly Dictionary<Component, float> _crouchHeightCache = new Dictionary<Component, float>();
        private readonly List<MonoBehaviour> _monoBehaviourCache = new List<MonoBehaviour>();

        public void UpdateCache(IReadOnlyList<PlayerSilhouetteTarget> targets, PlayerSilhouetteSettings settings)
        {
            if (EditorApplication.timeSinceStartup - _lastHeightsUpdate < 1.0) return;
            _lastHeightsUpdate = EditorApplication.timeSinceStartup;

            _crouchHeightCache.Clear();
            var currentStage = StageUtility.GetCurrentStageHandle();

            for (int i = 0; i < targets.Count; i++)
            {
                var t = targets[i];
                if (!t.Component || !t.GameObject) continue;
                
                var stage = StageUtility.GetStageHandle(t.GameObject);
                if (stage != currentStage) continue;

                _crouchHeightCache[t.Component] = ResolveCrouchHeightSlow(t, settings.crouchRatio);
            }
        }

        public void Draw(PlayerSilhouetteTarget target, PlayerSilhouetteSettings settings)
        {
            if (!settings.renderCrouch) return;

            float standH = target.StandingHeight;
            if (!_crouchHeightCache.TryGetValue(target.Component, out float crouchH))
            {
                crouchH = ResolveCrouchHeightSlow(target, settings.crouchRatio);
                _crouchHeightCache[target.Component] = crouchH;
            }

            if (crouchH <= 0.1f) return;

            float wScale = crouchH / standH;
            SilhouetteDrawerUtility.DrawSilhouette3D(
                target.FeetPosition, target.Rotation, settings, crouchH, settings.crouchColor,
                settings.shoulderWidth * wScale,
                settings.hipWidth * wScale,
                settings.waistWidth * wScale,
                settings.headRadius * Mathf.Sqrt(wScale),
                true);
        }

        private float ResolveCrouchHeightSlow(PlayerSilhouetteTarget target, float fallbackRatio)
        {
            _monoBehaviourCache.Clear();
            target.GameObject.GetComponents(_monoBehaviourCache);
            
            for (int i = 0; i < _monoBehaviourCache.Count; i++)
            {
                var comp = _monoBehaviourCache[i];
                if (comp == null) continue;
                
                using var so = new SerializedObject(comp);
                var it = so.GetIterator();
                
                while (it.NextVisible(true))
                {
                    if (it.propertyType != SerializedPropertyType.ObjectReference) continue;
                    if (it.objectReferenceValue is not ScriptableObject sobj) continue;

                    using var innerSo = new SerializedObject(sobj);
                    var heightProp = innerSo.FindProperty("heightBySlide");
                    
                    if (heightProp != null && heightProp.floatValue > 0.1f)
                    {
                        return heightProp.floatValue;
                    }
                }
            }
            return target.StandingHeight * fallbackRatio;
        }
    }
}
