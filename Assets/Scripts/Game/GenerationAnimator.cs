using System;
using System.Collections;
using System.Collections.Generic;
using MazePlace.Core;
using UnityEngine;

namespace MazePlace.Game
{
    /// <summary>Replays a generator: starts from a grid of closed cells and opens passages in the order they were carved.</summary>
    public sealed class GenerationAnimator : MonoBehaviour
    {
        [SerializeField] private Material cursorMaterial;
        [Tooltip("Roughly how long the whole replay takes, whatever the maze size")]
        [SerializeField, Min(0.5f)] private float targetDuration = 5f;
        [SerializeField, Min(1f)] private float minimumStepsPerSecond = 20f;

        private MazeRenderer mazeRenderer;
        private Generation generation;
        private GameObject cursor;
        private Coroutine running;

        public event Action Finished;

        public bool IsRunning => running != null;

        public void Play(MazeRenderer target, Generation replay)
        {
            Stop();
            mazeRenderer = target;
            generation = replay;
            mazeRenderer.Render(generation.After(0));
            running = StartCoroutine(Animate());
        }

        /// <summary>Skips to the finished maze.</summary>
        public void Finish()
        {
            if (!IsRunning) return;
            StopCoroutine(running);
            Complete();
        }

        /// <summary>Abandons the replay as it is, e.g. because another maze is about to be rendered.</summary>
        public void Stop()
        {
            if (running != null) StopCoroutine(running);
            running = null;
            RemoveCursor();
        }

        private IEnumerator Animate()
        {
            var steps = generation.Steps;
            var size = generation.Maze.Size;
            var stepsPerSecond = Mathf.Max(minimumStepsPerSecond, steps.Count / targetDuration);
            var shown = 0f;
            var next = 0;

            while (next < steps.Count)
            {
                shown += stepsPerSecond * Time.deltaTime;
                var touchedLevels = new HashSet<int>();
                var reached = -1;
                for (; next < steps.Count && next < shown; next++)
                {
                    var (cell, direction) = steps[next];
                    size.TryStep(cell, direction, out reached);
                    touchedLevels.Add(size.CellAt(cell).Level);
                    touchedLevels.Add(size.CellAt(reached).Level);
                }

                if (reached >= 0)
                {
                    var partial = generation.After(next);
                    // Colliders are only rebuilt at the end; cooking them every frame is too slow.
                    foreach (var level in touchedLevels)
                        mazeRenderer.Redraw(partial, level, updateCollider: false);
                    MoveCursor(size, reached);
                }
                yield return null;
            }

            Complete();
        }

        private void Complete()
        {
            running = null;
            RemoveCursor();
            for (var level = 0; level < generation.Maze.Size.Levels; level++)
                mazeRenderer.Redraw(generation.Maze, level, updateCollider: true);
            Finished?.Invoke();
        }

        private void MoveCursor(MazeSize size, int cell)
        {
            if (cursor == null)
            {
                cursor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cursor.name = "Generation Cursor";
                Destroy(cursor.GetComponent<Collider>());
                cursor.GetComponent<MeshRenderer>().sharedMaterial = cursorMaterial;
                var width = MazeLayout.CellSize * 0.4f;
                cursor.transform.localScale = new Vector3(width, 0.2f, width);
            }

            cursor.transform.SetParent(mazeRenderer.LevelRoot(size.CellAt(cell).Level), worldPositionStays: false);
            cursor.transform.localPosition = MazeLayout.CellCentre(size, cell) + Vector3.up * 0.1f;
        }

        private void RemoveCursor()
        {
            // The cursor dies with its level when the maze is re-rendered, so it may already be gone.
            if (cursor != null) Destroy(cursor);
            cursor = null;
        }
    }
}
