using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ScapeKitUnity
{

	class APiKeyCheckder : IPreprocessBuildWithReport
	{
	    public int callbackOrder { get { return 0; } }

	    public void OnPreprocessBuild(BuildReport report)
	    {
	    	var apiKey = ScapeClient.RetrieveKeyFromResources();
	        
	        if(apiKey == "")
	        {
	        	EditorUtility.DisplayDialog("Warning!", "A ScapeAPI key has not been set.", "Ok"); 
	        }
	    }
	}

}