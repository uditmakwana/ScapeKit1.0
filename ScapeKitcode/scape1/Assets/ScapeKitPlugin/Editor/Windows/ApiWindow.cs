using UnityEditor;
using UnityEngine;

namespace ScapeKitUnity
{
    internal class ApiWindow : EditorWindow
    {
        [MenuItem("ScapeKit/API Reference %#a", false, 4)]
        static void Load()
        {
            Application.OpenURL("https://api.scape.io/unity/index.html");
        }
    }
}
