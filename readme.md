# Goal

Unity project serving as a git subfolders based npm registry for personal UPM packages.

# Usage

These packages can be referenced in a Unity project by modifying [package.json directly](https://docs.unity3d.com/Manual/upm-git.html#subfolder) or with the [PackageManager UI](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

# Package List

## [Animator State Machine](Assets/AnimatorStateMachine/)

Hooks into a Unity Animator to display and transition between game states.

## [Library SF](Assets/LibrarySf/)

Miscellaneous generic utility scripts & PropertyDrawers Shawn Featherly likes to use in Unity projects.

## [ScriptableObject architecture](https://github.com/Feddas/SoArchitecture)

Adds payload events to Ryan Hippies Unite talk.

## [Versioning](Assets/VersionIt/)

Syncs the Unity build settings project version outside of the UnityEditor.

# Development

Use a [symbolic link](https://forum.unity.com/threads/samples-in-packages-manual-setup.623080/page-2#post-9852978) to modify samples inside of the Unity editor:
1. Navigate a cmd prompt inside the [Assets folder](Assets)
2. run `mklink /J AnimatorStateMachine_Samples AnimatorStateMachine\Samples~`
<!-- another option is with github actions https://medium.com/openupm/how-to-maintain-upm-package-part-1-7b4daf88d4c4#236a -->

If you create a new sample, [modify package.json](https://docs.unity3d.com/2021.2/Documentation/Manual/cus-samples.html).

<!-- TODO: Toggle UI, Image Crop -->

# Repo

local project name: feddas_upm

GitHub: https://github.com/Feddas/upm
