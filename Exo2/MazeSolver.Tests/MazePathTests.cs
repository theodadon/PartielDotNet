using MazeSolver.Core;
using Xunit;

namespace MazeSolver.Tests;

public sealed class MazePathTests
{
    [Fact]
    public void GetShortestPath_Open3x3()
    {
        var maze = new Maze(new[]
        {
            "D..",
            "...",
            "..S"
        });

        var dist = maze.GetDistance();
        var path = maze.GetShortestPath();

        Assert.Equal(dist + 1, path.Count);
        Assert.Equal((maze.Exit.Col, maze.Exit.Row), path[0]);
        Assert.Equal((maze.Start.Col, maze.Start.Row), path[^1]);

        for (int i = 0; i < path.Count - 1; i++)
        {
            var (x, y) = path[i];
            var (nx, ny) = path[i + 1];
            Assert.Equal(maze.Distances[y, x] - 1, maze.Distances[ny, nx]);
        }
    }

    [Fact]
    public void GetShortestPath_WithWalls()
    {
        var maze = new Maze(new[]
        {
            "D.#.",
            "..#S",
            "...."
        });

        var dist = maze.GetDistance();
        var path = maze.GetShortestPath();

        Assert.Equal(dist + 1, path.Count);
        Assert.Equal((maze.Exit.Col, maze.Exit.Row), path[0]);
        Assert.Equal((maze.Start.Col, maze.Start.Row), path[^1]);
    }
}
