using UnityEngine;
using UnityEditor;

namespace LOYAL.Editor
{
    public sealed class PlayerSilhouetteWindow : EditorWindow
    {
        private SerializedObject _serializedObject;
        private Vector2 _scrollPosition;

        private static class Contents
        {
            public static readonly string infoText = "Heights are read automatically from the target controller.";
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
            var settings = PlayerSilhouetteSettings.instance;
            if (settings == null) return;
            _serializedObject = new SerializedObject(settings);
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

            EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            SerializedProperty iterator = _serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (iterator.name == "m_Script") continue;

                if (iterator.name == "showOnlySelected" && !_serializedObject.FindProperty("showSilhouette").boolValue)
                    continue;

                if (iterator.name == "fillAlpha" && !_serializedObject.FindProperty("fillEnabled").boolValue)
                    continue;

                EditorGUILayout.PropertyField(iterator, true);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            var modules = PlayerSilhouetteDrawer.Modules;
            if (modules != null && modules.Count > 0)
            {
                EditorGUILayout.LabelField("Module Specific", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                for (int i = 0; i < modules.Count; i++)
                {
                    EditorGUILayout.LabelField(modules[i].ModuleName, EditorStyles.miniBoldLabel);
                    EditorGUI.indentLevel++;
                    modules[i].DrawSettingsGUI();
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button("Reset Global Defaults"))
            {
                PlayerSilhouetteSettings.instance.ResetToDefaults();
                _serializedObject.Update();
                PlayerSilhouetteSettings.instance.Save();
                SceneView.RepaintAll();
            }

            EditorGUILayout.EndScrollView();

            if (_serializedObject.ApplyModifiedProperties())
            {
                PlayerSilhouetteSettings.instance.Save();
                SceneView.RepaintAll();
            }
        }
    }
}
