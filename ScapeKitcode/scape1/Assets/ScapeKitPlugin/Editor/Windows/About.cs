using UnityEngine;
using UnityEditor;
using System.IO;

namespace ScapeKitUnity
{
    public class ScapeKitVersionInfo : EditorWindow
    {
        private static string versionString = "";

        [MenuItem("ScapeKit/About ScapeKit")]
        static void VersionOutput()
        {
            versionString = ScapeKitVersionVars.version + "." + ScapeKitVersionVars.build;

            ScapeKitVersionInfo window = (ScapeKitVersionInfo)EditorWindow.GetWindowWithRect(typeof(ScapeKitVersionInfo), 
                                                                                             new Rect(0, 0, 300, 200));
            window.titleContent = new GUIContent { text = "About ScapeKit" };
            window.Show();
        }

        [MenuItem("ScapeKit/Help")]
        static void HelpOutput()
        {
            Application.OpenURL("https://community.scape.io/");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("https://developer.scape.io\nScapeKit Version " + versionString + "\n" +
                "", EditorStyles.wordWrappedLabel);

            GUILayout.Space(5);
            if (GUILayout.Button("Developer Portal"))
            {
                this.Close();
                Application.OpenURL("https://developer.scape.io");
            }

            GUILayout.Space(5);
            if (GUILayout.Button("Done"))
            {
                this.Close();
            }
        }
    }
}