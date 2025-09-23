namespace ExtensionBlocks.Models;

/// <summary>
/// C# 14 Extension Block for IEnumerable&lt;T&gt; demonstrating collection extension methods.
/// Shows how extension methods can enhance collection operations.
/// 
/// Uses the actual C# 14 extension syntax that compiles and runs in .NET 10 RC 1.
/// This supports generic type parameters and provides better organization for
/// related collection operations.
/// </summary>
public static class CollectionExtensions
{
    extension<T> (IEnumerable<T> source)
    {
        /// <summary>
        /// Extension method that checks if the collection is null or empty.
        /// Utility method for safe collection checking.
        /// </summary>
        public bool IsNullOrEmpty()
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Extension method that returns a random element from the collection.
        /// Demonstrates random selection with proper null checking.
        /// </summary>
        public T? GetRandomElement()
        {
            if (source.IsNullOrEmpty())
                return default;

            List<T> list = [.. source];
            return list[Random.Shared.Next(list.Count)];
        }

        /// <summary>
        /// Extension method that returns multiple random elements from the collection.
        /// More complex random selection with count parameter.
        /// </summary>
        public IEnumerable<T> GetRandomElements(int count)
        {
            if (source.IsNullOrEmpty() || count <= 0)
                return [];

            List<T> list = [.. source];
            
            return Enumerable.Range(0, Math.Min(count, list.Count))
                            .Select(_ => list[Random.Shared.Next(list.Count)])
                            .Distinct();
        }

        /// <summary>
        /// Extension method that chunks the collection into batches of specified size.
        /// Useful for processing large collections in smaller batches.
        /// </summary>
        public IEnumerable<IEnumerable<T>> ChunkBy(int chunkSize)
        {
            if (chunkSize <= 0)
                throw new ArgumentException("Chunk size must be positive", nameof(chunkSize));

            if (source.IsNullOrEmpty())
                yield break;

            List<T> chunk = new(capacity: chunkSize);
            
            foreach (var item in source)
            {
                chunk.Add(item);
                
                if (chunk.Count == chunkSize)
                {
                    yield return chunk;
                    chunk = new(capacity: chunkSize);
                }
            }
            
            if (chunk.Count > 0)
                yield return chunk;
        }
    }
}
