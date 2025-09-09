using HelmImageUpdateAttributeParser.Existing;
using HelmImageUpdateAttributeParser.Existing.Domain;

namespace HelmImageUpdateAttributeParser;

public class ValuesFilesParser(Application app)
{
    private readonly List<KeyValuePair<string, string>> aliasAnnotations =
        app.Metadata.Annotations.Where(a => a.Key.StartsWith(ArgoCDConstants.ImageReplaceAliasKey)).ToList();

    private readonly List<KeyValuePair<string, string>> imageReplacePathAnnotations = app.Metadata.Annotations
        .Where(a => a.Key.StartsWith($"{ArgoCDConstants.ImageReplacePathsKey}")).ToList();

    private readonly List<ReferenceSource> refSources = app.Spec.Sources.OfType<ReferenceSource>().ToList();
    private readonly List<HelmSource> helmSources = app.Spec.Sources.OfType<HelmSource>().ToList();

    public List<ValuesFileSource> GetValuesFiles()
    {
        var toReturn = new List<ValuesFileSource>();
        if (helmSources.Count > 0)
        {
            foreach (var source in helmSources.Where(h => h.Helm.ValueFiles.Count > 0))
            {
                var results = ExtractValuesFilesForSource(source);
                toReturn.AddRange(results);
            }
        }
        return toReturn;
    }

    private List<ValuesFileSource> ExtractValuesFilesForSource(HelmSource source)
    {
        return source.Helm.ValueFiles.Select(file => file.StartsWith('$')
                ? ProcessRefValuesFile(file)
                : ProcessInlineValuesFile(source, file))
            .OfType<ValuesFileSource>()
            .ToList();
    }

    private ValuesFileSource? ProcessRefValuesFile(string file)
    {
        var refName = GetRefFromFilePath(file);

        var imageReplacementPathKeySpecifier = refName;
        var aliasWithRefValueKey = aliasAnnotations.FirstOrDefault(a => a.Value == file).Key;
        if (aliasWithRefValueKey != null)
        {
            // We have an alias against the value file path.
            imageReplacementPathKeySpecifier = GetSpecifierFromKey(aliasWithRefValueKey);
        }

        var imageReplacementPathsForRef = imageReplacePathAnnotations
            .FirstOrDefault(a => a.Key == $"{ArgoCDConstants.ImageReplacePathsKey}.{imageReplacementPathKeySpecifier}")
            .Value;
        if (!string.IsNullOrEmpty(imageReplacementPathsForRef))
        {
            var refForValuesFile = refSources.FirstOrDefault(r => r.Ref == refName);
            if (refForValuesFile != null)
            {
                var relativeFile = file.Substring(file.IndexOf('/') + 1);
                // Ref has image paths specified
               return new ValuesFileSource(refForValuesFile.RepoUrl, refForValuesFile.TargetRevision, "./",
                    relativeFile, imageReplacementPathsForRef);
            }

            // Something is misconfigured - exception or ignore?
            throw new Exception();
        }

        return null;
    }

    private ValuesFileSource? ProcessInlineValuesFile(HelmSource source, string file)
    {
        // Check for a default path annotation - if we find it, we can can assume a single values file
        var definedPathsForSource = imageReplacePathAnnotations
            .FirstOrDefault(a => a.Key == $"{ArgoCDConstants.ImageReplacePathsKey}").Value;
        if (definedPathsForSource != null)
        {
            return new ValuesFileSource(source.RepoUrl, source.TargetRevision, source.Path, file,
                definedPathsForSource);
        }

        // Check if there is an alias to just the file name,
        var aliasKeyForFile = aliasAnnotations.FirstOrDefault(a => a.Value == file).Key;
        string? alias = null;
        if (aliasKeyForFile != null)
        {
            alias = GetSpecifierFromKey(aliasKeyForFile);
        }
        else
        {
            // Check if there is a fully qualified alias
            var qualifiedAliasValue = source.GenerateInlineValuesFileAbsolutePath(file);
            var aliasKeyForSource = aliasAnnotations.FirstOrDefault(a => a.Value == qualifiedAliasValue).Key;
            if (aliasKeyForSource != null)
            {
                alias = GetSpecifierFromKey(aliasKeyForSource);
            }
        }

        if (alias != null)
        {
            var definedPathsForAlias = GetReplacementPathsForAlias(alias);
            if (definedPathsForAlias != null)
            {
                return new ValuesFileSource(source.RepoUrl, source.TargetRevision, source.Path, file,
                    definedPathsForAlias);
            }
        }

        return null;
    }

    private string? GetReplacementPathsForAlias(string alias)
    {
        return imageReplacePathAnnotations
            .FirstOrDefault(a => a.Key == $"{ArgoCDConstants.ImageReplacePathsKey}.{alias}").Value;
    }

    private static string GetSpecifierFromKey(string key)
    {
        return key[(key.LastIndexOf('.') + 1)..];
    }

    private static string GetRefFromFilePath(string filePath)
    {
        return filePath.TrimStart('$')[..(filePath.IndexOf('/') - 1)];
    }
}