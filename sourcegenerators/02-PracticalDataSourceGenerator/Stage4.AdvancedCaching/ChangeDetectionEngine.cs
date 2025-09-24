namespace Stage4.AdvancedCaching;

/// <summary>
/// Advanced change detection engine that tracks dependencies and detects changes across multiple dimensions
/// Note: Simplified for source generator constraints - no direct file I/O operations
/// </summary>
internal class ChangeDetectionEngine
{
    private readonly Dictionary<string, int> _contentHashes = [];
    private readonly Dictionary<string, HashSet<string>> _dependencyGraph = [];
    private readonly Dictionary<string, object> _externalDependencies = [];

    /// <summary>
    /// Tracks content for change detection
    /// </summary>
    public void TrackContent(string identifier, string content)
    {
        _contentHashes[identifier] = content.GetHashCode();
    }

    /// <summary>
    /// Adds a dependency relationship between two entities
    /// </summary>
    public void AddDependency(string dependent, string dependency)
    {
        if (!_dependencyGraph.TryGetValue(dependent, out var dependencies))
        {
            dependencies = [];
            _dependencyGraph[dependent] = dependencies;
        }
        dependencies.Add(dependency);
    }

    /// <summary>
    /// Tracks an external dependency (e.g., environment variable, API endpoint)
    /// </summary>
    public void TrackExternalDependency(string key, object value)
    {
        _externalDependencies[key] = value;
    }

    /// <summary>
    /// Detects if content has changed since it was last tracked
    /// </summary>
    public bool HasContentChanged(string identifier, string? currentContent = null)
    {
        if (!_contentHashes.TryGetValue(identifier, out var trackedHash))
            return true; // Not tracked, assume changed

        if (currentContent != null)
        {
            var currentHash = currentContent.GetHashCode();
            return currentHash != trackedHash;
        }

        return false; // No content to compare
    }

    /// <summary>
    /// Detects if an external dependency has changed
    /// </summary>
    public bool HasExternalDependencyChanged(string key, object currentValue)
    {
        if (!_externalDependencies.TryGetValue(key, out var trackedValue))
            return true; // Not tracked, assume changed

        return !Equals(trackedValue, currentValue);
    }

    /// <summary>
    /// Gets all entities that depend on a changed entity (transitively)
    /// </summary>
    public HashSet<string> GetAffectedEntities(string changedEntity)
    {
        var affected = new HashSet<string>();
        var toProcess = new Queue<string>();
        toProcess.Enqueue(changedEntity);

        while (toProcess.Count > 0)
        {
            var current = toProcess.Dequeue();
            
            foreach (var kvp in _dependencyGraph)
            {
                var dependent = kvp.Key;
                var dependencies = kvp.Value;
                if (dependencies.Contains(current) && !affected.Contains(dependent))
                {
                    affected.Add(dependent);
                    toProcess.Enqueue(dependent);
                }
            }
        }

        return affected;
    }

    /// <summary>
    /// Creates a change detection result with detailed information
    /// </summary>
    public ChangeDetectionResult AnalyzeChanges(Dictionary<string, string> currentContent, Dictionary<string, object> currentExternalDependencies)
    {
        var result = new ChangeDetectionResult();

        // Analyze content changes
        foreach (var kvp in currentContent)
        {
            var identifier = kvp.Key;
            var content = kvp.Value;
            if (HasContentChanged(identifier, content))
            {
                result.ChangedFiles.Add(identifier);
                
                // Update tracking
                TrackContent(identifier, content);
                
                // Find affected entities
                var affected = GetAffectedEntities(identifier);
                foreach (var entity in affected)
                {
                    result.AffectedEntities.Add(entity);
                }
            }
        }

        // Analyze external dependency changes
        foreach (var kvp in currentExternalDependencies)
        {
            var key = kvp.Key;
            var value = kvp.Value;
            if (HasExternalDependencyChanged(key, value))
            {
                result.ChangedExternalDependencies.Add(key);
                
                // Update tracking
                TrackExternalDependency(key, value);
                
                // Find affected entities
                var affected = GetAffectedEntities(key);
                foreach (var entity in affected)
                {
                    result.AffectedEntities.Add(entity);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Resets change tracking state
    /// </summary>
    public void Reset()
    {
        _contentHashes.Clear();
        _dependencyGraph.Clear();
        _externalDependencies.Clear();
    }

    /// <summary>
    /// Gets a summary of the current tracking state
    /// </summary>
    public string GetTrackingSummary()
    {
        return $"Tracking {_contentHashes.Count} content items, {_externalDependencies.Count} external dependencies, {_dependencyGraph.Count} dependency relationships";
    }
}

/// <summary>
/// Result of change detection analysis
/// </summary>
internal class ChangeDetectionResult
{
    public HashSet<string> ChangedFiles { get; } = new();
    public HashSet<string> ChangedExternalDependencies { get; } = new();
    public HashSet<string> AffectedEntities { get; } = new();

    public bool HasChanges => ChangedFiles.Count > 0 || ChangedExternalDependencies.Count > 0;

    public string GetSummary()
    {
        return $"Changes detected - Content: {ChangedFiles.Count}, External: {ChangedExternalDependencies.Count}, Affected Entities: {AffectedEntities.Count}";
    }
}