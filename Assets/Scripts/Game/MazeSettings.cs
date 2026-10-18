using System;
using System.IO;
using MazePlace.Core;
using UnityEngine;

namespace MazePlace.Game
{
    /// <summary>What the next maze should look like. Public fields so JsonUtility can save it.</summary>
    [Serializable]
    public sealed class MazeSettings
    {
        public const int MinWidth = 2;
        public const int MaxWidth = 40;
        public const int MaxLevels = 8;

        [Range(MinWidth, MaxWidth)] public int columns = 12;
        [Range(MinWidth, MaxWidth)] public int rows = 12;
        [Range(1, MaxLevels)] public int levels = 1;
        public GeneratorKind generator = GeneratorKind.Wilson;
        public SolverKind solver = SolverKind.GreedyBestFirst;
        public bool animateGeneration = true;
        [Tooltip("0 picks a random seed")]
        public int seed;

        public MazeSize Size => new MazeSize(columns, rows, levels);

        private static string FilePath => Path.Combine(Application.persistentDataPath, "settings.json");

        public void Save() => File.WriteAllText(FilePath, JsonUtility.ToJson(this, prettyPrint: true));

        /// <summary>Overwrites these settings from disk; a missing or unreadable file keeps the defaults.</summary>
        public void Load()
        {
            try
            {
                if (File.Exists(FilePath))
                    JsonUtility.FromJsonOverwrite(File.ReadAllText(FilePath), this);
            }
            catch (Exception e) when (e is IOException || e is ArgumentException)
            {
                Debug.LogWarning($"Ignoring unreadable settings file: {e.Message}");
            }

            columns = Mathf.Clamp(columns, MinWidth, MaxWidth);
            rows = Mathf.Clamp(rows, MinWidth, MaxWidth);
            levels = Mathf.Clamp(levels, 1, MaxLevels);
            if (!Enum.IsDefined(typeof(GeneratorKind), generator)) generator = GeneratorKind.Wilson;
            if (!Enum.IsDefined(typeof(SolverKind), solver)) solver = SolverKind.GreedyBestFirst;
        }
    }
}
