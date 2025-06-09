﻿using KoboldUi;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class AutoFillTools
    {
        private const string SEARCH_FOLDER = "Assets/";
        
#if KOBOLD_ALCHEMY_SUPPORT && UNITY_EDITOR
        [MenuItem("Tools/Kobold Ui/Autofill all windows and views", false)]
        private static void AutofillAllWindowsAndViews()
        {
            var assets = AssetDatabase.FindAssets("t:prefab", new[] {SEARCH_FOLDER});

            foreach (var asset in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(asset);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                var autoFillables = prefab.GetComponentsInChildren<IAutoFillable>();

                if (autoFillables.Length <= 0)
                    continue;
                
                foreach (var autoFillable in autoFillables) 
                    autoFillable.AutoFill();

                Debug.Log($"{nameof(AutoFillTools)} | Autofill called in {path}");
            }
        }
#endif
    }
}