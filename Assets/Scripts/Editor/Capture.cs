using System;
using System.IO;
using MazePlace.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MazePlace.Editor
{
    /// <summary>
    /// Renders the maze scene to a PNG without entering play mode. Headless use:
    /// -executeMethod MazePlace.Editor.Capture.Run -captureOutput path/to/file.png
    /// </summary>
    public static class Capture
    {
        [MenuItem("MazePlace/Capture Screenshot")]
        public static void Run()
        {
            var output = ArgumentAfter("-captureOutput") ?? "Screenshots/maze.png";
            EditorSceneManager.OpenScene("Assets/Scenes/Maze.unity");

            UnityEngine.Object.FindAnyObjectByType<MazeBootstrap>().Generate();

            var camera = Camera.main;
            var target = new RenderTexture(1600, 900, 24);
            camera.targetTexture = target;
            camera.Render();

            RenderTexture.active = target;
            var image = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
            File.WriteAllBytes(output, image.EncodeToPNG());

            camera.targetTexture = null;
            RenderTexture.active = null;
            UnityEngine.Object.DestroyImmediate(target);
            UnityEngine.Object.DestroyImmediate(image);
        }

        private static string ArgumentAfter(string flag)
        {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, flag);
            return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
        }
    }
}
