using UnityEngine;
using UnityEditor;

namespace Daisen.Editor
{
    [FilePath("UserSettings/StandingSilhouetteSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class StandingSilhouetteSettings : ScriptableSingleton<StandingSilhouetteSettings>
    {
        public Color wireColor = new Color(0.20f, 0.85f, 1.00f, 0.90f);
        public bool useSeparateFillColor = true;
        public Color fillColor = new Color(0.20f, 0.85f, 1.00f, 0.18f);

        public void Save() => Save(true);
    }

    public sealed class StandingSilhouetteModule : IPlayerSilhouetteModule
    {
        public string ModuleName => "Standing Outline";

        public void UpdateCache(System.Collections.Generic.IReadOnlyList<PlayerSilhouetteTarget> targets, PlayerSilhouetteSettings settings)
        {
        }

        public void Draw(PlayerSilhouetteTarget target, PlayerSilhouetteSettings settings)
        {
            var mySettings = StandingSilhouetteSettings.instance;
            var fill = mySettings.useSeparateFillColor ? mySettings.fillColor : new Color(mySettings.wireColor.r, mySettings.wireColor.g, mySettings.wireColor.b, settings.fillAlpha);
            
            SilhouetteDrawerUtility.DrawSilhouette3D(
                target.FeetPosition, target.Rotation,
                target.Height, mySettings.wireColor,
                settings.shoulderWidth, settings.hipWidth, settings.waistWidth, settings.headRadius,
                settings.edgeCount, settings.wireThickness,
                settings.fillEnabled, fill);
        }

        public void DrawSettingsGUI()
        {
            var mySettings = StandingSilhouetteSettings.instance;
            var obj = new SerializedObject(mySettings);
            obj.Update();

            EditorGUILayout.PropertyField(obj.FindProperty("wireColor"), new GUIContent("Color"));
            EditorGUILayout.PropertyField(obj.FindProperty("useSeparateFillColor"));
            if (obj.FindProperty("useSeparateFillColor").boolValue)
            {
                EditorGUILayout.PropertyField(obj.FindProperty("fillColor"));
            }

            if (obj.ApplyModifiedProperties())
            {
                mySettings.Save();
                SceneView.RepaintAll();
            }
        }
    }
}
