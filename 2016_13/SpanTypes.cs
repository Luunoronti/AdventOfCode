
public ref struct SpanQueue<T>
{
    private Span<T> Buffer;
    private int Head;
    private int Tail;
    private int CountValue;

    public SpanQueue(Span<T> buffer)
    {
        Buffer = buffer;
        Head = 0;
        Tail = 0;
        CountValue = 0;
    }

    public int Count => CountValue;
    public bool IsEmpty => CountValue == 0;
    public int Capacity => Buffer.Length;

    public void Enqueue(T value)
    {
        if (CountValue == Buffer.Length)
            throw new InvalidOperationException("Queue is full.");

        Buffer[Tail] = value;
        Tail++;
        if (Tail == Buffer.Length) Tail = 0;
        CountValue++;
    }

    public T Dequeue()
    {
        if (CountValue == 0)
            throw new InvalidOperationException("Queue is empty.");

        var result = Buffer[Head];
        Head++;
        if (Head == Buffer.Length) Head = 0;
        CountValue--;
        return result;
    }

    public T Peek()
    {
        if (CountValue == 0)
            throw new InvalidOperationException("Queue is empty.");

        return Buffer[Head];
    }
}

public ref struct SpanHashSet<T>
{
    Span<T> values;
    Span<byte> states;
    int count;

    public SpanHashSet(Span<T> values, Span<byte> states)
    {
        this.values = values;
        this.states = states;
        count = 0;
        for (var i = 0; i < states.Length; i++) states[i] = 0;
    }

    public int Count => count;
    public int Capacity => values.Length;

    public bool Add(T value)
    {
        var comparer = EqualityComparer<T>.Default;
        var hash = comparer.GetHashCode(value) & 0x7FFFFFFF;
        var idx = hash % values.Length;
        var start = idx;
        while (true)
        {
            var state = states[idx];
            if (state == 0)
            {
                values[idx] = value;
                states[idx] = 1;
                count++;
                return true;
            }
            if (state == 1 && comparer.Equals(values[idx], value)) return false;
            idx++;
            if (idx == values.Length) idx = 0;
            if (idx == start) throw new InvalidOperationException("Set is full");
        }
    }

    public bool Contains(T value)
    {
        if (values.Length == 0) return false;
        var comparer = EqualityComparer<T>.Default;
        var hash = comparer.GetHashCode(value) & 0x7FFFFFFF;
        var idx = hash % values.Length;
        var start = idx;
        while (true)
        {
            var state = states[idx];
            if (state == 0) return false;
            if (state == 1 && comparer.Equals(values[idx], value)) return true;
            idx++;
            if (idx == values.Length) idx = 0;
            if (idx == start) return false;
        }
    }
}

public ref struct SpanDictionary<TKey, TValue>
{
    Span<TKey> keys;
    Span<TValue> values;
    Span<byte> states;
    int count;

    public SpanDictionary(Span<TKey> keys, Span<TValue> values, Span<byte> states)
    {
        if (keys.Length != values.Length || keys.Length != states.Length) throw new ArgumentException();
        this.keys = keys;
        this.values = values;
        this.states = states;
        count = 0;
        for (var i = 0; i < states.Length; i++) states[i] = 0;
    }

    public int Count => count;
    public int Capacity => keys.Length;

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (keys.Length == 0)
        {
            value = default!;
            return false;
        }
        var comparer = EqualityComparer<TKey>.Default;
        var hash = comparer.GetHashCode(key) & 0x7fffffff;
        var idx = hash % keys.Length;
        var start = idx;
        while (true)
        {
            var state = states[idx];
            if (state == 0)
            {
                value = default!;
                return false;
            }
            if (state == 1 && comparer.Equals(keys[idx], key))
            {
                value = values[idx];
                return true;
            }
            idx++;
            if (idx == keys.Length) idx = 0;
            if (idx == start)
            {
                value = default!;
                return false;
            }
        }
    }

    public void Set(TKey key, TValue value)
    {
        if (keys.Length == 0) throw new InvalidOperationException();
        var comparer = EqualityComparer<TKey>.Default;
        var hash = comparer.GetHashCode(key) & 0x7fffffff;
        var idx = hash % keys.Length;
        var start = idx;
        while (true)
        {
            var state = states[idx];
            if (state == 0)
            {
                keys[idx] = key;
                values[idx] = value;
                states[idx] = 1;
                count++;
                return;
            }
            if (state == 1 && comparer.Equals(keys[idx], key))
            {
                values[idx] = value;
                return;
            }
            idx++;
            if (idx == keys.Length) idx = 0;
            if (idx == start) throw new InvalidOperationException("Dictionary is full");
        }
    }

    public bool TryAdd(TKey key, TValue value)
    {
        if (keys.Length == 0) throw new InvalidOperationException();
        var comparer = EqualityComparer<TKey>.Default;
        var hash = comparer.GetHashCode(key) & 0x7fffffff;
        var idx = hash % keys.Length;
        var start = idx;
        while (true)
        {
            var state = states[idx];
            if (state == 0)
            {
                keys[idx] = key;
                values[idx] = value;
                states[idx] = 1;
                count++;
                return true;
            }
            if (state == 1 && comparer.Equals(keys[idx], key)) return false;
            idx++;
            if (idx == keys.Length) idx = 0;
            if (idx == start) throw new InvalidOperationException("Dictionary is full");
        }
    }

    public bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out _);
    }

    public void Clear()
    {
        for (var i = 0; i < states.Length; i++) states[i] = 0;
        count = 0;
    }


    public TValue this[TKey key]
    {
        get => TryGetValue(key, out var value) ? value : throw new InvalidOperationException("Key not found in the dictionary.");
        set => Set(key, value);
    }

}