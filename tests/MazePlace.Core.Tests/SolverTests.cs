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
        var greedy = Solvers.GreedyBestFirst(maze, maze.StartCell, maze.GoalCell);
        Assert.Equal(dfs.Path, greedy.Path);
    }

    [Theory]
    [InlineData(SolverKind.DepthFirst)]
    [InlineData(SolverKind.GreedyBestFirst)]
    public void Start_equal_to_goal_is_a_one_cell_path(SolverKind solver)
    {
        var maze = Generators.Kruskal(new MazeSize(3, 3), new Random(1));
        var result = Solvers.Solve(solver, maze, 4, 4);
        Assert.Equal(new[] { 4 }, result.Path);
    }

    [Theory]
    [InlineData(SolverKind.DepthFirst)]
    [InlineData(SolverKind.GreedyBestFirst)]
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
}
