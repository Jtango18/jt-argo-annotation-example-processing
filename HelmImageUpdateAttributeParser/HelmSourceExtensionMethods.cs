using HelmImageUpdateAttributeParser.Existing.Domain;

namespace HelmImageUpdateAttributeParser;

public static class HelmSourceExtensionMethods
{
    public static string GenerateInlineValuesFileAbsolutePath(this HelmSource source, string fileName)
    {
        var baseUrl = source.RepoUrl.ToString().EndsWith('/') ? source.RepoUrl.ToString() : $"{source.RepoUrl}/";

        var repoPath = (source.Path.StartsWith("./") ? source.Path[2..] : source.Path).TrimEnd('/');
        if (!string.IsNullOrEmpty(repoPath))
        {
            repoPath = $"{repoPath}/";
        }
        var file = fileName.TrimStart('/');

        return $"{baseUrl}{source.TargetRevision}/{repoPath}{file}";
    }
}