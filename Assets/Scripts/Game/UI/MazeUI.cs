using System;
using System.Collections.Generic;
using System.Linq;
using MazePlace.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace MazePlace.Game.UI
{
    /// <summary>Switches between the menu screens and the in-game HUD, and wires their buttons up.</summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class MazeUI : MonoBehaviour
    {
        private enum Page
        {
            MainMenu,
            Options,
            Slots,
            Hud,
            Pause,
        }

        [SerializeField] private MazeSession session;

        private readonly Dictionary<Page, VisualElement> pages = new Dictionary<Page, VisualElement>();
        private VisualElement root;
        private Page current;
        private Page slotsReturnTo;
        private bool slotsSaving;
        private Label statusLabel;
        private Label hintLabel;

        private static readonly GeneratorKind[] GeneratorOptions = (GeneratorKind[])Enum.GetValues(typeof(GeneratorKind));
        private static readonly SolverKind[] SolverOptions = (SolverKind[])Enum.GetValues(typeof(SolverKind));

        private void Start()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            pages[Page.MainMenu] = root.Q("mainMenu");
            pages[Page.Options] = root.Q("options");
            pages[Page.Slots] = root.Q("slots");
            pages[Page.Hud] = root.Q("hud");
            pages[Page.Pause] = root.Q("pause");
            statusLabel = root.Q<Label>("statusLabel");
            hintLabel = root.Q<Label>("hintLabel");

            WireMainMenu();
            WireOptions();
            WireSlots();
            WireHud();
            WirePause();

            session.MazeChanged += () => statusLabel.text = session.IsGenerating ? $"Carving with {DisplayNames.Of(session.Generator)}…" : "";
            session.GenerationFinished += () => statusLabel.text = "";
            session.Solved += result => statusLabel.text = result.Found
                ? $"{DisplayNames.Of(session.Settings.solver)}: explored {result.Explored.Count} of {session.Maze.Size.CellCount} cells, path {result.Path.Count} long"
                : "No path to the goal";

            Show(Page.MainMenu);
        }

        private void Update()
        {
            var escape = Keyboard.current?.escapeKey.wasPressedThisFrame ?? false;
            if (escape && current == Page.Hud) Show(Page.Pause);
            else if (escape && current == Page.Pause) Show(Page.Hud);

            if (current == Page.Hud) hintLabel.text = Hint();
        }

        private void Show(Page page)
        {
            current = page;
            foreach (var (key, element) in pages)
                element.EnableInClassList("hidden", key != page);

            session.InputEnabled = page == Page.Hud;
            Time.timeScale = page == Page.Pause ? 0f : 1f;
        }

        private string Hint()
        {
            if (session.FirstPerson)
                return "WASD move · Mouse look · Space climb up a shaft · Tab overview";
            var levels = session.Maze.Size.Levels;
            var levelHint = levels > 1 ? $" · Q/E change level ({session.FocusedLevel + 1} of {levels})" : "";
            return $"Right-drag orbit · Scroll zoom{levelHint} · Tab first person";
        }

        private void WireMainMenu()
        {
            OnClick("newMazeButton", () =>
            {
                session.Generate();
                session.SetFirstPerson(false);
                Show(Page.Hud);
            });
            OnClick("loadButton", () => ShowSlots(saving: false, returnTo: Page.MainMenu));
            OnClick("optionsButton", () => Show(Page.Options));
            OnClick("quitButton", () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
#if UNITY_WEBGL && !UNITY_EDITOR
            // A browser tab can't quit itself.
            root.Q("quitButton").style.display = DisplayStyle.None;
#endif
        }

        private void WireOptions()
        {
            var settings = session.Settings;

            BindSlider("columnsSlider", MazeSettings.MinWidth, MazeSettings.MaxWidth, settings.columns, v => settings.columns = v);
            BindSlider("rowsSlider", MazeSettings.MinWidth, MazeSettings.MaxWidth, settings.rows, v => settings.rows = v);
            BindSlider("levelsSlider", 1, MazeSettings.MaxLevels, settings.levels, v => settings.levels = v);

            BindDropdown("generatorDropdown", GeneratorOptions, DisplayNames.Of, settings.generator, v => settings.generator = v);
            BindDropdown("solverDropdown", SolverOptions, DisplayNames.Of, settings.solver, v => settings.solver = v);

            var animate = root.Q<Toggle>("animateToggle");
            animate.SetValueWithoutNotify(settings.animateGeneration);
            animate.RegisterValueChangedCallback(e => settings.animateGeneration = e.newValue);

            var seed = root.Q<IntegerField>("seedField");
            seed.SetValueWithoutNotify(settings.seed);
            seed.RegisterValueChangedCallback(e => settings.seed = e.newValue);

            OnClick("optionsBackButton", () =>
            {
                settings.Save();
                Show(Page.MainMenu);
            });
        }

        private void WireSlots()
        {
            for (var slot = 1; slot <= SaveSlots.Count; slot++)
            {
                var chosen = slot;
                OnClick($"slot{slot}Button", () => ChooseSlot(chosen));
            }
            OnClick("slotsBackButton", () => Show(slotsReturnTo));
        }

        private void WireHud()
        {
            // HUD buttons must not take keyboard focus, or Space (climb) and Tab would trigger them.
            foreach (var button in pages[Page.Hud].Query<Button>().ToList())
                button.focusable = false;

            OnClick("solveButton", () => session.Solve());
            OnClick("viewButton", () => session.SetFirstPerson(!session.FirstPerson));
            OnClick("regenerateButton", session.Generate);
            OnClick("pauseButton", () => Show(Page.Pause));
        }

        private void WirePause()
        {
            OnClick("resumeButton", () => Show(Page.Hud));
            OnClick("saveButton", () => ShowSlots(saving: true, returnTo: Page.Pause));
            OnClick("mainMenuButton", () =>
            {
                session.SetFirstPerson(false);
                Show(Page.MainMenu);
            });
        }

        private void ShowSlots(bool saving, Page returnTo)
        {
            slotsSaving = saving;
            slotsReturnTo = returnTo;
            root.Q<Label>("slotsHeading").text = saving ? "Save maze" : "Load maze";
            RefreshSlots();
            Show(Page.Slots);
        }

        private void RefreshSlots()
        {
            for (var slot = 1; slot <= SaveSlots.Count; slot++)
            {
                var description = SaveSlots.Describe(slot);
                var button = root.Q<Button>($"slot{slot}Button");
                button.text = $"Slot {slot}    {description}";
                button.EnableInClassList("slot-button--empty", description == "Empty");
            }
        }

        private void ChooseSlot(int slot)
        {
            if (slotsSaving)
            {
                SaveSlots.Save(slot, session.Maze, session.Generator);
                RefreshSlots();
                return;
            }

            if (!SaveSlots.TryLoad(slot, out var maze, out var generator))
            {
                if (SaveSlots.Describe(slot) != "Empty")
                    root.Q<Button>($"slot{slot}Button").text = $"Slot {slot}    Couldn't load this save";
                return;
            }
            session.Show(maze, generator);
            session.SetFirstPerson(false);
            Show(Page.Hud);
        }

        private void OnClick(string name, Action action) => root.Q<Button>(name).clicked += action;

        private void BindSlider(string name, int low, int high, int value, Action<int> set)
        {
            var slider = root.Q<SliderInt>(name);
            slider.lowValue = low;
            slider.highValue = high;
            slider.SetValueWithoutNotify(value);
            slider.RegisterValueChangedCallback(e =>
            {
                // The input field accepts any number, so clamp what is typed in.
                var clamped = Mathf.Clamp(e.newValue, low, high);
                if (clamped != e.newValue) slider.SetValueWithoutNotify(clamped);
                set(clamped);
            });
        }

        private void BindDropdown<T>(string name, T[] options, Func<T, string> label, T value, Action<T> set)
        {
            var dropdown = root.Q<DropdownField>(name);
            dropdown.choices = options.Select(label).ToList();
            dropdown.index = Array.IndexOf(options, value);
            dropdown.RegisterValueChangedCallback(_ => set(options[dropdown.index]));
        }
    }
}
