using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleAnimatorStateMachine : StateMachineGameObject<ExampleAnimatorStateMachine>
{
    public UnityEngine.UI.Text Console = null;

    void Update()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            animator.SetTrigger("Next");
        }
    }
}
