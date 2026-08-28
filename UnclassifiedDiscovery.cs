namespace GraphicsSettingsMigrator;

internal static class UnclassifiedDiscovery
{
    public const string CategoryPrefix = "Unclassified profile data";
    private const int MaximumItemsPerProfile = 100;

    private static readonly string[] RiskyNameFragments =
    [
        "cache", "crash", "temp", "log", "logs", "telemetry", "license", "licensing",
        "credential", "session", "sessions", "session storage", "quicksave", "quick save",
        "autosave", "auto save"
    ];

    public static bool IsCategory(string category) =>
        category.StartsWith(CategoryPrefix, StringComparison.OrdinalIgnoreCase);

    public static void AddExisting(List<SettingsLocation> result, Func<string, string> portable)
    {
        var sources = result.Where(x => x.Kind != SourceKind.Registry &&
                                        (Directory.Exists(x.SourcePath) || File.Exists(x.SourcePath)))
            .ToList();
        var allowedRoots = UserProfileRoots();
        var rootedSources = sources.Select(source => new RootedSource(
                source, allowedRoots.FirstOrDefault(root => IsSameOrChild(root, source.SourcePath)) ?? ""))
            .Where(x => x.Root.Length > 0);

        foreach (var group in rootedSources.GroupBy(x => new ProfileKey(
                     x.Source.AppId, x.Source.Product, x.Source.Version, x.Root)))
        {
            var known = group.Select(x => Full(x.Source.SourcePath))
                .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var bases = group.Select(x => x.Source.Kind == SourceKind.File
                    ? Path.GetDirectoryName(x.Source.SourcePath) ?? x.Source.SourcePath
                    : x.Source.SourcePath)
                .Select(Full).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (bases.Count < 2) continue;

            var profileRoot = CommonDirectory(bases);
            if (string.IsNullOrWhiteSpace(profileRoot) || SamePath(profileRoot, group.Key.Root) ||
                known.Any(path => Directory.Exists(path) && SamePath(path, profileRoot)))
                continue;

            var count = 0;
            AddUnknownChildren(result, portable, group.Key.AppId, group.Key.Product, group.Key.Version,
                profileRoot, profileRoot, known, ref count, 0);
        }
    }

    private static void AddUnknownChildren(List<SettingsLocation> result, Func<string, string> portable,
        string appId, string product, string version, string profileRoot, string current,
        IReadOnlyCollection<string> known, ref int count, int depth)
    {
        if (count >= MaximumItemsPerProfile || depth > 8) return;
        foreach (var path in SafeEntries(current))
        {
            if (count >= MaximumItemsPerProfile) return;
            var full = Full(path);
            if (IsRiskyName(Path.GetFileName(full)) || IsReparsePoint(full)) continue;
            if (known.Any(knownPath => IsSameOrChild(knownPath, full))) continue;

            var containsKnownPath = Directory.Exists(full) &&
                                    known.Any(knownPath => IsSameOrChild(full, knownPath));
            if (containsKnownPath)
            {
                AddUnknownChildren(result, portable, appId, product, version, profileRoot, full,
                    known, ref count, depth + 1);
                continue;
            }

            var relative = Path.GetRelativePath(profileRoot, full);
            var exclusions = Directory.Exists(full) ? FindExcludedPrefixes(full) : [];
            var (files, bytes) = Measure(full, exclusions);
            if (files == 0) continue;
            result.Add(new SettingsLocation
            {
                AppId = appId,
                Product = product,
                Version = version,
                Category = CategoryPrefix + " — " + relative,
                Kind = Directory.Exists(full) ? SourceKind.Directory : SourceKind.File,
                SourcePath = full,
                PortablePath = portable(full),
                Recommended = false,
                Notes = "Experimental unclassified item. Review its contents before saving or restoring; " +
                        "manual selection only.",
                FileCount = files,
                SizeBytes = bytes,
                ExcludedPrefixes = exclusions
            });
            count++;
        }
    }

    private static List<string> FindExcludedPrefixes(string root)
    {
        var excluded = new List<string>();
        var pending = new Stack<string>();
        pending.Push(root);
        while (pending.Count > 0)
        {
            var current = pending.Pop();
            foreach (var entry in SafeEntries(current))
            {
                var relative = Path.GetRelativePath(root, entry);
                if (IsRiskyName(Path.GetFileName(entry)) || IsReparsePoint(entry))
                {
                    excluded.Add(relative);
                    continue;
                }
                if (Directory.Exists(entry)) pending.Push(entry);
            }
        }
        return excluded;
    }

    private static (int Files, long Bytes) Measure(string path, IReadOnlyCollection<string> exclusions)
    {
        if (File.Exists(path))
        {
            try { return (1, new FileInfo(path).Length); }
            catch { return (0, 0); }
        }

        var files = 0;
        long bytes = 0;
        try
        {
            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(path, file);
                if (DiscoveryService.IsExcluded(relative, exclusions)) continue;
                try
                {
                    files++;
                    bytes += new FileInfo(file).Length;
                }
                catch { }
            }
        }
        catch { }
        return (files, bytes);
    }

    private static string CommonDirectory(IReadOnlyList<string> paths)
    {
        var common = paths[0];
        while (paths.Any(path => !IsSameOrChild(common, path)))
        {
            common = Directory.GetParent(common)?.FullName ?? "";
            if (common.Length == 0) return "";
        }
        return common;
    }

    private static List<string> UserProfileRoots()
    {
        return new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            }
            .Where(x => !string.IsNullOrWhiteSpace(x)).Select(Full)
            .Distinct(StringComparer.OrdinalIgnoreCase).OrderByDescending(x => x.Length).ToList();
    }

    private static IEnumerable<string> SafeEntries(string path)
    {
        try { return Directory.Exists(path) ? Directory.EnumerateFileSystemEntries(path).ToArray() : []; }
        catch { return []; }
    }

    private static bool IsRiskyName(string name)
    {
        var normalized = name.Trim().ToLowerInvariant();
        if (normalized.EndsWith(".log") || normalized.EndsWith(".tmp")) return true;
        return RiskyNameFragments.Any(fragment => normalized.Equals(fragment) ||
                                                  normalized.StartsWith(fragment + ".") ||
                                                  normalized.StartsWith(fragment + "_") ||
                                                  normalized.EndsWith(fragment));
    }

    private static bool IsReparsePoint(string path)
    {
        try { return (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0; }
        catch { return true; }
    }

    private static string Full(string path) =>
        Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    private static bool SamePath(string left, string right) =>
        Full(left).Equals(Full(right), StringComparison.OrdinalIgnoreCase);

    private static bool IsSameOrChild(string parent, string candidate)
    {
        var root = Full(parent);
        var path = Full(candidate);
        return path.Equals(root, StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record RootedSource(SettingsLocation Source, string Root);
    private sealed record ProfileKey(string AppId, string Product, string Version, string Root);
}
