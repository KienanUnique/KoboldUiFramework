using KoboldUi.Element.View;
using KoboldUi.Windows;
using UnityEditor;
using UnityEngine;

namespace KoboldUiEditor
{
    /// <summary>
    /// Provides editor utilities for automatically wiring window and view references in prefabs.
    /// </summary>
    public static class AutoFillTools
    {
        private const string SEARCH_FOLDER = "Assets/";
        
#if (KOBOLD_ALCHEMY_SUPPORT || KOBOLD_ODIN_SUPPORT) && UNITY_EDITOR
        [MenuItem("Tools/Kobold Ui/Autofill all windows and views", false)]
        private static void AutofillAllWindowsAndViews()
        {
            var assets = AssetDatabase.FindAssets("t:prefab", new[] {SEARCH_FOLDER});

            foreach (var asset in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(asset);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                var windows = prefab.GetComponentsInChildren<AWindow>();
                var views = prefab.GetComponentsInChildren<AUiAnimatedView>();

                if (windows.Length == 0 && views.Length == 0)
                    continue;
                
                foreach (var window in windows)
                    window.AutoFill();

                foreach (var view in views)
                    view.AutoFill();

                Debug.Log($"{nameof(AutoFillTools)} | Autofill called in {path}");
            }
        }
#endif
    }
}
