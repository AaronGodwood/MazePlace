using System;
using System.Collections.Generic;
using System.Linq;

namespace MazePlace.Core
{
    public enum SolverKind
    {
        DepthFirst,
        GreedyBestFirst,
    }

    public sealed class SolveResult
    {
        /// <summary>Cells in the order the solver expanded them, for replaying the search.</summary>
        public IReadOnlyList<int> Explored { get; }

        /// <summary>Start to goal inclusive, or empty if the goal is unreachable.</summary>
        public IReadOnlyList<int> Path { get; }

        public bool Found => Path.Count > 0;

        public SolveResult(IReadOnlyList<int> explored, IReadOnlyList<int> path)
        {
            Explored = explored;
            Path = path;
        }
    }

    public static class Solvers
    {
        public static SolveResult Solve(SolverKind kind, Maze maze, int start, int goal) => kind switch
        {
            SolverKind.DepthFirst => DepthFirst(maze, start, goal),
            SolverKind.GreedyBestFirst => GreedyBestFirst(maze, start, goal),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };

        public static SolveResult DepthFirst(Maze maze, int start, int goal)
        {
            var stack = new Stack<int>();
            stack.Push(start);
            return Search(maze, start, goal, stack.Push, stack.Pop, () => stack.Count > 0);
        }

        /// <summary>Always expands the frontier cell nearest the goal in a straight line.</summary>
        public static SolveResult GreedyBestFirst(Maze maze, int start, int goal)
        {
            var target = maze.Size.CellAt(goal);
            double DistanceToGoal(int cell)
            {
                var c = maze.Size.CellAt(cell);
                var (dc, dr, dl) = (c.Column - target.Column, c.Row - target.Row, c.Level - target.Level);
                return Math.Sqrt(dc * dc + dr * dr + dl * dl);
            }

            var frontier = new MinHeap();
            frontier.Push(start, DistanceToGoal(start));
            return Search(maze, start, goal, cell => frontier.Push(cell, DistanceToGoal(cell)), frontier.Pop, () => frontier.Count > 0);
        }

        /// <summary>Shared graph search; the frontier's ordering is what makes it DFS or greedy.</summary>
        private static SolveResult Search(Maze maze, int start, int goal, Action<int> push, Func<int> pop, Func<bool> any)
        {
            var parent = Enumerable.Repeat(-1, maze.Size.CellCount).ToArray();
            var discovered = new bool[maze.Size.CellCount];
            var explored = new List<int>();
            discovered[start] = true;

            while (any())
            {
                var cell = pop();
                explored.Add(cell);
                if (cell == goal)
                    return new SolveResult(explored, TracePath(parent, goal));

                foreach (var next in maze.ConnectedNeighbours(cell).Where(n => !discovered[n]))
                {
                    discovered[next] = true;
                    parent[next] = cell;
                    push(next);
                }
            }

            return new SolveResult(explored, Array.Empty<int>());
        }

        private static IReadOnlyList<int> TracePath(int[] parent, int goal)
        {
            var path = new List<int>();
            for (var cell = goal; cell != -1; cell = parent[cell])
                path.Add(cell);
            path.Reverse();
            return path;
        }

        /// <summary>
        /// Unity's .NET profile has no PriorityQueue. Ties go to the earliest pushed cell so
        /// results are deterministic.
        /// </summary>
        private sealed class MinHeap
        {
            private readonly List<(int Item, double Priority, long Order)> heap = new List<(int, double, long)>();
            private long pushed;

            public int Count => heap.Count;

            public void Push(int item, double priority)
            {
                heap.Add((item, priority, pushed++));
                for (var i = heap.Count - 1; i > 0 && Less(i, (i - 1) / 2); i = (i - 1) / 2)
                    Swap(i, (i - 1) / 2);
            }

            public int Pop()
            {
                var top = heap[0].Item;
                heap[0] = heap[heap.Count - 1];
                heap.RemoveAt(heap.Count - 1);

                var i = 0;
                while (true)
                {
                    var (left, right, smallest) = (2 * i + 1, 2 * i + 2, i);
                    if (left < heap.Count && Less(left, smallest)) smallest = left;
                    if (right < heap.Count && Less(right, smallest)) smallest = right;
                    if (smallest == i) return top;
                    Swap(i, smallest);
                    i = smallest;
                }
            }

            private bool Less(int a, int b) =>
                heap[a].Priority < heap[b].Priority ||
                (heap[a].Priority == heap[b].Priority && heap[a].Order < heap[b].Order);

            private void Swap(int a, int b) => (heap[a], heap[b]) = (heap[b], heap[a]);
        }
    }
}
