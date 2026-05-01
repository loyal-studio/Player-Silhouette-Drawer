using UnityEngine;
using UnityEditor;

namespace LOYAL.Editor
{
    public sealed class StandingSilhouetteSettings : ScriptableObject
    {
        private const string PREFS_KEY = "LOYAL.Editor.StandingSilhouetteSettings";

        private static StandingSilhouetteSettings s_Instance;
        public static StandingSilhouetteSettings instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = CreateInstance<StandingSilhouetteSettings>();
                    s_Instance.hideFlags = HideFlags.DontSave;
                    s_Instance.Load();
                }
                return s_Instance;
            }
        }

        public Color wireColor = new Color(0.20f, 0.85f, 1.00f, 0.90f);
        public bool useSeparateFillColor = true;
        public Color fillColor = new Color(0.20f, 0.85f, 1.00f, 0.18f);

        public void Load()
        {
            var json = EditorPrefs.GetString(PREFS_KEY, string.Empty);
            if (!string.IsNullOrEmpty(json))
            {
                var oldFlags = hideFlags;
                hideFlags = HideFlags.None;
                EditorJsonUtility.FromJsonOverwrite(json, this);
                hideFlags = oldFlags;
            }
        }

        public void Save()
        {
            var oldFlags = hideFlags;
            hideFlags = HideFlags.None;
            var json = EditorJsonUtility.ToJson(this);
            hideFlags = oldFlags;
            EditorPrefs.SetString(PREFS_KEY, json);
        }
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
