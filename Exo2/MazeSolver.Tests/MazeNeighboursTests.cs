using MazeSolver.Core;
using Xunit;

namespace MazeSolver.Tests;

public sealed class MazeNeighboursTests
{

    private static Maze Open3x3()
        => new Maze(new[]
        {
            "D..",
            "...",
            "..S"
        });

    [Fact]
    public void GetNeighbours_AllFourAvailable_FromCenter()
    {
        var maze = Open3x3();

        var n = maze.GetNeighbours(1, 1);

        Assert.Equal(4, n.Count);
        Assert.Contains((0, 1), n);
        Assert.Contains((2, 1), n);
        Assert.Contains((1, 0), n);
        Assert.Contains((1, 2), n);
    }

    [Fact]
    public void GetNeighbours_ExcludesOutside_TopBorder()
    {
        var maze = Open3x3();

        var n = maze.GetNeighbours(1, 0);

        Assert.DoesNotContain((1, -1), n);
        Assert.Equal(2, n.Count);
        Assert.Contains((2, 0), n);
        Assert.Contains((1, 1), n);
    }

    [Fact]
    public void GetNeighbours_ExcludesOutside_BottomBorder()
    {
        var maze = Open3x3();

        var n = maze.GetNeighbours(1, 2);

        Assert.DoesNotContain((1, 3), n);
        Assert.Equal(3, n.Count);
        Assert.Contains((0, 2), n);
        Assert.Contains((2, 2), n);
        Assert.Contains((1, 1), n);
    }

    [Fact]
    public void GetNeighbours_ExcludesOutside_LeftBorder()
    {
        var maze = Open3x3();

        var n = maze.GetNeighbours(0, 1);

        Assert.DoesNotContain((-1, 1), n);
        Assert.Equal(2, n.Count);
        Assert.Contains((1, 1), n);
        Assert.Contains((0, 2), n);
    }

    [Fact]
    public void GetNeighbours_ExcludesOutside_RightBorder()
    {
        var maze = Open3x3();

        var n = maze.GetNeighbours(2, 1);

        Assert.DoesNotContain((3, 1), n);
        Assert.Equal(3, n.Count);
        Assert.Contains((1, 1), n);
        Assert.Contains((2, 0), n);
        Assert.Contains((2, 2), n);
    }

    [Fact]
    public void GetNeighbours_ExcludesWalls()
    {
        var maze = new Maze(new[]
        {
            "D..",
            ".#.",
            "..S"
        });

        var n = maze.GetNeighbours(1, 0);

        Assert.DoesNotContain((1, 1), n);
        Assert.Equal(1, n.Count);
        Assert.Contains((2, 0), n);
    }

    [Fact]
    public void GetNeighbours_ExcludesStartCell()
    {
        var maze = Open3x3();

        var n = maze.GetNeighbours(1, 0);

        Assert.DoesNotContain((0, 0), n);
    }

    [Fact]
    public void GetNeighbours_Throws_WhenCurrentIsOutside()
    {
        var maze = Open3x3();

        Assert.Throws<ArgumentOutOfRangeException>(() => maze.GetNeighbours(-1, 0));
    }
}
