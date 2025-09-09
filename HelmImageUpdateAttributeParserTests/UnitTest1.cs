using System.Text.Json;
using HelmImageUpdateAttributeParser;
using HelmImageUpdateAttributeParser.Existing.Domain;

namespace HelmImageUpdateAttributeParserTests;



public class UnitTest1
{
    private const string BasePath = "./jsonFiles/";
    
    [Fact]
    public async Task HelmInlineValuesFile()
    {
        var app = await ReadFromFile("inline-values-file.json");

        var sut = new ValuesFilesParser(app);

        var files = sut.GetValuesFiles();
        
        Assert.Single(files);
        var file = files[0];
        Assert.Equal("https://github.com/my-repo/my-argo-app", file.Repo.ToString());
        Assert.Equal("Chart", file.Path);
        Assert.Equal("main", file.Revision);
        Assert.Equal("values.yaml", file.FileName);

    }

    [Fact]
    public async Task HelmInlineValuesFiles()
    {
        var app = await ReadFromFile("helm-inline-values-files.json");
        
        var sut = new ValuesFilesParser(app);

        // Act
        var files = sut.GetValuesFiles();
        
        Assert.Equal(2, files.Count);
        var source1 = files[0];
        var source2 = files[1];
        
        Assert.Equal("https://github.com/my-repo/my-argo-app", source1.Repo.ToString());
        Assert.Equal("./", source1.Path);
        Assert.Equal("main", source1.Revision);
        Assert.Equal("app-files/values.yaml", source1.FileName);
        
        Assert.Equal("https://github.com/my-repo/my-argo-app", source2.Repo.ToString());
        Assert.Equal("./", source2.Path);
        Assert.Equal("main", source2.Revision);
        Assert.Equal("app-files/values-overlay.yaml", source2.FileName);
    }

    [Fact]
    public async Task MixedSourceValuesFiles()
    {
        var app = await ReadFromFile("mixed-source-values-files.json");
        
        var sut = new ValuesFilesParser(app);

        // Act
        var files = sut.GetValuesFiles();;
        
        Assert.Equal(2, files.Count);
        var source1 = files[0];
        var source2 = files[1];
        
        Assert.Equal("https://github.com/my-repo/my-argo-app", source1.Repo.ToString());
        Assert.Equal("./", source1.Path);
        Assert.Equal("main", source1.Revision);
        Assert.Equal("app-files/values.yaml", source1.FileName);
        Assert.Equal("{{ .Values.image.name}}:{{ .Values.image.version}}, {{ .Values.another-image.name }}", source1.DefinedImagePaths);
        
        Assert.Equal("https://github.com/another-repo/values-files-here", source2.Repo.ToString());
        Assert.Equal("./", source2.Path);
        Assert.Equal("main", source2.Revision);
        Assert.Equal("values.yaml", source2.FileName);
        Assert.Equal("{{ .Values.different.structure.here.image }}", source2.DefinedImagePaths);
    }
    
    [Fact]
    public async Task PathSeparatedValuesFiles()
    {
        var app = await ReadFromFile("path-separated-values-files.json");
        
        var sut = new ValuesFilesParser(app);

        // Act
        var files = sut.GetValuesFiles();
        
        Assert.Equal(2, files.Count);
        var source1 = files[0];
        var source2 = files[1];
        
        Assert.Equal("https://github.com/another-repo/shared-values-files-here", source1.Repo.ToString());
        Assert.Equal("./", source1.Path);
        Assert.Equal("main", source1.Revision);
        Assert.Equal("some-path/values.yaml", source1.FileName);
        Assert.Equal("{{ .Values.image.name}}:{{ .Values.image.version}}, {{ .Values.another-image.name }}", source1.DefinedImagePaths);
        //
        Assert.Equal("https://github.com/another-repo/shared-values-files-here", source2.Repo.ToString());
        Assert.Equal("./", source2.Path);
        Assert.Equal("main", source2.Revision);
        Assert.Equal("another-path/values.yaml", source2.FileName);
        Assert.Equal("{{ .Values.different.structure.here.image }}", source2.DefinedImagePaths);
    }

