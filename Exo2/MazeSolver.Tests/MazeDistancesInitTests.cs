using MazeSolver.Core;
using Xunit;

namespace MazeSolver.Tests;

public sealed class MazeDistancesInitTests
{
    [Fact]
    public void Distances_HasSameSizeAsGrid_And_IsInitializedToZero()
    {
        var input = new[]
        {
            "D.#....",
            ".#..#..",
            "...#..S"
        };

        var maze = new Maze(input);

        Assert.Equal(maze.Grid.GetLength(0), maze.Distances.GetLength(0));
        Assert.Equal(maze.Grid.GetLength(1), maze.Distances.GetLength(1));

        for (int r = 0; r < maze.Rows; r++)
        {
            for (int c = 0; c < maze.Cols; c++)
            {
                Assert.Equal(0, maze.Distances[r, c]);
            }
        }
    }
}