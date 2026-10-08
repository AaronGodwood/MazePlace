using MazePlace.Core;

namespace MazePlace.Core.Tests;

public class GeneratorTests
{
    public static TheoryData<GeneratorKind, int, int, int> Cases()
    {
        var data = new TheoryData<GeneratorKind, int, int, int>();
        var sizes = new[] { (1, 1, 1), (1, 7, 1), (5, 5, 1), (6, 4, 3), (2, 2, 9), (15, 15, 4) };
        foreach (var kind in Enum.GetValues<GeneratorKind>())
            foreach (var (c, r, l) in sizes)
                data.Add(kind, c, r, l);
        return data;
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void Generates_a_perfect_maze(GeneratorKind kind, int columns, int rows, int levels)
    {
        var size = new MazeSize(columns, rows, levels);
        var maze = Generators.Generate(kind, size, new Random(42));

        Assert.Equal(size, maze.Size);
        Assert.Equal(size.CellCount - 1, PassageCount(maze));
        Assert.Equal(size.CellCount, ReachableFrom(maze, 0));
    }

    [Theory]
    [InlineData(GeneratorKind.Kruskal)]
    [InlineData(GeneratorKind.DepthFirst)]
    [InlineData(GeneratorKind.Wilson)]
    public void Passages_are_two_way_and_stay_inside_the_grid(GeneratorKind kind)
    {
        var maze = Generators.Generate(kind, new MazeSize(8, 6, 3), new Random(7));
        var exception = Record.Exception(() => Maze.FromPassages(maze.Size, maze.Passages));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(GeneratorKind.Kruskal)]
    [InlineData(GeneratorKind.DepthFirst)]
    [InlineData(GeneratorKind.Wilson)]
    public void Same_seed_gives_same_maze(GeneratorKind kind)
    {
        var size = new MazeSize(10, 10, 2);
        var a = Generators.Generate(kind, size, new Random(123));
        var b = Generators.Generate(kind, size, new Random(123));
        Assert.Equal(a.Passages, b.Passages);
    }

    [Theory]
    [InlineData(GeneratorKind.Kruskal)]
    [InlineData(GeneratorKind.DepthFirst)]
    [InlineData(GeneratorKind.Wilson)]
    public void Different_seeds_give_different_mazes(GeneratorKind kind)
    {
        var size = new MazeSize(10, 10, 2);
        var a = Generators.Generate(kind, size, new Random(1));
        var b = Generators.Generate(kind, size, new Random(2));
        Assert.NotEqual(a.Passages, b.Passages);
    }

    [Theory]
    [InlineData(GeneratorKind.Kruskal)]
    [InlineData(GeneratorKind.DepthFirst)]
    [InlineData(GeneratorKind.Wilson)]
    public void Multi_level_mazes_use_vertical_passages(GeneratorKind kind)
    {
        var maze = Generators.Generate(kind, new MazeSize(5, 5, 4), new Random(3));
        Assert.Contains(maze.Passages, p => p.HasFlag(Direction.Up));
    }

    private static int PassageCount(Maze maze) =>
        Enumerable.Range(0, maze.Size.CellCount).Sum(cell => maze.ConnectedNeighbours(cell).Count()) / 2;

    private static int ReachableFrom(Maze maze, int start)
    {
        var seen = new HashSet<int> { start };
        var queue = new Queue<int>(seen);
        while (queue.TryDequeue(out var cell))
            foreach (var next in maze.ConnectedNeighbours(cell).Where(seen.Add))
                queue.Enqueue(next);
        return seen.Count;
    }
}
