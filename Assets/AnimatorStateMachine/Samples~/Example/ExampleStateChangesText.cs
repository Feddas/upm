using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleStateChangesText : GameStateBehaviour<ExampleAnimatorStateMachine>
{
    [SerializeField]
    private string UiText = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override protected void OnStateEntered()
    {
        stateMachine.Console.text = UiText;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdated() { }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override protected void OnStateExited()
    {
        if (stateMachine.Console != null)
        {
            stateMachine.Console.text = "Exited state #" + currentNameHash + ".";
        }
    }
}
