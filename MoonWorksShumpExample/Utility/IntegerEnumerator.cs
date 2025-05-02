namespace MoonWorksShumpExample.Utility;

public ref struct IntegerEnumerator
{
    private int _i;
    private int _end;
    private int _increment;

    public readonly IntegerEnumerator GetEnumerator() =>
        this;

    public IntegerEnumerator(int start, int end)
    {
        _i = start;
        _end = end;

        if(_end >= start)
        {
            _increment = 1;
        }
        else if(end < start)
        {
            _increment = -1;
        }
        else
        {
            _increment = 0;
        }
    }

    public bool MoveNext()
    {
        _i += _increment;
        return (_increment > 0 && _i <= _end) || (_increment < 0 && _i >= _end);
    }

    public readonly int Current => 
        _i;
}
