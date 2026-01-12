using MazeSolver.Core;
using Xunit;

namespace MazeSolver.Tests;

public sealed class MazeDistanceTests
{
    [Fact]
    public void GetDistance_StraightLine()
    {
        var maze = new Maze(new[]
        {
            "D..S"
        });

        var d = maze.GetDistance();

        Assert.Equal(3, d);
    }

    [Fact]
    public void GetDistance_WithWalls()
    {
        var maze = new Maze(new[]
        {
            "D.#.",
            "..#S",
            "...."
        });

        var d = maze.GetDistance();

        Assert.Equal(6, d);
    }
}
