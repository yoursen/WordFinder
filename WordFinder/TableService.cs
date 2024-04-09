namespace WordFinder;

public class TableService
{
    private GameLetter[,] _table;
    private int _gridSize;

    public void Initialize(GameLetter[,] table)
    {
        _table = table;
        _gridSize = table.GetLength(0);
    }

    public bool CanShiftRight()
    {
        for (int i = 0; i < _gridSize; i++)
            if (_table[i, _gridSize - 1] is not null)
                return false;
        return true;
    }

    public bool CanShiftLeft()
    {
        for (int i = 0; i < _gridSize; i++)
            if (_table[i, 0] is not null)
                return false;
        return true;
    }

    public bool CanShiftTop()
    {
        for (int i = 0; i < _gridSize; i++)
            if (_table[0, i] is not null)
                return false;
        return true;
    }

    public bool CanShiftBottom()
    {
        for (int i = 0; i < _gridSize; i++)
            if (_table[_gridSize - 1, i] is not null)
                return false;
        return true;
    }

    public void ShiftRight()
    {
        for (int i = 0; i < _gridSize; i++)
        {
            var bak = _table[i, _gridSize - 1];
            for (int j = _gridSize - 1; j > 0; j--)
            {
                _table[i, j] = _table[i, j - 1];
            }
            _table[i, 0] = bak;
        }
    }

    public void ShiftLeft()
    {
        for (int i = 0; i < _gridSize; i++)
        {
            var bak = _table[i, 0];
            for (int j = 0; j < _gridSize - 1; j++)
            {
                _table[i, j] = _table[i, j + 1];
            }
            _table[i, _gridSize - 1] = bak;
        }
    }

    public void ShiftTop()
    {
        for (int j = 0; j < _gridSize; j++)
        {
            var bak = _table[0, j];
            for (int i = 0; i < _gridSize - 1; i++)
            {
                _table[i, j] = _table[i + 1, j];
            }
            _table[_gridSize - 1, j] = bak;
        }
    }

    public void ShiftBottom()
    {
        for (int j = 0; j < _gridSize; j++)
        {
            var bak = _table[_gridSize - 1, j];
            for (int i = _gridSize - 1; i > 0; i--)
            {
                _table[i, j] = _table[i - 1, j];
            }
            _table[0, j] = bak;
        }
    }
}