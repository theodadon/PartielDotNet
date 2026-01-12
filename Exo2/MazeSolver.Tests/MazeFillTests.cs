using MazeSolver.Core;
using Xunit;

namespace MazeSolver.Tests;

public sealed class MazeFillTests
{
    [Fact]
    public void Constructor_InitializesQueue_WithStartAndDistance0()
    {
        var maze = new Maze(new[]
        {
            "D.S"
        });

        Assert.Single(maze.Frontier);
        var item = maze.Frontier.Peek();
        Assert.Equal(0, item.Distance);
        Assert.Equal(0, item.X);
        Assert.Equal(0, item.Y);
    }

    [Fact]
    public void Fill_ReturnsFalse_UntilExitThenTrue()
    {
        var maze = new Maze(new[]
        {
            "D.S"
        });

        Assert.False(maze.Fill()); 
        Assert.False(maze.Fill()); 
        Assert.True(maze.Fill());  

        Assert.True(maze.Distances[0, 2] > 0);
    }

    [Fact]
    public void Fill_IgnoresCellIfAlreadyHasDistance_AndDoesNotEnqueueNeighbours()
    {
        var maze = new Maze(new[]
        {
            "D..",
            "...",
            "..S"
        });

        Assert.False(maze.Fill());
        Assert.True(maze.Frontier.Count >= 1);

        var next = maze.Frontier.Peek();
        maze.Distances[next.Y, next.X] = 99;

        var before = maze.Frontier.Count;

        Assert.False(maze.Fill());

        var after = maze.Frontier.Count;
        Assert.Equal(before - 1, after);
    }

    [Fact]
    public void Fill_EnqueuesNeighbours_WithDistancePlusOne()
    {
        var maze = new Maze(new[]
        {
            "D..",
            "...",
            "..S"
        });

        Assert.False(maze.Fill());

        Assert.Contains(maze.Frontier, i => i.X == 1 && i.Y == 0 && i.Distance == 1);
        Assert.Contains(maze.Frontier, i => i.X == 0 && i.Y == 1 && i.Distance == 1);
    }
}
