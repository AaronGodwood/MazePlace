using MazePlace.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MazePlace.Game
{
    /// <summary>
    /// Owns the current maze and switches between the overview and first-person views.
    /// Keys: G generate, F solve, Tab change view, Q/E move between levels in the overview.
    /// </summary>
    public sealed class MazeSession : MonoBehaviour
    {
        [SerializeField] private MazeRenderer mazeRenderer;
        [SerializeField] private SolveAnimator solveAnimator;
        [SerializeField] private OverviewCamera overview;
        [SerializeField] private PlayerController player;

        [SerializeField] private MazeSettings settings = new MazeSettings();

        private int focusedLevel;

        public Maze Maze => mazeRenderer.Maze;
        public MazeSettings Settings => settings;
        public bool FirstPerson { get; private set; }
        public Camera OverviewCamera => overview.Camera;
        public Camera PlayerCamera => player.Camera;

        private void Start()
        {
            Generate();
            SetFirstPerson(false);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.gKey.wasPressedThisFrame) Generate();
            if (keyboard.fKey.wasPressedThisFrame) Solve();
            if (keyboard.tabKey.wasPressedThisFrame) SetFirstPerson(!FirstPerson);
            if (!FirstPerson && keyboard.qKey.wasPressedThisFrame) FocusLevel(focusedLevel - 1);
            if (!FirstPerson && keyboard.eKey.wasPressedThisFrame) FocusLevel(focusedLevel + 1);
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            var random = new System.Random(settings.seed != 0 ? settings.seed : System.Environment.TickCount);
            Show(Generators.Generate(settings.generator, settings.Size, random));
        }

        public void Show(Maze maze)
        {
            solveAnimator.Clear();
            mazeRenderer.Render(maze);
            overview.Frame(MazeLayout.Bounds(maze.Size));
            FocusLevel(maze.Size.Levels - 1);
            player.Spawn(maze, maze.StartCell);
        }

        public SolveResult Solve()
        {
            var result = Solvers.Solve(settings.solver, Maze, Maze.StartCell, Maze.GoalCell);
            if (Application.isPlaying) solveAnimator.Play(mazeRenderer, result);
            else solveAnimator.ShowAll(mazeRenderer, result);
            return result;
        }

        public void SetFirstPerson(bool firstPerson)
        {
            FirstPerson = firstPerson;
            player.gameObject.SetActive(firstPerson);
            overview.gameObject.SetActive(!firstPerson);
            mazeRenderer.ShowLevelsUpTo(firstPerson ? Maze.Size.Levels - 1 : focusedLevel);
        }

        /// <summary>In the overview, hides the levels above <paramref name="level"/> and points the camera at it.</summary>
        public void FocusLevel(int level)
        {
            focusedLevel = Mathf.Clamp(level, 0, Maze.Size.Levels - 1);
            mazeRenderer.ShowLevelsUpTo(FirstPerson ? Maze.Size.Levels - 1 : focusedLevel);
            overview.SetFocusHeight(focusedLevel * MazeLayout.LevelSpacing);
        }
    }
}
