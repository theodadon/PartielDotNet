using MazeSolver.Core;
using Xunit;

namespace MazeSolver.Tests;

public sealed class MazeConstructorTests
{
    [Fact]
    public void Constructor_FromExample_SetsStartExit_AndInterpretsWallsAndEmptyCells()
    {
        var input = new[]
        {
            "D..#.",
            "##...",
            ".#.#.",
            "...#.",
            "####S"
        };

        var maze = new Maze(input);

        Assert.Equal((0, 0), maze.Start);
        Assert.Equal((4, 4), maze.Exit);

        Assert.False(maze.Grid[0, 0]); 
        Assert.False(maze.Grid[0, 1]); 
        Assert.True (maze.Grid[0, 3]); 

        Assert.True (maze.Grid[1, 0]); 
        Assert.True (maze.Grid[1, 1]); 
        Assert.False(maze.Grid[1, 2]); 

        Assert.True (maze.Grid[2, 1]); 
        Assert.False(maze.Grid[3, 2]); 

        Assert.False(maze.Grid[4, 4]); 
    }
}
