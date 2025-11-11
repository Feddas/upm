using UnityEngine;

/// <summary>
/// This script defines WHAT gameobjects the state (a state in an Animator) can manipulate.
/// This script is the bridge between the Animator and gameobjects in the scene.
/// </summary>
public class ExampleStateBridgeToText : StateMachineGameObject<ExampleStateBridgeToText>
{
    public UnityEngine.UI.Text Console = null;

    void Update()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            animator.SetTrigger("Next"); // Tell the Animator to try to go to a new state.
        }
    }
}
