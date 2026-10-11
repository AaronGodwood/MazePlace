using System;
using System.IO;
using MazePlace.Core;
using MazePlace.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MazePlace.Editor
{
    /// <summary>
    /// Renders the maze scene to a PNG without entering play mode. Headless use:
    /// -executeMethod MazePlace.Editor.Capture.Run -captureOutput file.png
    /// Optional: -captureSize 12x12x3 -captureSeed 42 -captureGenerator Wilson -captureFocus 1
    /// -captureSolve -captureFirstPerson
    /// </summary>
    public static class Capture
    {
        [MenuItem("MazePlace/Capture Screenshot")]
        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Maze.unity");
            var session = UnityEngine.Object.FindAnyObjectByType<MazeSession>();
            ApplyArguments(session.Settings);

            // Cameras need the target's aspect ratio before the overview frames the maze.
            var target = new RenderTexture(1600, 900, 24);
            session.OverviewCamera.targetTexture = target;
            session.PlayerCamera.targetTexture = target;

            session.Generate();
            if (int.TryParse(ArgumentAfter("-captureFocus"), out var focus)) session.FocusLevel(focus);
            if (HasFlag("-captureSolve")) session.Solve();
            session.SetFirstPerson(HasFlag("-captureFirstPerson"));

            var camera = session.FirstPerson ? session.PlayerCamera : session.OverviewCamera;
            camera.Render();
            Save(target, ArgumentAfter("-captureOutput") ?? "Screenshots/maze.png");

            session.OverviewCamera.targetTexture = null;
            session.PlayerCamera.targetTexture = null;
            UnityEngine.Object.DestroyImmediate(target);
        }

        private static void ApplyArguments(MazeSettings settings)
        {
            var size = ArgumentAfter("-captureSize")?.Split('x');
            if (size?.Length == 3)
            {
                settings.columns = int.Parse(size[0]);
                settings.rows = int.Parse(size[1]);
                settings.levels = int.Parse(size[2]);
            }
            if (int.TryParse(ArgumentAfter("-captureSeed"), out var seed)) settings.seed = seed;
            if (Enum.TryParse<GeneratorKind>(ArgumentAfter("-captureGenerator"), out var generator)) settings.generator = generator;
            if (Enum.TryParse<SolverKind>(ArgumentAfter("-captureSolver"), out var solver)) settings.solver = solver;
        }

        private static void Save(RenderTexture target, string output)
        {
            RenderTexture.active = target;
            var image = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
            RenderTexture.active = null;

            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
            File.WriteAllBytes(output, image.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(image);
        }

        private static bool HasFlag(string flag) => Array.IndexOf(Environment.GetCommandLineArgs(), flag) >= 0;

        private static string ArgumentAfter(string flag)
        {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, flag);
            return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
        }
    }
}
