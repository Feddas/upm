# Goal

Quickly test cropping an image without modifying that image. Allows other image references to retain the uncropped image.

# Details

CropImageAtRuntime will crop the image only at runtime. This ensures other references to an image won't be impacted by cropping.

# Installation

Reference this package in your Unity project by either:

1. Modifying your projects `Packages/manifest.json` file to include this package subfolder as shown below. ([ref](https://docs.unity3d.com/Manual/upm-git.html#subfolder))
```
"dependencies": {
  "com.feddas.cropimage": "https://github.com/Feddas/upm.git?path=/Assets/CropImage",
  ...
}
```

Then Start Unity.

2. Or add `https://github.com/Feddas/upm.git?path=/Assets/CropImage` by using the [PackageManager UI](https://docs.unity3d.com/Manual/upm-ui-giturl.html). Optionally, for stability, you can [specify the exact revision using a commit hash at the end of the added URL](https://docs.unity3d.com/Manual/upm-git.html#revision).

# Usage

- `CropImageAtRuntime.cs` - Core component. Add to any image component, then modify the `cropArea` to change the bounds of a rectangular crop. This will affect the image shown by the image component.
- `CropImagePreload.cs` - Offers a performance boost at the cost of the complexity of tracking a library of cropped images. AKA, if you find yourself using this you should consider going through all references to the image and seeing if they can be updated to consistently display the image.
- `CropImageTraits.cs` - Allows binding `cropArea` to multiple components. Changing one `cropArea` value will update all components using the trait.

# Repo

GitHub: https://github.com/feddas/upm/tree/master/Assets/CropImage/
