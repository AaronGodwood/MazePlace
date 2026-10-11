using System.Collections;
using System.Collections.Generic;
using MazePlace.Core;
using UnityEngine;

namespace MazePlace.Game
{
    /// <summary>Replays a solver's search: every explored cell in order, then the final path on top.</summary>
    public sealed class SolveAnimator : MonoBehaviour
    {
        [SerializeField] private Material exploredMaterial;
        [SerializeField] private Material pathMaterial;
        [Tooltip("Roughly how long the whole replay takes, whatever the maze size")]
        [SerializeField, Min(0.5f)] private float targetDuration = 6f;
        [SerializeField, Min(1f)] private float minimumCellsPerSecond = 20f;

        private readonly List<GameObject> markers = new List<GameObject>();
        private Coroutine running;

        public bool IsRunning => running != null;

        public void Play(MazeRenderer mazeRenderer, SolveResult result)
        {
            Clear();
            running = StartCoroutine(Animate(mazeRenderer, result));
        }

        /// <summary>Places every marker immediately, e.g. for screenshots.</summary>
        public void ShowAll(MazeRenderer mazeRenderer, SolveResult result)
        {
            Clear();
            foreach (var cell in result.Explored)
                AddMarker(mazeRenderer, cell, exploredMaterial, 0.5f, 0.04f);
            foreach (var cell in result.Path)
                AddMarker(mazeRenderer, cell, pathMaterial, 0.3f, 0.12f);
        }

        public void Clear()
        {
            if (running != null)
            {
                StopCoroutine(running);
                running = null;
            }

            foreach (var marker in markers)
            {
                // Markers die with their level when the maze is re-rendered, so some may already be gone.
                if (marker == null) continue;
                if (Application.isPlaying) Destroy(marker);
                else DestroyImmediate(marker);
            }
            markers.Clear();
        }

        private IEnumerator Animate(MazeRenderer mazeRenderer, SolveResult result)
        {
            var cellsPerSecond = Mathf.Max(minimumCellsPerSecond, (result.Explored.Count + result.Path.Count) / targetDuration);

            yield return Reveal(result.Explored, cell => AddMarker(mazeRenderer, cell, exploredMaterial, 0.5f, 0.04f), cellsPerSecond);
            yield return Reveal(result.Path, cell => AddMarker(mazeRenderer, cell, pathMaterial, 0.3f, 0.12f), cellsPerSecond);

            running = null;
        }

        private static IEnumerator Reveal(IReadOnlyList<int> cells, System.Action<int> show, float cellsPerSecond)
        {
            var shown = 0f;
            var next = 0;
            while (next < cells.Count)
            {
                shown += cellsPerSecond * Time.deltaTime;
                for (; next < cells.Count && next < shown; next++)
                    show(cells[next]);
                yield return null;
            }
        }

        private void AddMarker(MazeRenderer mazeRenderer, int cell, Material material, float widthFraction, float height)
        {
            var size = mazeRenderer.Maze.Size;
            var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = $"{material.name} {size.CellAt(cell)}";
            if (!Application.isPlaying) marker.hideFlags = HideFlags.DontSave;
            DestroyImmediate(marker.GetComponent<Collider>());
            marker.GetComponent<MeshRenderer>().sharedMaterial = material;

            var width = MazeLayout.CellSize * widthFraction;
            marker.transform.SetParent(mazeRenderer.LevelRoot(size.CellAt(cell).Level), worldPositionStays: false);
            marker.transform.localPosition = MazeLayout.CellCentre(size, cell) + Vector3.up * height / 2;
            marker.transform.localScale = new Vector3(width, height, width);
            markers.Add(marker);
        }
    }
}
