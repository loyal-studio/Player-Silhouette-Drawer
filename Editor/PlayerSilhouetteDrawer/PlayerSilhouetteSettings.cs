using UnityEditor;
using UnityEngine;

namespace Daisen.Editor
{
    [FilePath("UserSettings/PlayerSilhouetteSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class PlayerSilhouetteSettings : ScriptableSingleton<PlayerSilhouetteSettings>
    {
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
        }

        public void Save() => Save(true);
    }
}
