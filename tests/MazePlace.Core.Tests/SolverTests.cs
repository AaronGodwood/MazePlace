using MazePlace.Core;

namespace MazePlace.Core.Tests;

public class SolverTests
{
    public static TheoryData<SolverKind, GeneratorKind> Cases()
    {
        var data = new TheoryData<SolverKind, GeneratorKind>();
        foreach (var solver in Enum.GetValues<SolverKind>())
            foreach (var generator in Enum.GetValues<GeneratorKind>())
                data.Add(solver, generator);
        return data;
    }

    public static TheoryData<SolverKind> AllSolvers()
    {
        var data = new TheoryData<SolverKind>();
        foreach (var solver in Enum.GetValues<SolverKind>())
            data.Add(solver);
        return data;
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void Finds_a_valid_path_from_start_to_goal(SolverKind solver, GeneratorKind generator)
    {
        var maze = Generators.Generate(generator, new MazeSize(12, 9, 3), new Random(11));
        var result = Solvers.Solve(solver, maze, maze.StartCell, maze.GoalCell);

        Assert.True(result.Found);
        Assert.Equal(maze.StartCell, result.Path[0]);
        Assert.Equal(maze.GoalCell, result.Path[^1]);
        Assert.Equal(result.Path.Count, result.Path.Distinct().Count());
        foreach (var (from, to) in result.Path.Zip(result.Path.Skip(1)))
            Assert.Contains(to, maze.ConnectedNeighbours(from));
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void Explored_starts_at_start_ends_at_goal_and_covers_the_path(SolverKind solver, GeneratorKind generator)
    {
        var maze = Generators.Generate(generator, new MazeSize(10, 10, 1), new Random(5));
        var result = Solvers.Solve(solver, maze, maze.StartCell, maze.GoalCell);

        Assert.Equal(maze.StartCell, result.Explored[0]);
        Assert.Equal(maze.GoalCell, result.Explored[^1]);
        Assert.Equal(result.Explored.Count, result.Explored.Distinct().Count());
        Assert.Subset(result.Explored.ToHashSet(), result.Path.ToHashSet());
    }

    [Fact]
    public void Solvers_agree_because_a_perfect_maze_has_one_path()
    {
        var maze = Generators.Wilson(new MazeSize(15, 15, 2), new Random(9));
        var dfs = Solvers.DepthFirst(maze, maze.StartCell, maze.GoalCell);
        foreach (var solver in Enum.GetValues<SolverKind>())
            Assert.Equal(dfs.Path, Solvers.Solve(solver, maze, maze.StartCell, maze.GoalCell).Path);
    }

    [Theory]
    [InlineData(SolverKind.BreadthFirst)]
    [InlineData(SolverKind.AStar)]
    public void Finds_a_shortest_path_when_the_maze_has_loops(SolverKind solver)
    {
        var maze = OpenGrid(new MazeSize(7, 5, 3));
        var result = Solvers.Solve(solver, maze, maze.StartCell, maze.GoalCell);

        // With every passage open, the shortest route is the Manhattan distance between the corners.
        Assert.Equal(6 + 4 + 2 + 1, result.Path.Count);
    }

    [Fact]
    public void Depth_first_can_take_a_longer_path_when_the_maze_has_loops()
    {
        var maze = OpenGrid(new MazeSize(7, 5, 3));
        var result = Solvers.DepthFirst(maze, maze.StartCell, maze.GoalCell);
        Assert.True(result.Path.Count > 6 + 4 + 2 + 1);
    }

    [Fact]
    public void AStar_explores_fewer_cells_than_breadth_first()
    {
        var maze = OpenGrid(new MazeSize(10, 10, 2));
        var bfs = Solvers.BreadthFirst(maze, maze.StartCell, maze.GoalCell);
        var aStar = Solvers.AStar(maze, maze.StartCell, maze.GoalCell);

        Assert.Equal(bfs.Path.Count, aStar.Path.Count);
        Assert.True(aStar.Explored.Count < bfs.Explored.Count);
    }

    [Theory]
    [MemberData(nameof(AllSolvers))]
    public void Start_equal_to_goal_is_a_one_cell_path(SolverKind solver)
    {
        var maze = Generators.Kruskal(new MazeSize(3, 3), new Random(1));
        var result = Solvers.Solve(solver, maze, 4, 4);
        Assert.Equal(new[] { 4 }, result.Path);
    }

    [Theory]
    [MemberData(nameof(AllSolvers))]
    public void Unreachable_goal_gives_empty_path_after_exploring_everything_reachable(SolverKind solver)
    {
        // Two cells joined, one cell cut off: [0]-[1] [2]
        var size = new MazeSize(3, 1);
        var maze = Maze.FromPassages(size, new[] { Direction.East, Direction.West, Direction.None });

        var result = Solvers.Solve(solver, maze, 0, 2);

        Assert.False(result.Found);
        Assert.Empty(result.Path);
        Assert.Equal(new[] { 0, 1 }, result.Explored.OrderBy(c => c));
    }

    private static Maze OpenGrid(MazeSize size)
    {
        var passages = Enumerable.Range(0, size.CellCount)
            .Select(cell => size.Neighbours(cell).Aggregate(Direction.None, (open, n) => open | n.Direction))
            .ToArray();
        return Maze.FromPassages(size, passages);
    }
}
