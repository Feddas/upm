using UnityEngine;

/// <summary>
/// This script defines HOW the state (a state in an Animator) will manipulate gameobjects during the lifecyle of this state.
/// </summary>
public class ExampleStateChangesText : StateMachineAnimatorState<ExampleStateBridgeToText>
{
    [SerializeField]
    private string UiText = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override protected void OnStateEntered()
    {
        // manipulate gameobject
        stateMachine.Console.text = UiText;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdated() { }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override protected void OnStateExited()
    {
        if (stateMachine.Console != null) // skip if Application.Quit or OnDestroy
        {
            // Record telemetry event
            Debug.Log(Time.realtimeSinceStartup + " Exited state #" + currentNameHash);
        }
    }
}
