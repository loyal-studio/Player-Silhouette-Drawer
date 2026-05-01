using UnityEditor;
using UnityEngine;

namespace LOYAL.Editor
{
    public sealed class PlayerSilhouetteSettings : ScriptableObject
    {
        private const string PREFS_KEY = "LOYAL.Editor.PlayerSilhouetteSettings";

        private static PlayerSilhouetteSettings s_Instance;
        public static PlayerSilhouetteSettings instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = CreateInstance<PlayerSilhouetteSettings>();
                    s_Instance.hideFlags = HideFlags.DontSave;
                    s_Instance.Load();
                }
                return s_Instance;
            }
        }

        [Header("Visibility")]
        public bool showSilhouette = true;
        public bool showOnlySelected = false;

        [Header("Proportions")]
        [Range(0.25f, 0.65f)] public float shoulderWidth = 0.40f;
        [Range(0.20f, 0.60f)] public float hipWidth = 0.36f;
        [Range(0.15f, 0.55f)] public float waistWidth = 0.28f;
        [Range(0.07f, 0.18f)] public float headRadius = 0.11f;

        [Header("Rendering")]
        public int edgeCount = 12;
        [Range(0.5f, 5.0f)] public float wireThickness = 2.0f;

        [Header("Fill Options")]
        public bool fillEnabled = true;
        [Range(0f, 1f)] public float fillAlpha = 0.18f;

        public void ResetToDefaults()
        {
            showSilhouette = true;
            showOnlySelected = false;
            shoulderWidth = 0.40f;
            hipWidth = 0.36f;
            waistWidth = 0.28f;
            headRadius = 0.11f;
            edgeCount = 12;
            wireThickness = 2.0f;
            fillEnabled = true;
            fillAlpha = 0.18f;
            Save();
        }

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
}
