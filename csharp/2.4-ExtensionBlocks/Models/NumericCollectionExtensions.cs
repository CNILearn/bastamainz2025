namespace ExtensionBlocks.Models;

/// <summary>
/// C# 14 Extension Block for IEnumerable&lt;int&gt; demonstrating numeric collection extensions.
/// Specialized extension methods for numeric collections.
/// 
/// Uses the actual C# 14 extension syntax that compiles and runs in .NET 10 RC 1.
/// </summary>
public static class NumericCollectionExtensions
{
    extension (IEnumerable<int> source)
    {
        /// <summary>
        /// Extension method that calculates the median value of the collection.
        /// Statistical calculation extension method for numeric collections.
        /// </summary>
        public double Median()
        {
            if (source.IsNullOrEmpty())
                return 0;

            List<int> sorted = [.. source.OrderBy(x => x)];
            var count = sorted.Count;
            
            if (count % 2 == 0)
            {
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
            }
            else
            {
                return sorted[count / 2];
            }
        }

        /// <summary>
        /// Extension method that finds the mode (most frequent value) in the collection.
        /// More complex statistical calculation with frequency analysis.
        /// </summary>
        public int Mode()
        {
            if (source.IsNullOrEmpty())
                return 0;

            return source.GroupBy(x => x)
                      .OrderByDescending(g => g.Count())
                      .First()
                      .Key;
        }

        /// <summary>
        /// Extension method that calculates the standard deviation of the collection.
        /// Advanced statistical calculation demonstrating mathematical operations.
        /// </summary>
        public double StandardDeviation()
        {
            if (source.IsNullOrEmpty())
                return 0;

            var mean = source.Average();
            var sumOfSquaredDifferences = source.Sum(x => Math.Pow(x - mean, 2));
            return Math.Sqrt(sumOfSquaredDifferences / source.Count());
        }

        /// <summary>
        /// Extension method that returns statistical summary of the collection.
        /// Combines multiple statistical calculations into a single result.
        /// </summary>
        public (int Count, double Mean, double Median, int Mode, double StdDev, int Min, int Max) GetStatistics()
        {
            if (source.IsNullOrEmpty())
                return (0, 0, 0, 0, 0, 0, 0);

            return (
                Count: source.Count(),
                Mean: source.Average(),
                Median: source.Median(),
                Mode: source.Mode(),
                StdDev: source.StandardDeviation(),
                Min: source.Min(),
                Max: source.Max()
            );
        }
    }
}