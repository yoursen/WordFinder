namespace WordFinder.Models;

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
    private readonly string[] UALetters = new[] { "Й", "Ц", "У", "К", "Е", "Н", "Г", "Ѓ", "Ш", "Щ"
        , "З", "Х", "Ї", "Ф", "І", "В", "А", "П", "Р", "О"
        , "Л", "Д", "Ж", "Є", "Я", "Ч", "С", "М", "И", "Т"
        , "Ь", "Б", "Ю"};

    private readonly string[] ENLetters = new[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P"
        , "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z"
        , "X", "C", "V", "B", "N", "M"};
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

    private void ResetField()
    {
        _col = -1;
        _row = -1;
        for (int i = 0; i < _gridSize; i++)
            for (int j = 0; j < _gridSize; j++)
                _table[i, j] = null;
    }

    /// <summary>
    /// Uses dummy alg for fitting word, when random position failed. */
    /// </summary>
    public void FitWordDummy(GameWord gameWord)
    {
        ResetField();

        int letterIndex = 0;
        int direction = 1;
        _row = 1;
        _col = 1;
        foreach (var ch in gameWord.Word)
        {
            _table[_row, _col] = new GameLetter(ch.ToString().ToUpper())
            {
                IsMainLetter = true,
                LetterIndex = letterIndex
            };
            letterIndex++;

            _col += direction;
            if (_col == 5)
            {
                _row++;
                _col = 4;
                direction = -1;
            }
            else if (_col == -1)
            {
                _col = 0;
                _row++;
                direction = 1;
            }
        }
    }

    public bool FitWord(GameWord gameWord)
    {
        if (gameWord is null)
            return false;

        ResetField();
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
                    ResetField();
                    break;
                }
                _table[_row, _col] = new GameLetter(ch.ToString().ToUpper())
                {
                    IsMainLetter = true,
                    LetterIndex = letterIndex
                };
                letterIndex++;
            }
            if (success)
                break;
        }
        return success;
    }

    public void FitSecondaryWord(GameWord gameWord)
    {
        var blankCell = GetFirstBlankCell();
        if (blankCell.row < 0 || blankCell.col < 0)
            return;

        _row = blankCell.row;
        _col = blankCell.col;

        int letterIndex = 0;
        foreach (var ch in gameWord.Word)
        {
            _table[_row, _col] = new GameLetter(ch.ToString().ToUpper());
            letterIndex++;

            if (!UpdateCursorPosition(letterIndex, Direction.Right | Direction.Top | Direction.Bottom, false))
                break;
        }
    }

    private (int row, int col) GetFirstBlankCell()
    {
        for (int i = 0; i < _gridSize; i++)
        {
            for (var j = 0; j < _gridSize; j++)
            {
                if (_table[i, j] is null)
                    return (i, j);
            }
        }

        return (-1, -1);
    }

    public void FitBlank(GameLanguage language)
    {
        for (int i = 0; i < _gridSize; i++)
        {
            for (var j = 0; j < _gridSize; j++)
            {
                if (_table[i, j] is not null)
                    continue;

                string letter = null;
                switch (language)
                {
                    case GameLanguage.Ukrainian:
                        letter = GetLetter(UALetters);
                        break;
                    default:
                        letter = GetLetter(ENLetters);
                        break;
                }
                _table[i, j] = new GameLetter(letter);
            }
        }
    }

    private static string GetLetter(string[] alphabet)
    {
        string letter;
        var idx = Random.Shared.Next(0, alphabet.Length);
        letter = alphabet[idx];
        return letter;
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

    private bool UpdateCursorPosition(int letterIndex, Direction allowedDirections, bool shiftAllowed = true)
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
                if (_col == 0 && _tableService.CanShiftRight())
                {
                    if (!shiftAllowed)
                        return false;
                    _tableService.ShiftRight();
                    _col++;
                }

                if (_col > 0 && _table[_row, _col - 1] is null)
                    _col--;
                break;
            case Direction.Top:
                if (_row == 0 && _tableService.CanShiftBottom())
                {
                    if (!shiftAllowed)
                        return false;
                    _tableService.ShiftBottom();
                    _row++;
                }

                if (_row > 0 && _table[_row - 1, _col] is null)
                    _row--;
                break;
            case Direction.Right:
                if (_col == _gridSize - 1 && _tableService.CanShiftLeft())
                {
                    if (!shiftAllowed)
                        return false;
                    _tableService.ShiftLeft();
                    _col--;
                }

                if (_col < _gridSize - 1 && _table[_row, _col + 1] is null)
                    _col++;
                break;
            case Direction.Bottom:
                if (_row == _gridSize - 1 && _tableService.CanShiftTop())
                {
                    if (!shiftAllowed)
                        return false;
                    _tableService.ShiftTop();
                    _row--;
                }

                if (_row < _gridSize - 1 && _table[_row + 1, _col] is null)
                    _row++;
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