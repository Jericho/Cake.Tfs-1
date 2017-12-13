///////////////////////////////////////////////////////////////////////////////
// REFERENCES
///////////////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine&version=3.6.4"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var sourceDir = "./src";
var buildDir = "./build";

var solutionFile = sourceDir + "/Cake.Tfs.sln";
var nuspecRegex = sourceDir + "/**/*.nuspec";

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() => {
		CleanDirectories(sourceDir + "/**/obj");
		CleanDirectories(sourceDir + "/**/bin");
        CleanDirectories(new []{buildDir});
    });

Task("Restore")
    .Does(() => {
        NuGetRestore(solutionFile);
    });

Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = true,
            WorkingDirectory = sourceDir,
            OutputType = GitVersionOutput.BuildServer
        });
    });

Task("Build")
    .Does(() => {
        DotNetBuild(solutionFile, c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal));
    });

Task("Pack")
    .Does(() => {
		var projectFiles = 
            GetFiles(nuspecRegex)
                .Select(t => t.ChangeExtension("csproj"))
                .Where(FileExists);
        
        NuGetPack(projectFiles, new NuGetPackSettings{
            OutputDirectory = buildDir,
            Version = GitVersion().NuGetVersionV2,
            ArgumentCustomization = t => t.Append("-Prop Configuration=" + configuration)
        });
    });

Task("Default")    
	.IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Version")
    .IsDependentOn("Build")
    .IsDependentOn("Pack");

RunTarget(target);