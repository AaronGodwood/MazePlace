using System;
using System.Globalization;
using System.IO;
using MazePlace.Core;
using UnityEngine;

namespace MazePlace.Game
{
    public static class SaveSlots
    {
        public const int Count = 4;

        [Serializable]
        private sealed class SaveData
        {
            public int columns;
            public int rows;
            public int levels;
            public GeneratorKind generator;
            public string savedAt;
            public string passages;

            public MazeSize Size => new MazeSize(columns, rows, levels);
        }

        public static void Save(int slot, Maze maze, GeneratorKind generator)
        {
            var data = new SaveData
            {
                columns = maze.Size.Columns,
                rows = maze.Size.Rows,
                levels = maze.Size.Levels,
                generator = generator,
                savedAt = DateTime.Now.ToString("o", CultureInfo.InvariantCulture),
                passages = Convert.ToBase64String(MazeEncoding.ToBytes(maze)),
            };
            File.WriteAllText(PathFor(slot), JsonUtility.ToJson(data, prettyPrint: true));
        }

        public static bool TryLoad(int slot, out Maze maze, out GeneratorKind generator)
        {
            maze = null;
            generator = default;
            if (!TryRead(slot, out var data)) return false;

            try
            {
                maze = MazeEncoding.FromBytes(data.Size, Convert.FromBase64String(data.passages));
                generator = data.generator;
                return true;
            }
            catch (Exception e) when (e is ArgumentException || e is FormatException)
            {
                return false;
            }
        }

        /// <summary>One-line summary for the slot picker, e.g. "12 × 12 × 2 · Wilson's · 21 Oct 14:02".</summary>
        public static string Describe(int slot)
        {
            if (!File.Exists(PathFor(slot))) return "Empty";
            if (!TryRead(slot, out var data)) return "Unreadable save";

            var when = DateTime.TryParse(data.savedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var saved)
                ? saved.ToString("d MMM HH:mm", CultureInfo.CurrentCulture)
                : "unknown time";
            return $"{data.columns} × {data.rows} × {data.levels} · {DisplayNames.Of(data.generator)} · {when}";
        }

        private static bool TryRead(int slot, out SaveData data)
        {
            data = null;
            try
            {
                if (!File.Exists(PathFor(slot))) return false;
                data = JsonUtility.FromJson<SaveData>(File.ReadAllText(PathFor(slot)));
                return data != null && data.columns > 0 && data.rows > 0 && data.levels > 0 && data.passages != null;
            }
            catch (Exception e) when (e is IOException || e is ArgumentException)
            {
                return false;
            }
        }

        private static string PathFor(int slot)
        {
            if (slot < 1 || slot > Count) throw new ArgumentOutOfRangeException(nameof(slot), slot, $"Slots are 1 to {Count}");
            return Path.Combine(Application.persistentDataPath, $"slot{slot}.json");
        }
    }
}
