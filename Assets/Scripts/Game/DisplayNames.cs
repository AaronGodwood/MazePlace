using MazePlace.Core;

namespace MazePlace.Game
{
    public static class DisplayNames
    {
        public static string Of(GeneratorKind kind) => kind switch
        {
            GeneratorKind.Kruskal => "Kruskal's",
            GeneratorKind.DepthFirst => "Recursive backtracker",
            GeneratorKind.Wilson => "Wilson's",
            _ => kind.ToString(),
        };

        public static string Of(SolverKind kind) => kind switch
        {
            SolverKind.DepthFirst => "Depth-first",
            SolverKind.GreedyBestFirst => "Greedy best-first",
            SolverKind.BreadthFirst => "Breadth-first",
            SolverKind.AStar => "A*",
            _ => kind.ToString(),
        };
    }
}
