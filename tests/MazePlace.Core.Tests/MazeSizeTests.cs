using MazePlace.Core;

namespace MazePlace.Core.Tests;

public class MazeSizeTests
{
    [Fact]
    public void IndexOf_and_CellAt_round_trip()
    {
        var size = new MazeSize(4, 3, 2);
        for (var i = 0; i < size.CellCount; i++)
            Assert.Equal(i, size.IndexOf(size.CellAt(i)));
    }

    [Fact]
    public void CellAt_orders_columns_then_rows_then_levels()
    {
        var size = new MazeSize(4, 3, 2);
        Assert.Equal(new Cell(1, 0, 0), size.CellAt(1));
        Assert.Equal(new Cell(0, 1, 0), size.CellAt(4));
        Assert.Equal(new Cell(0, 0, 1), size.CellAt(12));
        Assert.Equal(new Cell(3, 2, 1), size.CellAt(23));
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(1, 0, 1)]
    [InlineData(1, 1, 0)]
    public void Rejects_empty_dimensions(int columns, int rows, int levels) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new MazeSize(columns, rows, levels));

    [Fact]
    public void Corner_cell_has_only_inward_neighbours()
    {
        var size = new MazeSize(3, 3, 3);
        var directions = size.Neighbours(0).Select(n => n.Direction).ToHashSet();
        Assert.Equal(new HashSet<Direction> { Direction.East, Direction.South, Direction.Up }, directions);
    }

    [Fact]
    public void Centre_cell_has_six_neighbours()
    {
        var size = new MazeSize(3, 3, 3);
        Assert.Equal(6, size.Neighbours(size.IndexOf(new Cell(1, 1, 1))).Count());
    }

    [Fact]
    public void Steps_do_not_wrap_around_rows()
    {
        var size = new MazeSize(3, 3);
        Assert.False(size.TryStep(size.IndexOf(new Cell(2, 0, 0)), Direction.East, out _));
        Assert.False(size.TryStep(size.IndexOf(new Cell(0, 1, 0)), Direction.West, out _));
    }

    [Fact]
    public void Opposite_directions_cancel_out()
    {
        foreach (var direction in Directions.All)
        {
            var (c1, r1, l1) = direction.Offset();
            var (c2, r2, l2) = direction.Opposite().Offset();
            Assert.Equal((0, 0, 0), (c1 + c2, r1 + r2, l1 + l2));
        }
    }
}
