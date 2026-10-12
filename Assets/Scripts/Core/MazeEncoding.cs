using System.Collections.Generic;
using System.Linq;

namespace MazePlace.Core
{
    /// <summary>One byte of passage flags per cell, in cell index order.</summary>
    public static class MazeEncoding
    {
        public static byte[] ToBytes(Maze maze) => maze.Passages.Select(passages => (byte)passages).ToArray();

        /// <exception cref="System.ArgumentException">The data does not describe a valid maze of this size.</exception>
        public static Maze FromBytes(MazeSize size, IReadOnlyCollection<byte> bytes) =>
            Maze.FromPassages(size, bytes.Select(b => (Direction)b).ToArray());
    }
}
