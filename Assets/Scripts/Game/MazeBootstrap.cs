using MazePlace.Core;
using UnityEngine;

namespace MazePlace.Game
{
    /// <summary>Generates a maze on play from Inspector settings. Replaced by the menu flow later.</summary>
    public sealed class MazeBootstrap : MonoBehaviour
    {
        [SerializeField] private MazeRenderer mazeRenderer;
        [SerializeField] private Camera viewCamera;
        [SerializeField, Min(1)] private int columns = 12;
        [SerializeField, Min(1)] private int rows = 12;
        [SerializeField, Min(1)] private int levels = 1;
        [SerializeField] private GeneratorKind generator = GeneratorKind.Wilson;
        [Tooltip("0 picks a random seed")]
        [SerializeField] private int seed;

        private void Start() => Generate();

        [ContextMenu("Generate")]
        public void Generate()
        {
            var size = new MazeSize(columns, rows, levels);
            var random = new System.Random(seed != 0 ? seed : System.Environment.TickCount);
            mazeRenderer.Render(Generators.Generate(generator, size, random));
            Frame(MazeLayout.Bounds(size));
        }

        private void Frame(Bounds bounds)
        {
            // Look down at 60° from the south, far enough back for the whole maze to fit vertically.
            var direction = Quaternion.Euler(60f, 0f, 0f) * Vector3.forward;
            var radius = bounds.extents.magnitude;
            var distance = radius / Mathf.Sin(viewCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            viewCamera.transform.position = bounds.center - direction * distance;
            viewCamera.transform.rotation = Quaternion.LookRotation(direction);
            viewCamera.farClipPlane = distance + radius * 2;
        }
    }
}
