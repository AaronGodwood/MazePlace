using System;
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
