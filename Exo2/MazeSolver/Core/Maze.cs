namespace MazeSolver.Core;

public sealed class Maze
{
    public bool[,] Grid { get; }
    public (int Row, int Col) Start { get; }
    public (int Row, int Col) Exit { get; }
    public int[,] Distances { get; }

    public int Rows => Grid.GetLength(0);
    public int Cols => Grid.GetLength(1);

    public Queue<FrontierItem> Frontier { get; } = new();

    private readonly bool[,] _visited;

    public Maze(string[] lines)
    {
        if (lines is null) throw new ArgumentNullException(nameof(lines));
        if (lines.Length == 0) throw new ArgumentException(nameof(lines));

        int rows = lines.Length;
        int cols = lines[0]?.Length ?? 0;
        if (cols == 0) throw new ArgumentException(nameof(lines));

        for (int r = 0; r < rows; r++)
        {
            if (lines[r] is null) throw new ArgumentException(nameof(lines));
            if (lines[r].Length != cols) throw new ArgumentException(nameof(lines));
        }

        var grid = new bool[rows, cols];
        var distances = new int[rows, cols];
        _visited = new bool[rows, cols];

        (int Row, int Col)? start = null;
        (int Row, int Col)? exit = null;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                char ch = lines[y][x];
                switch (ch)
                {
                    case '#':
                        grid[y, x] = true;
                        break;

                    case '.':
                        grid[y, x] = false;
                        break;

                    case 'D':
                        if (start is not null) throw new ArgumentException(nameof(lines));
                        start = (y, x);
                        grid[y, x] = false;
                        break;

                    case 'S':
                        if (exit is not null) throw new ArgumentException(nameof(lines));
                        exit = (y, x);
                        grid[y, x] = false;
                        break;

                    default:
                        throw new ArgumentException(nameof(lines));
                }
            }
        }

        if (start is null || exit is null) throw new ArgumentException(nameof(lines));

        Grid = grid;
        Start = start.Value;
        Exit = exit.Value;
        Distances = distances;

        Frontier.Enqueue(new FrontierItem(Start.Col, Start.Row, 0));
    }

    public IList<(int X, int Y)> GetNeighbours(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Cols || y >= Rows)
            throw new ArgumentOutOfRangeException();

        var result = new List<(int X, int Y)>();

        void TryAdd(int nx, int ny)
        {
            if (nx < 0 || ny < 0 || nx >= Cols || ny >= Rows) return;
            if (Grid[ny, nx]) return;
            if (nx == Start.Col && ny == Start.Row) return;
            result.Add((nx, ny));
        }

        TryAdd(x - 1, y);
        TryAdd(x + 1, y);
        TryAdd(x, y - 1);
        TryAdd(x, y + 1);

        return result;
    }

    public bool Fill()
    {
        if (Frontier.Count == 0)
            return false;

        var item = Frontier.Dequeue();
        int x = item.X;
        int y = item.Y;
        int d = item.Distance;

        if (_visited[y, x])
            return false;

        bool isStart = (x == Start.Col && y == Start.Row);

        if (!isStart && Distances[y, x] != 0)
        {
            _visited[y, x] = true;
            return false;
        }

        if (x == Exit.Col && y == Exit.Row)
        {
            if (Distances[y, x] != 0)
            {
                _visited[y, x] = true;
                return false;
            }

            Distances[y, x] = d;
            _visited[y, x] = true;
            return true;
        }

        if (!isStart)
            Distances[y, x] = d;

        _visited[y, x] = true;

        foreach (var (nx, ny) in GetNeighbours(x, y))
        {
            if (_visited[ny, nx]) continue;
            if (Distances[ny, nx] != 0) continue;
            Frontier.Enqueue(new FrontierItem(nx, ny, d + 1));
        }

        return false;
    }

    public int GetDistance()
    {
        while (Frontier.Count > 0)
        {
            if (Fill())
                return Distances[Exit.Row, Exit.Col];
        }

        throw new InvalidOperationException();
    }

    public IList<(int X, int Y)> GetShortestPath()
    {
        int exitDistance = Distances[Exit.Row, Exit.Col];
        if (exitDistance == 0)
            throw new InvalidOperationException();

        var path = new List<(int X, int Y)>();

        int x = Exit.Col;
        int y = Exit.Row;
        int d = exitDistance;

        path.Add((x, y));

        while (d > 0)
        {
            bool found = false;

            foreach (var (nx, ny) in AdjacentIncludingStart(x, y))
            {
                if (d == 1)
                {
                    if (nx == Start.Col && ny == Start.Row)
                    {
                        x = nx; y = ny; d--;
                        path.Add((x, y));
                        found = true;
                        break;
                    }
                }
                else
                {
                    if (Distances[ny, nx] == d - 1)
                    {
                        x = nx; y = ny; d--;
                        path.Add((x, y));
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
                throw new InvalidOperationException();
        }

        return path;
    }

    private IEnumerable<(int X, int Y)> AdjacentIncludingStart(int x, int y)
    {
        static IEnumerable<(int X, int Y)> Four(int x, int y)
        {
            yield return (x - 1, y);
            yield return (x + 1, y);
            yield return (x, y - 1);
            yield return (x, y + 1);
        }

        foreach (var (nx, ny) in Four(x, y))
        {
            if (nx < 0 || ny < 0 || nx >= Cols || ny >= Rows) continue;
            if (Grid[ny, nx]) continue;
            yield return (nx, ny);
        }
    }

    public readonly record struct FrontierItem(int X, int Y, int Distance);
}
