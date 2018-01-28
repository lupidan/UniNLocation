using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class NativeLocationPostprocessor
{

	[PostProcessBuild]
	public static void ModifyXcodeProject(BuildTarget buildTarget, string pathToBuiltProject)
	{
		if (buildTarget == BuildTarget.iOS)
		{
			var xcodeProjectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
			var project = new PBXProject();
			project.ReadFromFile(xcodeProjectPath);
			
			string target = project.TargetGuidByName("Unity-iPhone");
			project.AddCapability(target, PBXCapabilityType.BackgroundModes);
			project.WriteToFile(xcodeProjectPath);
	
			var infoPlistPath = pathToBuiltProject + "/Info.plist";
			var infoPlist = new PlistDocument();
			infoPlist.ReadFromString(File.ReadAllText(infoPlistPath));
			PlistElementDict rootDictionary = infoPlist.root;
			rootDictionary.SetString("NSLocationWhenInUseUsageDescription", "For stuff leñe!");
			var backgroundModes = rootDictionary.CreateArray("UIBackgroundModes");
			backgroundModes.AddString("location");
			File.WriteAllText(infoPlistPath, infoPlist.WriteToString());		
		}
	}

}
