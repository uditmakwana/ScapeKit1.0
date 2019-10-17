using UnityEditor;
using UnityEngine;

namespace ScapeKitUnity
{

    public static class PluginBuilder
    {
        public static void PackageScapeKit()
        {
            string[] assetPaths = new string[]
            {
              "Assets/ScapeKitPlugin/Plugins", // native libs for all platforms here
              "Assets/Plugins/Android", // custom gradle file is here
              "Assets/ScapeKitPlugin/Scripts",
              "Assets/ScapeKitPlugin/Examples",
              "Assets/ScapeKitPlugin/Prefabs",
              "Assets/ScapeKitPlugin/Editor", 
              "Assets/UnityARKitPlugin/Plugins/iOS/UnityARKit/NativeInterface/ARSessionNative.mm",
              "Assets/UnityARKitPlugin/Plugins/iOS/UnityARKit/NativeInterface/ARVideoFormat.cs",
              "Assets/JsonDotNet/Assemblies",
              "Assets/GoogleARCore",
              "Assets/UnityARkitPlugin"
            };

            string packageName = GetArg("-packageName");
            UnityEditor.ExportPackageOptions options = UnityEditor.ExportPackageOptions.Recurse;
            AssetDatabase.ExportPackage(assetPaths, packageName, options);
        }

        public static string GetArg(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
            return null;
        }
    }
}
