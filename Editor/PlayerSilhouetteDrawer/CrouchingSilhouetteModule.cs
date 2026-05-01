using UnityEngine;
using UnityEditor;

namespace LOYAL.Editor
{
    public sealed class CrouchingSilhouetteSettings : ScriptableObject
    {
        private const string PREFS_KEY = "LOYAL.Editor.CrouchingSilhouetteSettings";

        private static CrouchingSilhouetteSettings s_Instance;
        public static CrouchingSilhouetteSettings instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = CreateInstance<CrouchingSilhouetteSettings>();
                    s_Instance.hideFlags = HideFlags.DontSave;
                    s_Instance.Load();
                }
                return s_Instance;
            }
        }

        public bool renderCrouch = true;
        [Range(0.30f, 0.90f)]
        public float crouchRatio = 0.53f;
        public Color wireColor = new Color(1.00f, 0.75f, 0.20f, 0.90f);
        public bool useSeparateFillColor = true;
        public Color fillColor = new Color(1.00f, 0.75f, 0.20f, 0.18f);

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
