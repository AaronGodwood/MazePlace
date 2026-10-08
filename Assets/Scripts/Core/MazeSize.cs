using System;
using System.Collections.Generic;

namespace MazePlace.Core
{
    public readonly struct Cell : IEquatable<Cell>
    {
        public int Column { get; }
        public int Row { get; }
        public int Level { get; }

        public Cell(int column, int row, int level)
        {
            Column = column;
            Row = row;
            Level = level;
        }

        public bool Equals(Cell other) => Column == other.Column && Row == other.Row && Level == other.Level;
        public override bool Equals(object obj) => obj is Cell other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Column, Row, Level);
        public override string ToString() => $"({Column}, {Row}, {Level})";
    }

    /// <summary>
    /// Dimensions of a maze grid. Cells are identified by a flat index so generators and
    /// solvers can use plain arrays; <see cref="CellAt"/> and <see cref="IndexOf"/> convert.
    /// </summary>
    public readonly struct MazeSize : IEquatable<MazeSize>
    {
        public int Columns { get; }
        public int Rows { get; }
        public int Levels { get; }

        public MazeSize(int columns, int rows, int levels = 1)
        {
            if (columns < 1) throw new ArgumentOutOfRangeException(nameof(columns), columns, "Must be at least 1");
            if (rows < 1) throw new ArgumentOutOfRangeException(nameof(rows), rows, "Must be at least 1");
            if (levels < 1) throw new ArgumentOutOfRangeException(nameof(levels), levels, "Must be at least 1");
            Columns = columns;
            Rows = rows;
            Levels = levels;
        }

        public int CellCount => Columns * Rows * Levels;

        public bool Contains(Cell cell) =>
            cell.Column >= 0 && cell.Column < Columns &&
            cell.Row >= 0 && cell.Row < Rows &&
            cell.Level >= 0 && cell.Level < Levels;

        public int IndexOf(Cell cell)
        {
            if (!Contains(cell)) throw new ArgumentOutOfRangeException(nameof(cell), cell, "Cell is outside the maze");
            return cell.Column + Columns * (cell.Row + Rows * cell.Level);
        }

        public Cell CellAt(int index)
        {
            if (index < 0 || index >= CellCount) throw new ArgumentOutOfRangeException(nameof(index), index, "Index is outside the maze");
            var layer = Columns * Rows;
            return new Cell(index % Columns, index % layer / Columns, index / layer);
        }

        public bool TryStep(int index, Direction direction, out int neighbour)
        {
            var cell = CellAt(index);
            var (dc, dr, dl) = direction.Offset();
            var next = new Cell(cell.Column + dc, cell.Row + dr, cell.Level + dl);
            neighbour = Contains(next) ? IndexOf(next) : -1;
            return neighbour >= 0;
        }

        public IEnumerable<(Direction Direction, int Cell)> Neighbours(int index)
        {
            foreach (var direction in Directions.All)
                if (TryStep(index, direction, out var neighbour))
                    yield return (direction, neighbour);
        }

        public bool Equals(MazeSize other) => Columns == other.Columns && Rows == other.Rows && Levels == other.Levels;
        public override bool Equals(object obj) => obj is MazeSize other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Columns, Rows, Levels);
        public override string ToString() => $"{Columns}x{Rows}x{Levels}";
    }
}
