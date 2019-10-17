using UnityEditor;
using UnityEngine;

namespace ScapeKitUnity
{
	internal class BuildDeployment : EditorWindow
	{
		private static bool showPlatforms = true;
        private int selectedPlatform;
		private string apiKey;

        private static bool showApiKeySettings = true;

        [MenuItem("ScapeKit/Account", false, 1)]
        static void Init()
        {
            EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            EditorWindow window = null;
            if(allWindows.Length > 1) 
            {
                window = GetWindow<BuildDeployment>("ScapeKit", true, allWindows[1].GetType());
            } else 
            {
                window = GetWindow<BuildDeployment>("ScapeKit");
            }
            window.Show();
        }

        void Awake() {
            apiKey = ScapeClient.RetrieveKeyFromResources();
        }

        void OnGUI ()
        {
            ShowAccountSettings(); 
        }

        private void ShowLogo() 
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label(Utility.GetIcon("scape-logo.png"), GUILayout.Width(350), GUILayout.Height(140));
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        private void ShowAccountSettings() 
        {
            showApiKeySettings = EditorGUILayout.Foldout(showApiKeySettings, "Scape Account");
            if(showApiKeySettings)
            {
                ShowLogo();

                Rect DevelopmentSettings = EditorGUILayout.BeginHorizontal("box");
                {
                    Rect DevID = EditorGUILayout.BeginHorizontal("box");
                    {
                        GUILayout.Label("Enter your Scape API Key here:");
                        var newKey = EditorGUILayout.TextField(apiKey);
                        if (newKey != apiKey) 
                        {
                            apiKey = newKey;
                            ScapeClient.SaveApiKeyToResource(apiKey);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                }
                EditorGUILayout.EndHorizontal();
                
                if (GUILayout.Button("Acquire API Key!"))
                {
                    this.Close();
                    Application.OpenURL("https://developer.scape.io/dashboard/");
                }
            }
        }

        internal static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y+=padding/2;
            r.x-=2;
            r.width +=6;
            EditorGUI.DrawRect(r, color);
        }

        void OnLostFocus()
        {
            ScapeClient.SaveApiKeyToResource(apiKey);
        }

        void OnDestroy()
        {
            ScapeClient.SaveApiKeyToResource(apiKey);
        }

        public void OnInspectorUpdate()
        {
            this.Repaint();
        }
    }
}