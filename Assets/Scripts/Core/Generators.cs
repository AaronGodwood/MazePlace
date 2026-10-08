using System;
using System.Collections.Generic;
using System.Linq;

namespace MazePlace.Core
{
    public enum GeneratorKind
    {
        Kruskal,
        DepthFirst,
        Wilson,
    }

    /// <summary>
    /// Each generator produces a perfect maze: a spanning tree of the grid, so every pair of
    /// cells is joined by exactly one path.
    /// </summary>
    public static class Generators
    {
        public static Maze Generate(GeneratorKind kind, MazeSize size, Random random) => kind switch
        {
            GeneratorKind.Kruskal => Kruskal(size, random),
            GeneratorKind.DepthFirst => DepthFirst(size, random),
            GeneratorKind.Wilson => Wilson(size, random),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };

        /// <summary>Randomised Kruskal's: join random edges whose ends are not yet connected.</summary>
        public static Maze Kruskal(MazeSize size, Random random)
        {
            var builder = new Maze.Builder(size);
            var sets = new DisjointSet(size.CellCount);

            // Only "forward" directions, so each internal edge is listed once.
            var edges = Enumerable.Range(0, size.CellCount)
                .SelectMany(cell => new[] { Direction.East, Direction.South, Direction.Up }
                    .Where(direction => size.TryStep(cell, direction, out _))
                    .Select(direction => (Cell: cell, Direction: direction)))
                .ToArray();
            Shuffle(edges, random);

            foreach (var (cell, direction) in edges)
            {
                size.TryStep(cell, direction, out var neighbour);
                if (sets.Union(cell, neighbour))
                    builder.Carve(cell, direction);
            }

            return builder.Build();
        }

        /// <summary>Recursive backtracker: random walk to unvisited cells, backtracking at dead ends.</summary>
        public static Maze DepthFirst(MazeSize size, Random random)
        {
            var builder = new Maze.Builder(size);
            var visited = new bool[size.CellCount];
            var stack = new Stack<int>();

            var start = random.Next(size.CellCount);
            visited[start] = true;
            stack.Push(start);

            while (stack.Count > 0)
            {
                var cell = stack.Peek();
                var options = size.Neighbours(cell).Where(n => !visited[n.Cell]).ToList();
                if (options.Count == 0)
                {
                    stack.Pop();
                    continue;
                }

                var (direction, next) = options[random.Next(options.Count)];
                builder.Carve(cell, direction);
                visited[next] = true;
                stack.Push(next);
            }

            return builder.Build();
        }

        /// <summary>
        /// Wilson's: loop-erased random walks from each cell outside the tree until they hit it.
        /// Unlike the other two, this samples uniformly from all possible mazes.
        /// </summary>
        public static Maze Wilson(MazeSize size, Random random)
        {
            var builder = new Maze.Builder(size);
            var inTree = new bool[size.CellCount];
            inTree[random.Next(size.CellCount)] = true;

            // Overwriting the exit direction each time a cell is revisited erases the loop.
            var exitDirection = new Direction[size.CellCount];

            var order = Enumerable.Range(0, size.CellCount).ToArray();
            Shuffle(order, random);

            foreach (var start in order.Where(cell => !inTree[cell]))
            {
                var cell = start;
                while (!inTree[cell])
                {
                    var neighbours = size.Neighbours(cell).ToList();
                    var (direction, next) = neighbours[random.Next(neighbours.Count)];
                    exitDirection[cell] = direction;
                    cell = next;
                }

                cell = start;
                while (!inTree[cell])
                {
                    inTree[cell] = true;
                    cell = builder.Carve(cell, exitDirection[cell]);
                }
            }

            return builder.Build();
        }

        private static void Shuffle<T>(T[] items, Random random)
        {
            for (var i = items.Length - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (items[i], items[j]) = (items[j], items[i]);
            }
        }

        private sealed class DisjointSet
        {
            private readonly int[] parent;
            private readonly int[] rank;

            public DisjointSet(int count)
            {
                parent = Enumerable.Range(0, count).ToArray();
                rank = new int[count];
            }

            private int Find(int x)
            {
                while (parent[x] != x)
                {
                    parent[x] = parent[parent[x]];
                    x = parent[x];
                }
                return x;
            }

            /// <returns>False if the two were already in the same set.</returns>
            public bool Union(int a, int b)
            {
                var (rootA, rootB) = (Find(a), Find(b));
                if (rootA == rootB) return false;
                if (rank[rootA] < rank[rootB]) (rootA, rootB) = (rootB, rootA);
                parent[rootB] = rootA;
                if (rank[rootA] == rank[rootB]) rank[rootA]++;
                return true;
            }
        }
    }
}