    [Fact]
    public async Task MultipleHelmSources()
    {
        var app = await ReadFromFile("multiple-helm-sources.json");
        var sut = new ValuesFilesParser(app);

        // Act
        var files = sut.GetValuesFiles();
        
        // Assert
        Assert.Equal(2, files.Count);
        var source1 = files[0];
        var source2 = files[1];
        
        Assert.Equal("https://github.com/my-repo/my-argo-app", source1.Repo.ToString());
        Assert.Equal("./", source1.Path);
        Assert.Equal("main", source1.Revision);
        Assert.Equal("values.yaml", source1.FileName);
        Assert.Equal("{{ .Values.image.name}}:{{ .Values.image.version}}, {{ .Values.another-image.name }}", source1.DefinedImagePaths);
        
        Assert.Equal("https://github.com/my-repo/my-other-argo-app", source2.Repo.ToString());
        Assert.Equal("cool", source2.Path);
        Assert.Equal("main", source2.Revision);
        Assert.Equal("values.yaml", source2.FileName);
        Assert.Equal("{{ .Values.different.structure.here.image }}", source2.DefinedImagePaths);

        
    }

    [Fact]
    public async Task SingleRefNoAlias()
    {
        var app = await ReadFromFile("single-ref-no-alias.json");
        var sut = new ValuesFilesParser(app);
        
        var files = sut.GetValuesFiles();
        
        Assert.Single(files);
        var source1 = files[0];
        
        Assert.Equal("https://github.com/another-repo/values-files-here", source1.Repo.ToString());
        Assert.Equal("./", source1.Path);
        Assert.Equal("main", source1.Revision);
        Assert.Equal("values.yaml", source1.FileName);
        Assert.Equal("{{ .Values.image.name}}:{{ .Values.image.version}}, {{ .Values.another-image.name }}", source1.DefinedImagePaths);
        
    }

    [Fact]
    public async Task SingleRefWithAlias()
    {
        var app = await ReadFromFile("single-ref-with-alias.json");
        var sut = new ValuesFilesParser(app);
        
        var files = sut.GetValuesFiles();
        Assert.Single(files);
        var source1 = files[0];
        
        Assert.Equal("https://github.com/another-repo/values-files-here", source1.Repo.ToString());
        Assert.Equal("./", source1.Path);
        Assert.Equal("main", source1.Revision);
        Assert.Equal("values.yaml", source1.FileName);
        Assert.Equal("{{ .Values.image.name}}:{{ .Values.image.version}}, {{ .Values.another-image.name }}", source1.DefinedImagePaths);
    }

    [Fact]
    public async Task MultiRefNoAlias()
    {
        var app = await ReadFromFile("multi-ref-no-alias.json");
        var sut = new ValuesFilesParser(app);
        
        var files = sut.GetValuesFiles();
        
        Assert.Equal(2, files.Count);
        var source1 = files[0];
        var source2 = files[1];
        
        Assert.Equal("https://github.com/main-repo/values-files-here", source1.Repo.ToString());
        Assert.Equal("./", source1.Path);
        Assert.Equal("main", source1.Revision);
        Assert.Equal("values.yaml", source1.FileName);
        Assert.Equal("{{ .Values.another-image.name }}", source1.DefinedImagePaths);
        
        Assert.Equal("https://github.com/another-repo/values-files-here", source2.Repo.ToString());
        Assert.Equal("./", source2.Path);
        Assert.Equal("main", source2.Revision);
        Assert.Equal("values.yaml", source2.FileName);
        Assert.Equal("{{ .Values.image.name}}:{{ .Values.image.version}}, {{ .Values.another-image.name }}", source2.DefinedImagePaths);
        
    }

    [Fact]
    public async Task MultiRefWithAlias()
    {
        var app = await ReadFromFile("multi-ref-with-alias.json");
        var sut = new ValuesFilesParser(app);
        
        var files = sut.GetValuesFiles();
        
        Assert.Equal(2, files.Count);
        var source1 = files[0];
        var source2 = files[1];
        
        Assert.Equal("https://github.com/another-repo/values-files-here", source1.Repo.ToString());
        Assert.Equal("./", source1.Path);
        Assert.Equal("main", source1.Revision);
        Assert.Equal("values.yaml", source1.FileName);
        Assert.Equal("{{ .Values.image.name}}:{{ .Values.image.version}}, {{ .Values.another-image.name }}", source1.DefinedImagePaths);
        
        Assert.Equal("https://github.com/main-repo/values-files-here", source2.Repo.ToString());
        Assert.Equal("./", source2.Path);
        Assert.Equal("main", source2.Revision);
        Assert.Equal("values.yaml", source2.FileName);
        Assert.Equal("{{ .Values.another-image.name }}", source2.DefinedImagePaths);
    }


    async Task<Application> ReadFromFile(string fileName)
    {
        var filePath = $"{BasePath}{fileName}";
        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<Application>(json)!;
    }

}