using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace ScapeKitUnity
{
    public class PostBuildTrigger : MonoBehaviour
    {
        [PostProcessBuild(500)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.iOS)
            {
#if UNITY_IOS
            Debug.Log ("Post Processing iOS Build...");

            var projPath = PBXProject.GetPBXProjectPath (pathToBuiltProject);
            var proj = new PBXProject ();
            proj.ReadFromString (File.ReadAllText (projPath));
            var target = proj.TargetGuidByName ("Unity-iPhone");

            const string defaultLocationInProj = "Plugins/iOS";
            const string scapekitFrameworkName = "ScapeKit.framework";

            string frameworkPath = Path.Combine(defaultLocationInProj, scapekitFrameworkName);
            string swiftHeaderPath = Path.Combine(frameworkPath, "/Headers/ScapeKit-Swift.h");

            proj.SetBuildProperty(target, "IPHONEOS_DEPLOYMENT_TARGET", "11.4");
            proj.AddBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            proj.AddBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");
            proj.AddBuildProperty(target, "ENABLE_BITCODE", "NO");
            proj.AddBuildProperty(target, "VALID_ARCHS", "arm64 arm64e");

            proj.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", "/Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/lib/swift/iphoneos");

            proj.AddBuildProperty(target, "LD_RUNPATH_SEARCH_PATHS", "/usr/lib/swift $(inherited) @executable_path/Frameworks @loader_path/Frameworks @rpath/");
            proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-lz -lc++ -weak_framework CoreMotion -weak-lSystem -ObjC");

            proj.WriteToFile(projPath); 
#endif

            }
            else if (buildTarget == BuildTarget.Android)
            {
#if UNITY_ANDROID
                //Debug.Log("Post Processing Android Build...");
#endif
            }
        }
    }
}