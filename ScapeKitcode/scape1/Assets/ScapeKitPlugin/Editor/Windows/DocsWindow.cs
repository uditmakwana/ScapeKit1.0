using UnityEditor;
using UnityEngine;

namespace ScapeKitUnity
{
    internal class DocsWindow : EditorWindow
    {
        [MenuItem("ScapeKit/Docs %#d", false, 3)]
        static void Load()
        {
            Application.OpenURL("https://developer.scape.io/documentation");
        }
    }
}
