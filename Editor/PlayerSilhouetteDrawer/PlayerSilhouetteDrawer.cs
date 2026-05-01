using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Daisen.Editor
{
    internal static class PlayerSilhouetteDrawer
    {
        private static bool s_HierarchyDirty = true;
        
        private static readonly List<Component> s_RawComponents = new List<Component>();
        private static readonly List<PlayerSilhouetteTarget> s_Targets = new List<PlayerSilhouetteTarget>();
        
        private static IPlayerSilhouetteReader[] s_Readers;
        
        private static readonly IPlayerSilhouetteModule[] s_Modules = new IPlayerSilhouetteModule[]
        {
            new StandingSilhouetteModule(),
            new CrouchingSilhouetteModule()
        };

        [InitializeOnLoadMethod]
        private static void RegisterCallbacks()
        {
            SceneView.duringSceneGui -= OnSceneGuiGlobal;
            SceneView.duringSceneGui += OnSceneGuiGlobal;
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            s_HierarchyDirty = true;
        }

        private static void EnsureReadersInitialized()
        {
            if (s_Readers != null) return;
            
            var readerList = new List<IPlayerSilhouetteReader>();
            var types = TypeCache.GetTypesDerivedFrom<IPlayerSilhouetteReader>();
            
            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface) continue;
                readerList.Add((IPlayerSilhouetteReader)System.Activator.CreateInstance(type));
            }
            
            s_Readers = readerList.ToArray();
        }

        private static void UpdateCacheIfNecessary()
        {
            if (!s_HierarchyDirty) return;
            s_HierarchyDirty = false;
            
            EnsureReadersInitialized();
            
            s_RawComponents.Clear();
            for (int i = 0; i < s_Readers.Length; i++)
            {
                s_Readers[i].FindTargets(s_RawComponents);
            }
            
            s_Targets.Clear();
            for (int i = 0; i < s_RawComponents.Count; i++)
            {
                var comp = s_RawComponents[i];
                if (!comp) continue;

                for (int r = 0; r < s_Readers.Length; r++)
                {
                    if (s_Readers[r].TryGetTargetData(comp, out var targetData))
                    {
                        s_Targets.Add(targetData);
                        break;
                    }
                }
            }
        }

        private static void OnSceneGuiGlobal(SceneView _)
        {
            if (Event.current.type != EventType.Repaint) return;
            
            var s = PlayerSilhouetteSettings.Instance;
            if (s == null || !s.showSilhouette) return;

            UpdateCacheIfNecessary();

            for (int m = 0; m < s_Modules.Length; m++)
            {
                s_Modules[m].UpdateCache(s_Targets, s);
            }

            var currentStage = StageUtility.GetCurrentStageHandle();
            var activeGo = Selection.activeGameObject;

            for (int i = 0; i < s_Targets.Count; i++)
            {
                var t = s_Targets[i];
                if (!t.Component || !t.GameObject) continue;

                var stage = StageUtility.GetStageHandle(t.GameObject);
                if (stage != currentStage) continue;

                if (s.showOnlySelected)
                {
                    if (activeGo != t.GameObject) continue;
                }
                else
                {
                    if (!t.GameObject.activeInHierarchy && activeGo != t.GameObject) continue;
                }

                for (int m = 0; m < s_Modules.Length; m++)
                {
                    s_Modules[m].Draw(t, s);
                }
            }
        }
    }
}