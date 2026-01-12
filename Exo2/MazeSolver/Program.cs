using MazeSolver.Core;

Console.WriteLine("MazeSolver (Exo2) - ready");

var lines = new[]
{
    "D..#.",
    "##...",
    ".#.#.",
    "...#.",
    "####S"
};

var maze = new Maze(lines);
Console.WriteLine($"Start={maze.Start}, Exit={maze.Exit}, Size={maze.Rows}x{maze.Cols}");
