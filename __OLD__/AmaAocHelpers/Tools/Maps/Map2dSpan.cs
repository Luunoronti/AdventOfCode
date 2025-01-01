using System.Runtime.CompilerServices;

namespace AmaAocHelpers.Tools.Maps;

public ref struct Map2dSpan<T>
{
    private Span<T> __innerSpan;
    private int _width;
    private int _height;
    private OutOfBoundsBehavior _outOfBoundsBehavior;

    public Map2dSpan(Map2d<T> map)
    {
        _width = map.Width;
        _height = map.Height;
        __innerSpan = map._map.AsSpan();
        _outOfBoundsBehavior = map.OutOfBoundsBehavior;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnOOB(int x, int y)
    {
        if (_outOfBoundsBehavior == OutOfBoundsBehavior.RaiseException)
            throw new IndexOutOfRangeException($"Coordinates are out of bounds: {x} ({_width}), {y} ({_height})");
        if (_outOfBoundsBehavior == OutOfBoundsBehavior.LogReturnDefault)
            Log.WriteErrorLine($"Coordinates are out of bounds: {x} ({_width}), {y} ({_height})");
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnOOB(int offset)
    {
        if (_outOfBoundsBehavior == OutOfBoundsBehavior.RaiseException)
            throw new IndexOutOfRangeException($"Coordinates are out of bounds: {offset} ({__innerSpan.Length})");
        if (_outOfBoundsBehavior == OutOfBoundsBehavior.LogReturnDefault)
            Log.WriteErrorLine($"Coordinates are out of bounds: {offset} ({__innerSpan.Length})");
    }

    /// <summary>
    /// The number of items in the span.
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => __innerSpan.Length;
    }
    /// <summary>
    /// The Width of the map span.
    /// </summary>
    public int Width
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _width;
    }
    /// <summary>
    /// The Height of the map span.
    /// </summary>
    public int Height
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _height;
    }


    /// <summary>
    /// Gets a value indicating whether this <see cref="Span{T}"/> is empty.
    /// </summary>
    /// <value><see langword="true"/> if this span is empty; otherwise, <see langword="false"/>.</value>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => __innerSpan.Length == 0;
    }

    /// <summary>
    /// Returns a reference to specified element of the Span.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="System.IndexOutOfRangeException">
    /// Thrown when index less than 0 or index greater than or equal to Length
    /// </exception>
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (index >= 0 || index < Length)
                return __innerSpan[index];
            OnOOB(index);
            return default!;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (index >= 0 || index < Length)
                __innerSpan[index] = value;
            OnOOB(index);
        }
    }

    /// <summary>
    /// Returns a reference to specified element of the Span.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="System.IndexOutOfRangeException">
    /// Thrown when index less than 0 or index greater than or equal to Length
    /// </exception>
    public T this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (x >= 0 || y >= 0 || x < _width || y < _height)
                return __innerSpan[y * _width + x];
            OnOOB(x, y);
            return default!;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (x >= 0 || y >= 0 || x < _width || y < _height)
                __innerSpan[y * _width + x] = value;
            OnOOB(x, y);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Map2dSpan<T> left, Map2dSpan<T> right) => !(left == right);

    /// <summary>
    /// Defines an implicit conversion of a map to a Map2dSpan{T}/>
    /// </summary>
    public static implicit operator Map2dSpan<T>(Map2d<T> map) => new(map);


    public static Span<T> Empty => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => __innerSpan.Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fill(T value) => __innerSpan.Fill(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Map2dSpan<T> destination)
    {
        destination._width = _width;
        destination._height = _height;
        __innerSpan.CopyTo(destination.__innerSpan);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Map2dSpan<T> left, Map2dSpan<T> right) =>
    left._width == right._width &&
    left._height == right._height &&
    left.__innerSpan == right.__innerSpan;

    public override string ToString()
    {
        if (typeof(T) == typeof(char))
            return __innerSpan.ToString();
        return $"Map2dSpan<{typeof(T).Name}>[{__innerSpan.Length}]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] ToArray() => __innerSpan.ToArray();

    public readonly override bool Equals(object? obj)
    {
        if (obj == null) return false;
        var m = (Map2dSpan<T>)obj!;
        return this == m;
    }
    public override int GetHashCode() => _width.GetHashCode() ^ _height.GetHashCode();

}
