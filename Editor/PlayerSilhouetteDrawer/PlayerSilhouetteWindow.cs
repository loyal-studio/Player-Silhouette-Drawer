using UnityEngine;
using UnityEditor;

namespace Daisen.Editor
{
    public sealed class PlayerSilhouetteWindow : EditorWindow
    {
        private SerializedObject _serializedObject;

        private SerializedProperty _showSilhouette;
        private SerializedProperty _showOnlySelected;
        private SerializedProperty _renderCrouch;

        private SerializedProperty _shoulderWidth;
        private SerializedProperty _hipWidth;
        private SerializedProperty _waistWidth;
        private SerializedProperty _headRadius;
        private SerializedProperty _crouchRatio;

        private SerializedProperty _edgeCount;
        private SerializedProperty _wireThickness;

        private SerializedProperty _fillEnabled;
        private SerializedProperty _fillAlpha;
        private SerializedProperty _useSeparateFillColors;
        private SerializedProperty _fillStandingColor;
        private SerializedProperty _fillCrouchColor;

        private SerializedProperty _standingColor;
        private SerializedProperty _crouchColor;

        private Vector2 _scrollPosition;

        private static class Contents
        {
            public static readonly GUIContent visibilityHeader = new GUIContent("Visibility");
            public static readonly GUIContent proportionsHeader = new GUIContent("Proportions");
            public static readonly GUIContent renderingHeader = new GUIContent("Rendering");
            public static readonly GUIContent fillHeader = new GUIContent("Fill Options");
            public static readonly GUIContent colorsHeader = new GUIContent("Colors");
            public static readonly string infoText = "Heights are read automatically from the CharacterController.";
        }

        [MenuItem("Tools/Player Silhouette Settings")]
        public static void ShowWindow()
        {
            var win = GetWindow<PlayerSilhouetteWindow>("Player Silhouette");
            win.minSize = new Vector2(300f, 400f);
            win.Show();
        }

        private void OnEnable()
        {
            var settings = PlayerSilhouetteSettings.Instance;
            if (settings == null) return;

            _serializedObject = new SerializedObject(settings);

            _showSilhouette = _serializedObject.FindProperty("showSilhouette");
            _showOnlySelected = _serializedObject.FindProperty("showOnlySelected");
            _renderCrouch = _serializedObject.FindProperty("renderCrouch");

            _shoulderWidth = _serializedObject.FindProperty("shoulderWidth");
            _hipWidth = _serializedObject.FindProperty("hipWidth");
            _waistWidth = _serializedObject.FindProperty("waistWidth");
            _headRadius = _serializedObject.FindProperty("headRadius");
            _crouchRatio = _serializedObject.FindProperty("crouchRatio");

            _edgeCount = _serializedObject.FindProperty("edgeCount");
            _wireThickness = _serializedObject.FindProperty("wireThickness");

            _fillEnabled = _serializedObject.FindProperty("fillEnabled");
            _fillAlpha = _serializedObject.FindProperty("fillAlpha");
            _useSeparateFillColors = _serializedObject.FindProperty("useSeparateFillColors");
            _fillStandingColor = _serializedObject.FindProperty("fillStandingColor");
            _fillCrouchColor = _serializedObject.FindProperty("fillCrouchColor");

            _standingColor = _serializedObject.FindProperty("standingColor");
            _crouchColor = _serializedObject.FindProperty("crouchColor");
        }

        private void OnGUI()
        {
            if (_serializedObject == null)
            {
                EditorGUILayout.HelpBox("Settings not found.", MessageType.Error);
                return;
            }

            _serializedObject.Update();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(Contents.infoText, MessageType.Info);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Contents.visibilityHeader, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_showSilhouette);
            if (_showSilhouette.boolValue)
            {
                EditorGUILayout.PropertyField(_showOnlySelected);
                EditorGUILayout.PropertyField(_renderCrouch);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Contents.proportionsHeader, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_crouchRatio);
            EditorGUILayout.Slider(_shoulderWidth, 0.25f, 0.65f);
            EditorGUILayout.Slider(_hipWidth, 0.20f, 0.60f);
            EditorGUILayout.Slider(_waistWidth, 0.15f, 0.55f);
            EditorGUILayout.Slider(_headRadius, 0.07f, 0.18f);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Contents.renderingHeader, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_edgeCount);
            EditorGUILayout.Slider(_wireThickness, 0.5f, 5.0f);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Contents.colorsHeader, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_standingColor);
            EditorGUILayout.PropertyField(_crouchColor);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Contents.fillHeader, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_fillEnabled);
            if (_fillEnabled.boolValue)
            {
                EditorGUILayout.PropertyField(_useSeparateFillColors);
                if (_useSeparateFillColors.boolValue)
                {
                    EditorGUILayout.PropertyField(_fillStandingColor);
                    EditorGUILayout.PropertyField(_fillCrouchColor);
                }
                else
                {
                    EditorGUILayout.PropertyField(_fillAlpha);
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            if (GUILayout.Button("Reset to Defaults"))
            {
                ResetToDefaults();
                PlayerSilhouetteSettings.Instance.Save();
                SceneView.RepaintAll();
            }

            EditorGUILayout.EndScrollView();

            if (_serializedObject.ApplyModifiedProperties())
            {
                PlayerSilhouetteSettings.Instance.Save();
                SceneView.RepaintAll();
            }
        }

        private void ResetToDefaults()
        {
            _shoulderWidth.floatValue = 0.40f;
            _hipWidth.floatValue = 0.36f;
            _waistWidth.floatValue = 0.28f;
            _headRadius.floatValue = 0.11f;
            _crouchRatio.floatValue = 0.53f;

            _standingColor.colorValue = new Color(0.20f, 0.85f, 1.00f, 0.90f);
            _crouchColor.colorValue = new Color(1.00f, 0.75f, 0.20f, 0.90f);

            _fillAlpha.floatValue = 0.18f;
            _fillStandingColor.colorValue = new Color(0.20f, 0.85f, 1.00f, 0.18f);
            _fillCrouchColor.colorValue = new Color(1.00f, 0.75f, 0.20f, 0.18f);

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
