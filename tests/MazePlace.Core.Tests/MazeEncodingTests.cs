using MazePlace.Core;

namespace MazePlace.Core.Tests;

public class MazeEncodingTests
{
    [Fact]
    public void Round_trips_a_generated_maze()
    {
        var maze = Generators.Wilson(new MazeSize(7, 5, 3), new Random(8));
        var decoded = MazeEncoding.FromBytes(maze.Size, MazeEncoding.ToBytes(maze));
        Assert.Equal(maze.Passages, decoded.Passages);
    }

    [Fact]
    public void Uses_one_byte_per_cell()
    {
        var maze = Generators.Kruskal(new MazeSize(4, 4, 2), new Random(1));
        Assert.Equal(32, MazeEncoding.ToBytes(maze).Length);
    }

    [Fact]
    public void Rejects_data_for_a_different_size()
    {
        var maze = Generators.Kruskal(new MazeSize(4, 4), new Random(1));
        Assert.Throws<ArgumentException>(() => MazeEncoding.FromBytes(new MazeSize(5, 4), MazeEncoding.ToBytes(maze)));
    }

    [Fact]
    public void Rejects_corrupted_passages()
    {
        var bytes = MazeEncoding.ToBytes(Generators.Kruskal(new MazeSize(4, 4), new Random(1)));
        bytes[0] = (byte)Direction.West;
        Assert.Throws<ArgumentException>(() => MazeEncoding.FromBytes(new MazeSize(4, 4), bytes));
    }
}
