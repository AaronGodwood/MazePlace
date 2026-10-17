using System;
using System.Collections.Generic;
using System.Linq;

namespace MazePlace.Core
{
    /// <summary>
    /// An immutable maze: for each cell, the set of directions with an open passage.
    /// Passages are always stored on both sides, so <c>IsOpen(a, d)</c> implies the reverse.
    /// </summary>
    public sealed class Maze
    {
        private readonly Direction[] passages;

        public MazeSize Size { get; }
        public IReadOnlyList<Direction> Passages => passages;

        private Maze(MazeSize size, Direction[] passages)
        {
            Size = size;
            this.passages = passages;
        }

        /// <summary>Validates untrusted passage data, e.g. from a save file.</summary>
        public static Maze FromPassages(MazeSize size, IReadOnlyList<Direction> passages)
        {
            if (passages.Count != size.CellCount)
                throw new ArgumentException($"Expected {size.CellCount} cells, got {passages.Count}", nameof(passages));

            for (var cell = 0; cell < passages.Count; cell++)
            {
                foreach (var direction in Directions.All.Where(d => passages[cell].HasFlag(d)))
                {
                    if (!size.TryStep(cell, direction, out var neighbour))
                        throw new ArgumentException($"Cell {size.CellAt(cell)} has a passage {direction} out of the maze", nameof(passages));
                    if (!passages[neighbour].HasFlag(direction.Opposite()))
                        throw new ArgumentException($"Passage {direction} from {size.CellAt(cell)} is one-way", nameof(passages));
                }
            }

            return new Maze(size, passages.ToArray());
        }

        public bool IsOpen(int cell, Direction direction) => passages[cell].HasFlag(direction);

        public IEnumerable<int> ConnectedNeighbours(int cell)
        {
            foreach (var (direction, neighbour) in Size.Neighbours(cell))
                if (IsOpen(cell, direction))
                    yield return neighbour;
        }

        public int StartCell => 0;
        public int GoalCell => Size.CellCount - 1;

        /// <summary>Mutable passage grid used while a generator is carving.</summary>
        internal sealed class Builder
        {
            private readonly Direction[] passages;
            private readonly List<(int Cell, Direction Direction)> steps = new List<(int Cell, Direction Direction)>();

            public MazeSize Size { get; }
            public IReadOnlyList<(int Cell, Direction Direction)> Steps => steps;

            public Builder(MazeSize size)
            {
                Size = size;
                passages = new Direction[size.CellCount];
            }

            public int Carve(int cell, Direction direction)
            {
                if (!Size.TryStep(cell, direction, out var neighbour))
                    throw new InvalidOperationException($"Cannot carve {direction} out of the maze from {Size.CellAt(cell)}");
                passages[cell] |= direction;
                passages[neighbour] |= direction.Opposite();
                steps.Add((cell, direction));
                return neighbour;
            }

            public Maze Build() => new Maze(Size, passages);
        }
    }
}
