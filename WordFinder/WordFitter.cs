using System.Net.NetworkInformation;

namespace WordFinder;

public enum Direction
{
    None = 0,
    Left = 1,
    Top = 2,
    Right = 4,
    Bottom = 8,
    Any = Left | Top | Right | Bottom
}

public class WordFitter
{
    private GameLetter[,] _table;
    private int _gridSize;
    private int _row;
    private int _col;
    private TableService _tableService;

    public WordFitter(TableService tableService)
    {
        _tableService = tableService;
    }

    public void Initialize(int gridSize)
    {
        _table = new GameLetter[gridSize, gridSize];
        _tableService.Initialize(_table);
        _gridSize = gridSize;
        _row = -1;
        _col = -1;
    }

    public GameLetter[] Flush()
    {
        var letters = new GameLetter[_gridSize * _gridSize];
        int index = 0;
        for (int i = 0; i < _gridSize; i++)
        {
            for (int j = 0; j < _gridSize; j++)
            {
                letters[index++] = _table[i, j];
            }
        }
        return letters;
    }

    public bool FitWord(GameWord gameWord)
    {
        if (gameWord is null)
            return false;

        // TODO: create unit tests for TableService
        // _table[0, 2] = new GameLetter("T");
        // _table[4, 2] = new GameLetter("B");
        // _table[2, 0] = new GameLetter("L");
        // _table[2, 4] = new GameLetter("R");
        // _table[2, 2] = new GameLetter("A");
        // _tableService.ShiftBottom();

        bool success = true;
        for (int attempt = 0; attempt < 50; attempt++)
        {
            success = true;
            int letterIndex = 0;
            foreach (var ch in gameWord.Word)
            {
                if (!UpdateCursorPosition(letterIndex, Direction.Any))
                {
                    success = false;
                    _col = -1;
                    _row = -1;

                    for (int i = 0; i < _gridSize; i++)
                        for (int j = 0; j < _gridSize; j++)
                            _table[i, j] = null;
                    break;
                }
                _table[_row, _col] = new GameLetter(ch.ToString().ToUpper())
                {
                    IsMainLetter = true
                };
                letterIndex++;
            }
            if (success)
                break;
        }

        return success;
    }

    public void FitBlank()
    {
        for (int i = 0; i < _gridSize; i++)
        {
            for (var j = 0; j < _gridSize; j++)
            {
                if (_table[i, j] is not null)
                    continue;

                _table[i, j] = new GameLetter(((char)Random.Shared.Next(65, 90)).ToString());
            }
        }
    }

    private Direction GetRandomDirection(Direction allowedDirections)
    {
        if (allowedDirections == Direction.None)
            return allowedDirections;

        List<Direction> possibleValues = new();
        if (allowedDirections.HasFlag(Direction.Left))
            possibleValues.Add(Direction.Left);
        if (allowedDirections.HasFlag(Direction.Top))
            possibleValues.Add(Direction.Top);
        if (allowedDirections.HasFlag(Direction.Right))
            possibleValues.Add(Direction.Right);
        if (allowedDirections.HasFlag(Direction.Bottom))
            possibleValues.Add(Direction.Bottom);

        var index = Random.Shared.Next(0, possibleValues.Count - 1);
        return possibleValues[index];
    }

    private bool UpdateCursorPosition(int letterIndex, Direction allowedDirections)
    {
        if (_row < 0 || _col < 0)
        {
            _row = Random.Shared.Next(0, _gridSize - 1);
            _col = Random.Shared.Next(0, _gridSize - 1);
            return true;
        }

        var direction = letterIndex == 1 ? Direction.Right : GetRandomDirection(allowedDirections);
        switch (direction)
        {
            case Direction.Left:
                if (_row == 0 && _tableService.CanShiftRight())
                {
                    _tableService.ShiftRight();
                    _row++;
                }

                if (_row > 0 && _table[_row - 1, _col] is null)
                    _row -= 1;
                break;
            case Direction.Top:
                if (_col == 0 && _tableService.CanShiftBottom())
                {
                    _tableService.ShiftBottom();
                    _col++;
                }

                if (_col > 0 && _table[_row, _col - 1] is null)
                    _col -= 1;
                break;
            case Direction.Right:
                if (_col == _gridSize - 1 && _tableService.CanShiftLeft())
                {
                    _tableService.ShiftLeft();
                    _col--;
                }

                if (_col < _gridSize - 1 && _table[_row, _col + 1] is null)
                    _col += 1;
                break;
            case Direction.Bottom:
                if (_row == _gridSize - 1 && _tableService.CanShiftTop())
                {
                    _tableService.ShiftTop();
                    _row--;
                }

                if (_row < _gridSize - 1 && _table[_row + 1, _col] is null)
                    _row += 1;
                break;
        }

        if (_table[_row, _col] is not null)
        {
            allowedDirections &= ~allowedDirections;
            if (allowedDirections == Direction.None)
                return false;

            return UpdateCursorPosition(letterIndex, allowedDirections);
        }
        return true;
    }
}