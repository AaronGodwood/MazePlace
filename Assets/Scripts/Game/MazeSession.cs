using System;
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
        [SerializeField] private GenerationAnimator generationAnimator;
        [SerializeField] private OverviewCamera overview;
        [SerializeField] private PlayerController player;

        [SerializeField] private MazeSettings settings = new MazeSettings();

        public event Action MazeChanged;
        public event Action<SolveResult> Solved;
        public event Action GenerationFinished;

        /// <summary>The finished maze, even while its generation is still being replayed.</summary>
        public Maze Maze { get; private set; }
        public bool IsGenerating => generationAnimator.IsRunning;
        public GeneratorKind Generator { get; private set; }
        public MazeSettings Settings => settings;
        public bool FirstPerson { get; private set; }
        public int FocusedLevel { get; private set; }
        public Camera OverviewCamera => overview.Camera;
        public Camera PlayerCamera => player.Camera;

        /// <summary>Turns keyboard shortcuts and camera/player controls on or off, e.g. while a menu is open.</summary>
        public bool InputEnabled
        {
            get => enabled;
            set
            {
                enabled = value;
                overview.enabled = value;
                player.enabled = value;
            }
        }

        private void Awake()
        {
            settings.Load();
            generationAnimator.Finished += () => GenerationFinished?.Invoke();
        }

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
            if (!FirstPerson && keyboard.qKey.wasPressedThisFrame) FocusLevel(FocusedLevel - 1);
            if (!FirstPerson && keyboard.eKey.wasPressedThisFrame) FocusLevel(FocusedLevel + 1);
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            var random = new System.Random(settings.seed != 0 ? settings.seed : Environment.TickCount);
            var generation = Generators.GenerateWithSteps(settings.generator, settings.Size, random);
            Show(generation.Maze, settings.generator, Application.isPlaying && settings.animateGeneration ? generation : null);
        }

        public void Show(Maze maze, GeneratorKind generator) => Show(maze, generator, replay: null);

        private void Show(Maze maze, GeneratorKind generator, Generation replay)
        {
            Maze = maze;
            Generator = generator;
            solveAnimator.Clear();
            generationAnimator.Stop();
            if (replay != null) generationAnimator.Play(mazeRenderer, replay);
            else mazeRenderer.Render(maze);
            overview.Frame(MazeLayout.Bounds(maze.Size));
            FocusLevel(maze.Size.Levels - 1);
            player.Spawn(maze, maze.StartCell);
            MazeChanged?.Invoke();
        }

        public SolveResult Solve()
        {
            generationAnimator.Finish();
            var result = Solvers.Solve(settings.solver, Maze, Maze.StartCell, Maze.GoalCell);
            if (Application.isPlaying) solveAnimator.Play(mazeRenderer, result);
            else solveAnimator.ShowAll(mazeRenderer, result);
            Solved?.Invoke(result);
            return result;
        }

        public void SetFirstPerson(bool firstPerson)
        {
            FirstPerson = firstPerson;
            player.gameObject.SetActive(firstPerson);
            overview.gameObject.SetActive(!firstPerson);
            mazeRenderer.ShowLevelsUpTo(firstPerson ? Maze.Size.Levels - 1 : FocusedLevel);
        }

        /// <summary>In the overview, hides the levels above <paramref name="level"/> and points the camera at it.</summary>
        public void FocusLevel(int level)
        {
            FocusedLevel = Mathf.Clamp(level, 0, Maze.Size.Levels - 1);
            mazeRenderer.ShowLevelsUpTo(FirstPerson ? Maze.Size.Levels - 1 : FocusedLevel);
            overview.SetFocusHeight(FocusedLevel * MazeLayout.LevelSpacing);
        }
    }
}
