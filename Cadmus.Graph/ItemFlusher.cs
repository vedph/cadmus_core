using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Cadmus.Graph;

/// <summary>
/// Generic items flusher. This tiny helper is just a wrapper around
/// <see cref="ConcurrentQueue{T}"/>, which allows consumers to <see cref="Add"/>
/// one item after another to a queue, and have it automatically flushed when
/// its size reaches the specified value. When the flusher is disposed,
/// all the remaining items are flushed at once.
/// </summary>
/// <typeparam name="T">The item's type.</typeparam>
public sealed class ItemFlusher<T> : IDisposable
{
    private readonly ConcurrentQueue<T> _queue;
    private readonly int _size;
    private readonly Action<IList<T>> _flush;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemFlusher{T}" /> class.
    /// </summary>
    /// <param name="flush">The flush method to be invoked.</param>
    /// <param name="size">The cache size value. When items added reach
    /// this size, they get flushed.</param>
    /// <exception cref="ArgumentOutOfRangeException">size</exception>
    /// <exception cref="ArgumentNullException">flush</exception>
    public ItemFlusher(Action<IList<T>> flush, int size = 100)
    {
        if (size < 1) throw new ArgumentOutOfRangeException(nameof(size));

        _queue = new ConcurrentQueue<T>();
        _flush = flush ?? throw new ArgumentNullException(nameof(flush));
        _size = size;
    }

    /// <summary>
    /// Adds the specified item to the queue.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Add(T item)
    {
        _queue.Enqueue(item);

        if (_queue.Count >= _size)
        {
            List<T> items = new();
            for (int i = 0; i < _size; i++)
            {
                if (_queue.TryDequeue(out T? cached))
                    items.Add(cached);
            }
            _flush(items);
        }
    }

    #region IDisposable Support
    private bool _disposed; // To detect redundant calls

    void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                T[] items = new T[_queue.Count];
                _queue.CopyTo(items, 0);
                _flush(items);
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing,
    /// releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }
    #endregion
}
