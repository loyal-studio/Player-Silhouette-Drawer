using UnityEngine;
using UnityEditor;

namespace Daisen.Editor
{
    [FilePath("UserSettings/PlayerSilhouetteSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class PlayerSilhouetteSettings : ScriptableSingleton<PlayerSilhouetteSettings>
    {
        public bool showSilhouette = true;
        public bool showOnlySelected = false;
        public bool renderCrouch = true;

        public float shoulderWidth = 0.40f;
        public float hipWidth = 0.36f;
        public float waistWidth = 0.28f;
        public float headRadius = 0.11f;

        [Range(0.30f, 0.90f)]
        public float crouchRatio = 0.53f;

        [Range(4, 16)]
        public int edgeCount = 8;    
        public float wireThickness = 2.0f;

        public bool fillEnabled = true;
        [Range(0.01f, 1.0f)]
        public float fillAlpha = 0.18f;
        public bool useSeparateFillColors = false;
        public Color fillStandingColor = new Color(0.20f, 0.85f, 1.00f, 0.18f);
        public Color fillCrouchColor = new Color(1.00f, 0.75f, 0.20f, 0.18f);

        public Color standingColor = new Color(0.20f, 0.85f, 1.00f, 0.90f);
        public Color crouchColor = new Color(1.00f, 0.75f, 0.20f, 0.90f);

        public static PlayerSilhouetteSettings Instance => instance;

        public void Save() => Save(true);
    }
}
