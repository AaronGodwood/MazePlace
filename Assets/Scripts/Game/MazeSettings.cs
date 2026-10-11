using System;
using MazePlace.Core;
using UnityEngine;

namespace MazePlace.Game
{
    /// <summary>What the next maze should look like. Public fields so JsonUtility can save it.</summary>
    [Serializable]
    public sealed class MazeSettings
    {
        [Min(1)] public int columns = 12;
        [Min(1)] public int rows = 12;
        [Min(1)] public int levels = 1;
        public GeneratorKind generator = GeneratorKind.Wilson;
        public SolverKind solver = SolverKind.GreedyBestFirst;
        [Tooltip("0 picks a random seed")]
        public int seed;

        public MazeSize Size => new MazeSize(columns, rows, levels);
    }
}
