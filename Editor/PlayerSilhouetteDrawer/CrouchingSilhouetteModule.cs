using UnityEngine;
using UnityEditor;

namespace Daisen.Editor
{
    [FilePath("UserSettings/CrouchingSilhouetteSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class CrouchingSilhouetteSettings : ScriptableSingleton<CrouchingSilhouetteSettings>
    {
        public bool renderCrouch = true;
        [Range(0.30f, 0.90f)]
        public float crouchRatio = 0.53f;
        public Color wireColor = new Color(1.00f, 0.75f, 0.20f, 0.90f);
        public bool useSeparateFillColor = true;
        public Color fillColor = new Color(1.00f, 0.75f, 0.20f, 0.18f);

        public void Save() => Save(true);
    }

    public sealed class CrouchingSilhouetteModule : IPlayerSilhouetteModule
    {
        public string ModuleName => "Crouched Outline";
        
        private Vector3[] _crouchPts;

        public void UpdateCache(System.Collections.Generic.IReadOnlyList<PlayerSilhouetteTarget> targets, PlayerSilhouetteSettings settings)
        {
            var mySettings = CrouchingSilhouetteSettings.instance;
            if (!mySettings.renderCrouch || targets.Count == 0) return;

            int needed = targets.Count;
            if (_crouchPts == null || _crouchPts.Length != needed)
            {
                _crouchPts = new Vector3[needed];
            }

            for (int i = 0; i < needed; i++)
            {
                _crouchPts[i] = targets[i].FeetPosition;
            }
        }

        public void Draw(PlayerSilhouetteTarget target, PlayerSilhouetteSettings settings)
        {
            var mySettings = CrouchingSilhouetteSettings.instance;
            if (!mySettings.renderCrouch) return;

            var fill = mySettings.useSeparateFillColor ? mySettings.fillColor : new Color(mySettings.wireColor.r, mySettings.wireColor.g, mySettings.wireColor.b, settings.fillAlpha);
            
            float h = target.Height * mySettings.crouchRatio;

            SilhouetteDrawerUtility.DrawSilhouette3D(
                target.FeetPosition, target.Rotation,
                h, mySettings.wireColor,
                settings.shoulderWidth, settings.hipWidth, settings.waistWidth, settings.headRadius,
                settings.edgeCount, settings.wireThickness,
                settings.fillEnabled, fill);
        }

        public void DrawSettingsGUI()
        {
            var mySettings = CrouchingSilhouetteSettings.instance;
            var obj = new SerializedObject(mySettings);
            obj.Update();

            EditorGUILayout.PropertyField(obj.FindProperty("renderCrouch"));
            if (obj.FindProperty("renderCrouch").boolValue)
            {
                EditorGUILayout.PropertyField(obj.FindProperty("crouchRatio"));
                EditorGUILayout.PropertyField(obj.FindProperty("wireColor"), new GUIContent("Color"));
                EditorGUILayout.PropertyField(obj.FindProperty("useSeparateFillColor"));
                if (obj.FindProperty("useSeparateFillColor").boolValue)
                {
                    EditorGUILayout.PropertyField(obj.FindProperty("fillColor"));
                }
            }

            if (obj.ApplyModifiedProperties())
            {
                mySettings.Save();
                SceneView.RepaintAll();
            }
        }
    }
}
