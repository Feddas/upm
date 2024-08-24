# Goal

Use Unity's built-in animator window as a Game State Machine.

# Details

`StateMachineBehaviour`s are a type already supported by the Unity Animator. This AnimatorStateMachine package interops `StateMachineBehaviour`s with scene gameobjects and enable/disable events of game state.

**Disclaimer:** The Unity animator component isn't considered performant. For that reason, it's been shuned for being used as a State Machine. I haven't had any perf problems, So I'm going to keep using it. [Unity 6+ is focused on DOTS alternatives](https://forum.unity.com/threads/dots-animation-options-wiki.1339196/).

# Setup

Reference this package in your Unity project by either:

1. Modifying your projects `Packages/manifest.json` file to include this package subfolder as shown below. ([ref](https://docs.unity3d.com/Manual/upm-git.html#subfolder))
```
"dependencies": {
  "com.feddas.animatorstatemachine": "https://github.com/Feddas/upm.git?path=/Assets/AnimatorStateMachine",
  ...
}
```

Then Start Unity.

2. Or add `https://github.com/Feddas/upm.git?path=/Assets/AnimatorStateMachine` by using the [PackageManager UI](https://docs.unity3d.com/Manual/upm-ui-giturl.html). Optionally, for stability, you can [specify the exact revision using a commit hash at the end of the added URL](https://docs.unity3d.com/Manual/upm-git.html#revision).

# Example

The `Example` folder can be safely deleted. It does not need to be included for this tool to work. That folder is only to demonstrate basic usage.

To view the example, run `Example\ExampleAnimatorStateMachine.unity` in the editor. If you click on states in the Animator window you'll see most states contain a `StateMachineBehaviour` component called `ExampleStateChangesText.cs`. That script is what links a state in the animator to be able to interact with the scene.

# Troubleshooting

Works best when Animator transitions have no "Has Exit Time". Make sure this is unchecked for each transition. Also set `Transition Duration` to 0. This ensures the current state is exited before the next state is entered.

# Reference

This project is based on [a blog post](https://medium.com/the-unity-developers-handbook/dont-re-invent-finite-state-machines-how-to-repurpose-unity-s-animator-7c6c421e5785)
which included [source code](https://github.com/DarrenTsung/DTAnimatorStateMachine) and [example usage](https://github.com/DarrenTsung/laserbeak/blob/master/Assets/Game/States/MainMenuState/MainMenuState.cs). I later came across a [Unite 2015 talk - Applied Mecanim](https://youtu.be/Is9C4i4XyXk) with describes a more complex setup.

# Repo

GitHub: https://github.com/feddas/upm/tree/master/Assets/AnimatorStateMachine/
