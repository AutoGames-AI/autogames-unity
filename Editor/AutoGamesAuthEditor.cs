#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// Editor utility to create a GameObject named "AuthManager" with the AutoGamesAuth component.
// Place this file in Assets/Editor (Unity compiles it only in the Editor).

namespace QAI.LogIn.Editor
{
    public static class AutoGamesAuthEditor
    {
        private const string MenuPath = "Tools/AutoGames/Create AuthManager";

        [MenuItem(MenuPath)]
        public static void CreateAuthManager()
        {
            // If there's already a GameObject with that name - select it and notify the user.
            var existing = GameObject.Find("AuthManager");
            if (existing != null)
            {
                Selection.activeGameObject = existing;
                EditorUtility.DisplayDialog("Create AuthManager", "AuthManager already exists and was selected in the Hierarchy.", "OK");
                EditorGUIUtility.PingObject(existing);
                return;
            }

            // Create new GameObject and attach the runtime component.
            var go = new GameObject("AuthManager");

            // Try to add the component. If the runtime script isn't compiled yet, this will still compile in the editor
            // because this file is wrapped in UNITY_EDITOR. If the type cannot be found, warn the user.
            var comp = go.AddComponent(typeof(AutoGamesAuth));
            if (comp == null)
            {
                Debug.LogWarning("AutoGamesAuth component type not found. Make sure the runtime script is compiled and in namespace QAI.LogIn.");
            }

            // Register undo so the creation can be reverted with Ctrl+Z.
            Undo.RegisterCreatedObjectUndo(go, "Create AuthManager");

            Selection.activeGameObject = go;
            EditorGUIUtility.PingObject(go);
        }

        // Validation method: disable the menu while in Play Mode to avoid accidental creation during runtime.
        [MenuItem(MenuPath, true)]
        public static bool ValidateCreateAuthManager()
        {
            return !Application.isPlaying;
        }
    }
}
#endif
