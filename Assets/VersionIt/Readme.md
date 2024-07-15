# Goal

- Standardize how semantic versioning is tracked in Unity projects
- Support a single project being able to version multiple packages

# Details

Uses [semantic versioning](https://semver.org) to version projects. Version numbers are in x.y.z format. Where x, y, and z are always a positive integer. for example, 1.2.3. This format makes the version numbers semantic versioning compliant.

VersionIt has a button to increment only the z section of the verison number. The x & y values need to be manually incremented.

# Setup

### Installation by Git

These steps require Unity 2018.3.0 or greater. The steps below are not tested but should work according to https://docs.unity3d.com/Manual/upm-git.html <!-- was https://forum.unity.com/threads/git-support-on-package-manager.573673/#post-3819487 -->

1. Modify your projects Packages/manifest.json to include this package.

```
{
  "dependencies": {
    "com.feddas.versionit": "https://github.com/Feddas/upm.git?path=/Assets/VersionIt",
    ...
  }
}
```

2. Start Unity

### Getting started

1. Using the Unity drop-down menu "Assets => Create => Feddas => Version It" create a new versionAsset to be a part of your project, but outside of the package folder itself. This versionAsset is meant to manage the package.json which is part of the project. versionAsset is not meant to be a part of the project itself.
2. Rename the created VersionNameOfProject.asset to Version[ Name Of Your Project ].asset
3. Make sure the new versionAsset is selected and being shown in the unity inspector
4. If you don't already have a package.json for the project, click "New Package Json" and move the created package.json inside your package folder. Otherwise, add the package.json to the PackageJsonFile field that you want to maintain

### Version updates

Whenever you make a new build that will be shared or otherwise accessible to anyone other then yourself, you need to update the version.

1. Make sure the new versionAsset is selected and being shown in the unity inspector
2. Click the "Increment Patch Version" button

# Buttons

The following buttons are visible in the Unity inspector when a versionAsset is selected

| Button | Description |
|-|-|
| Save Package Info | Saves the values in the Unity inspector to package.json. Also saves to build settings if IsSavingToBuildSettings is checked. |
| New Package Json | Creates a new package.json using first the values in PackageInfo, if they're blank, uses values from build settings. |
| Load From Json File | re/loads the values from the package.json file referenced in the PackageJsonFile field. |
| Load Build Settings | loads the values from the projects build settings. |
| Increment Patch Version | Increments the last value of the version. Causing 0.0.0 to become 0.0.1 |

# Validation tests

1. Test StreamingAssets VersionData.json
    1. Delete StreamingAssets folder
    2. Create "Assets => Create => Feddas => Version It"
    3. Click the "New Package Json" button
    4. Ensure "Is Saving To Streaming Asset" is checked
    5. Click the "Save Package Info" button
    6. **Expected**: newly created StreamingAssets/VersionData.json to match info inside of created VersionIt.asset

# Repositories

GitHub: https://github.com/Feddas/upm/blob/master/Assets/VersionIt/