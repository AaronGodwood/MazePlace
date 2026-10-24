using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace MazePlace.Editor
{
    /// <summary>
    /// Builds the game for the browser. Headless use:
    /// -executeMethod MazePlace.Editor.Build.WebGL [-buildOutput Builds/WebGL]
    /// </summary>
    public static class Build
    {
        [MenuItem("MazePlace/Build WebGL")]
        public static void WebGL()
        {
            // GitHub Pages can't send Content-Encoding headers, so the loader has to decompress itself.
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.emscriptenArgs = "";

            // App UI comes in with the editor-only AI Assistant package; the game never uses it,
            // and its WebGL plugin needs embind, which Unity's Emscripten setup does not link.
            // High stripping drops the App UI code that calls into the plugin (and shrinks the download).
            PlayerSettings.SetManagedStrippingLevel(UnityEditor.Build.NamedBuildTarget.WebGL, ManagedStrippingLevel.High);
            var appUiPlugin = PluginImporter.GetAllImporters().FirstOrDefault(p => p.assetPath.EndsWith("libAppUITextMateLib.a"));
            appUiPlugin?.SetIncludeInBuildDelegate(_ => false);

            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "-buildOutput");
            var output = index >= 0 && index + 1 < args.Length ? args[index + 1] : "Builds/WebGL";

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Maze.unity" },
                locationPathName = output,
                target = BuildTarget.WebGL,
                options = BuildOptions.None,
            });

            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception($"WebGL build {report.summary.result} with {report.summary.totalErrors} errors");
        }
    }
}
