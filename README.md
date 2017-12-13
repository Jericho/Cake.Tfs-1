# Cake.Tfs

[![Build status](https://ci.appveyor.com/api/projects/status/1nsdhkb7y8wk6e3y/branch/master?svg=true)](https://ci.appveyor.com/project/mabreuortega/cake-tfs/branch/master)

Collection of Cake TFS tools and extensions.

## Cake.Tfs.AutoMerge

### Referencing

Cake.Tfs.AutoMerge is available as a nuget package from the package manager console:

```csharp
Install-Package Cake.Tfs.AutoMerge
```

or directly in your build script via a cake addin directive:

```csharp
#addin "Cake.Tfs.AutoMerge"
```

## Usage

```csharp
#addin "Cake.Tfs.AutoMerge"

Task("Merge")
	.Does(c => {
		CreateAutoMergePullRequest(
			new AutoMergeSettings 
			{
		                // Required
				CollectionUri = "https://{instance}/{collection-name}",
				ProjectName = "project-name",
				RepositoryName = "repository-name",
				SourceBranch = "refs/heads/release/1.0.0",
				TargetBranch = "refs/heads/develop",
				Title = "[Auto Merge from Release]",

                		// Optional
				Description = "Brief description of the changes about to get merge",

                		// Control
				DeleteSourceBranch = false,
				SquashMerge = false,
				OverridePolicies = true,
				AutoComplete = true,
				AutoApprove = true
			});
	});

RunTarget("Merge");
```
