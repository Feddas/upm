# Goal

Miscellaneous generic utility scripts & PropertyDrawers Shawn Featherly likes to use in Unity projects.

# Setup

Reference this package in your Unity project by either:

1. Modifying your projects `Packages/manifest.json` file to include this package subfolder as shown below. ([ref](https://docs.unity3d.com/Manual/upm-git.html#subfolder))
```
"dependencies": {
  "com.feddas.librarysf":"https://github.com/Feddas/upm.git?path=/Assets/LibrarySf",
  ...
}
```

Then Start Unity.

2. Or add `https://github.com/Feddas/upm.git?path=/Assets/LibrarySf` by using the [PackageManager UI](https://docs.unity3d.com/Manual/upm-ui-giturl.html). Optionally, for stability, you can [specify the exact revision using the latest commit hash at the end of the added URL](https://docs.unity3d.com/Manual/upm-git.html#revision).

# Components

Scripts that can be directly added to gameobjects.

### Billboard

Rotates an object to face a camera using 3 different techniques:
- AngleToCamera
- DirectionOfCamera
- InverseCameraForward

### Comment

Component that provides a text area for a comment to be added to a game object.

# Abstract
Scripts that need to be inherited to be used.

### NameOnSerialize

NameOnSerialize allows list data types to be nameable by override ElementName

# PropertyDrawers
Data types with Unity inspector support.

### LoadsScene

Is also a component that exposes method LoadScene() to load the scene file from its `public SceneField Scene;`. LoadsScene contains a SceneField data type that allows scene files to be dragged into fields in the Unity inspector.

### MinMaxRange

PropertyDrawer data type that allows inspector friendly range of values to be restricted to set bounds.

### StringInList

PropertyDrawer attribute that turns a string field into a dropdown list. [Example usage shown in ExampleBehavior.cs](https://gist.github.com/ProGM/9cb9ae1f7c8c2a4bd3873e4df14a6687)

# Repositories

GitHub: https://github.com/Feddas/upm/blob/master/Assets/LibrarySf
<!-- OvrSource: https://phabricator.intern.facebook.com/diffusion/OVRSOURCE/browse/master/Research/audio/lib/unity/Modules/UnityModules/Assets/LibraryShawn
GHE: https://ghe.oculus-rep.com/sfeatherly/LibraryShawn -->
