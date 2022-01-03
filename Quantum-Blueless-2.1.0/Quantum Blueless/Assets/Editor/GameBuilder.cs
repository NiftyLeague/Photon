using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

#if UNITY_2018
using UnityEditor.Build.Reporting;
#endif

/// <summary>
/// Main class to build standalone executables of the project
/// without use of graphical interface
/// </summary>
public static class GameBuilder
{
  /// <summary>
  /// The output folder where the binary will be saved
  /// </summary>
  static string OutputFolder;

  /// <summary>
  /// Executable name
  /// </summary>
  static string BuildName = "Game";

  /// <summary>
  /// Mapping between Build Targets and the extension 
  /// of the binary
  /// </summary>
  static Dictionary<BuildTarget, string> ExtensionMapping;

  /// <summary>
  /// Options for a Normal Build
  /// </summary>
  static BuildOptions buildNomal = BuildOptions.None;

  /// <summary>
  /// Options for a Development Build
  /// </summary>
  static BuildOptions buildDevelopment = BuildOptions.Development;

  /// <summary>
  /// Initialize the OutputFolder and the Extension mapping
  /// </summary>
  static GameBuilder()
  {
    OutputFolder = Application.dataPath.Replace("Assets", "");

    ExtensionMapping = new Dictionary<BuildTarget, string>()
    {
      {BuildTarget.StandaloneOSX, "app"},
      {BuildTarget.StandaloneWindows, "exe"}
    };
  }

  /// <summary>
  /// Build a standalone version of the game.
  /// </summary>
  /// <param name="buildName">Executable name.</param>
  /// <param name="target">Target Build.</param>
  /// <param name="options">Build Options.</param>
  static void Build(string buildName, BuildTarget target, BuildOptions options)
  {
    var suffix = options.Equals(buildDevelopment) ? "_dev" : "";
    var name = string.Format("{0}{1}.{2}", buildName, suffix, ExtensionMapping[target]);

    var folderName = string.Format("{0}{1}", target.ToString(), suffix);
    var outputPath = Path.Combine(Path.Combine(Path.Combine(OutputFolder, "build"), folderName), name);

    Debug.Log(string.Format("Building to {0}, Target {1}", outputPath, target));

    try
    {
      var errorMessage = "";
#if UNITY_2018
      BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, target, options);

      if (report.summary.result == BuildResult.Failed)
      {
        errorMessage = string.Format("Build failed with {0} errors", report.summary.totalErrors);
      }
#else
      errorMessage = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, target, options).ToString();
#endif
      
      if (string.IsNullOrEmpty(errorMessage))
      {
        Debug.Log("Build done!");
      }
      else
      {
        Debug.Log(string.Format("Build result: {0}", errorMessage));

        if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
        {
          EditorApplication.Exit(1);
        }
      }
    }
    catch (Exception e)
    {
      Debug.LogError(string.Format("Build error {0}", e.Message));
    }
  }

  /// <summary>
  /// Builds for OSX.
  /// </summary>
  [MenuItem("Builder/Build OSX")]
  public static void BuildOSX()
  {
    Build(BuildName, BuildTarget.StandaloneOSX, buildNomal);
  }

  /// <summary>
  /// Builds for OSX in Development Mode.
  /// </summary>
  [MenuItem("Builder/Build OSX [DEV]")]
  public static void BuildOSXDev()
  {
    Build(BuildName, BuildTarget.StandaloneOSX, buildDevelopment);
  }

  /// <summary>
  /// Builds for Windows.
  /// </summary>
  [MenuItem("Builder/Build Windows")]
  public static void BuildWindows()
  {
    Build(BuildName, BuildTarget.StandaloneWindows, buildNomal);
  }

  /// <summary>
  /// Builds for Windows in Development mode.
  /// </summary>
  [MenuItem("Builder/Build Windows [DEV]")]
  public static void BuildWindowsDev()
  {
    Build(BuildName, BuildTarget.StandaloneWindows, buildDevelopment);
  }
}