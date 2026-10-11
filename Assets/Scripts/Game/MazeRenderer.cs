using System.Collections.Generic;
using MazePlace.Core;
using UnityEngine;

namespace MazePlace.Game
{
    public sealed class MazeRenderer : MonoBehaviour
    {
        [SerializeField] private Material wallMaterial;
        [SerializeField] private Material floorMaterial;
        [SerializeField] private Material startMaterial;
        [SerializeField] private Material goalMaterial;

        private readonly List<GameObject> levels = new List<GameObject>();

        public Maze Maze { get; private set; }

        public void Render(Maze maze)
        {
            Clear();
            Maze = maze;

            for (var level = 0; level < maze.Size.Levels; level++)
                levels.Add(CreateLevel(maze, level));

            AddMarker("Start", maze.StartCell, startMaterial);
            AddMarker("Goal", maze.GoalCell, goalMaterial);
        }

        public Transform LevelRoot(int level) => levels[level].transform;

        /// <summary>Hides every level above <paramref name="topLevel"/> so lower ones can be seen from above.</summary>
        public void ShowLevelsUpTo(int topLevel)
        {
            for (var level = 0; level < levels.Count; level++)
                levels[level].SetActive(level <= topLevel);
        }

        private GameObject CreateLevel(Maze maze, int level)
        {
            var mesh = MazeLayout.BuildLevelMesh(maze, level);
            var go = new GameObject(mesh.name);
            go.hideFlags = GeneratedFlags;
            go.transform.SetParent(transform, worldPositionStays: false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterials = new[] { wallMaterial, floorMaterial };
            go.AddComponent<MeshCollider>().sharedMesh = mesh;
            return go;
        }

        private void AddMarker(string name, int cell, Material material)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = name;
            marker.hideFlags = GeneratedFlags;
            Dispose(marker.GetComponent<Collider>());
            marker.GetComponent<MeshRenderer>().sharedMaterial = material;

            var level = levels[Maze.Size.CellAt(cell).Level].transform;
            marker.transform.SetParent(level, worldPositionStays: false);
            marker.transform.localPosition = MazeLayout.CellCentre(Maze.Size, cell) + Vector3.up * 0.05f;
            marker.transform.localScale = new Vector3(MazeLayout.CellSize * 0.6f, 0.1f, MazeLayout.CellSize * 0.6f);
        }

        private void Clear()
        {
            foreach (var level in levels)
            {
                Dispose(level.GetComponent<MeshFilter>().sharedMesh);
                Dispose(level);
            }
            levels.Clear();
        }

        private void OnDestroy() => Clear();

        // Mazes generated in edit mode are previews; keep them out of the saved scene.
        private static HideFlags GeneratedFlags => Application.isPlaying ? HideFlags.None : HideFlags.DontSave;

        // Rendering also runs in edit mode (context menu, screenshot capture), where Destroy is not allowed.
        private static void Dispose(Object target)
        {
            if (Application.isPlaying) Destroy(target);
            else DestroyImmediate(target);
        }
    }
}
