using System;
using System.Collections.Generic;

namespace MazePlace.Core
{
    [Flags]
    public enum Direction : byte
    {
        None = 0,
        West = 1 << 0,
        East = 1 << 1,
        North = 1 << 2,
        South = 1 << 3,
        Down = 1 << 4,
        Up = 1 << 5,
    }

    public static class Directions
    {
        public static readonly IReadOnlyList<Direction> All = new[]
        {
            Direction.West, Direction.East, Direction.North, Direction.South, Direction.Down, Direction.Up,
        };

        public static Direction Opposite(this Direction direction) => direction switch
        {
            Direction.West => Direction.East,
            Direction.East => Direction.West,
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.Down => Direction.Up,
            Direction.Up => Direction.Down,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Expected a single direction"),
        };

        public static (int Column, int Row, int Level) Offset(this Direction direction) => direction switch
        {
            Direction.West => (-1, 0, 0),
            Direction.East => (1, 0, 0),
            Direction.North => (0, -1, 0),
            Direction.South => (0, 1, 0),
            Direction.Down => (0, 0, -1),
            Direction.Up => (0, 0, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Expected a single direction"),
        };
    }
}
