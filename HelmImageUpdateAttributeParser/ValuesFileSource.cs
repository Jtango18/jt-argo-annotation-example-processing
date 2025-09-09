namespace HelmImageUpdateAttributeParser;

public record ValuesFileSource(Uri Repo, string Revision, string Path, string FileName, string DefinedImagePaths);
