namespace LockKeywordSample.Models;

/// <summary>
/// Represents a shared counter for demonstrating thread-safe operations
/// </summary>
public class SharedCounter
{
    private int _value;
    private readonly object _lockObject = new();
    private readonly Lock _lockType = new();

    public int Value => _value;

    /// <summary>
    /// Increment using traditional lock (Monitor)
    /// </summary>
    public void IncrementTraditional()
    {
        lock (_lockObject)
        {
            _value++;
        }
    }

    /// <summary>
    /// Increment using new Lock type
    /// </summary>
    public void IncrementWithLockType()
    {
        lock (_lockType)
        {
            _value++;
        }
    }

    /// <summary>
    /// Complex operation using traditional lock
    /// </summary>
    public void ComplexOperationTraditional(int iterations)
    {
        lock (_lockObject)
        {
            for (int i = 0; i < iterations; i++)
            {
                _value = (_value * 2) % 1000000;
                _value += i;
            }
        }
    }

    /// <summary>
    /// Complex operation using Lock type
    /// </summary>
    public void ComplexOperationWithLockType(int iterations)
    {
        lock (_lockType)
        {
            for (int i = 0; i < iterations; i++)
            {
                _value = (_value * 2) % 1000000;
                _value += i;
            }
        }
    }

    /// <summary>
    /// Reset the counter value
    /// </summary>
    public void Reset()
    {
        lock (_lockObject)
        {
            _value = 0;
        }
    }
}

/// <summary>
/// Bank account class demonstrating thread-safe financial operations
/// </summary>
public class BankAccount(decimal initialBalance)
{
    private decimal _balance = initialBalance;
    private readonly object _lockObject = new();
    private readonly Lock _lockType = new();

    public decimal Balance => _balance;

    /// <summary>
    /// Withdraw money using traditional lock
    /// </summary>
    public bool WithdrawTraditional(decimal amount)
    {
        lock (_lockObject)
        {
            if (_balance >= amount)
            {
                _balance -= amount;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Withdraw money using Lock type
    /// </summary>
    public bool WithdrawWithLockType(decimal amount)
    {
        lock (_lockType)
        {
            if (_balance >= amount)
            {
                _balance -= amount;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Deposit money using traditional lock
    /// </summary>
    public void DepositTraditional(decimal amount)
    {
        lock (_lockObject)
        {
            _balance += amount;
        }
    }

    /// <summary>
    /// Deposit money using Lock type
    /// </summary>
    public void DepositWithLockType(decimal amount)
    {
        lock (_lockType)
        {
            _balance += amount;
        }
    }
}

/// <summary>
/// Producer-Consumer buffer for demonstrating concurrent operations
/// </summary>
public class ProducerConsumerBuffer<T>
{
    private readonly Queue<T> _queue = [];
    private readonly int _maxSize;
    private readonly object _lockObject = new();
    private readonly Lock _lockType = new();

    public ProducerConsumerBuffer(int maxSize)
    {
        _maxSize = maxSize;
    }

    public int Count => _queue.Count;

    /// <summary>
    /// Add item using traditional lock
    /// </summary>
    public bool TryAddTraditional(T item)
    {
        lock (_lockObject)
        {
            if (_queue.Count < _maxSize)
            {
                _queue.Enqueue(item);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Add item using Lock type
    /// </summary>
    public bool TryAddWithLockType(T item)
    {
        lock (_lockType)
        {
            if (_queue.Count < _maxSize)
            {
                _queue.Enqueue(item);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Remove item using traditional lock
    /// </summary>
    public bool TryRemoveTraditional(out T? item)
    {
        lock (_lockObject)
        {
            if (_queue.Count > 0)
            {
                item = _queue.Dequeue();
                return true;
            }
            item = default;
            return false;
        }
    }

    /// <summary>
    /// Remove item using Lock type
    /// </summary>
    public bool TryRemoveWithLockType(out T? item)
    {
        lock (_lockType)
        {
            if (_queue.Count > 0)
            {
                item = _queue.Dequeue();
                return true;
            }
            item = default;
            return false;
        }
    }
}