using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Extends an Animator StateMachineBehaviour to support
/// interacting with gameobject through a StateMachine variable
/// </summary>
/// <typeparam name="TStateInterop"> The type of interop to be loaded.
/// If no game objects need to be access by the animator, use GameStateBehaviour`MonoBehaviour` </typeparam>
public class GameStateBehaviour<TStateInterop> : StateMachineBehaviour
{
    #region StateMachineBehaviour Lifecycle
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentNameHash = stateInfo.fullPathHash;
        if (active || !enabled)
        {
            return;
        }

        OnStateEntered();
        active = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!active || !enabled)
        {
            return;
        }

        active = false;
        OnStateExited();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!enabled)
        {
            return;
        }

        OnStateUpdated();
    }

    public void InitializeWithContext(Animator animator, TStateInterop stateMachine)
    {
        this.stateMachine = stateMachine;
        OnInitialized();
    }

    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
    {
        DisableAndExit();
    }
    #endregion StateMachineBehaviour Lifecycle

    #region Internal
    protected TStateInterop stateMachine { get; private set; }
    protected int currentNameHash { get; private set; }

    private bool active = false;
    private bool enabled = true;

    private void OnDisable()
    {
        DisableAndExit();
    }

    private void DisableAndExit()
    {
        if (!enabled)
        {
            return;
        }

        if (active)
        {
            OnStateExited();
            active = false;
        }
        enabled = false;
    }

    protected virtual void OnInitialized()
    {
        // stub
    }

    protected virtual void OnStateEntered()
    {
        // stub
    }

    protected virtual void OnStateExited()
    {
        // stub
    }

    protected virtual void OnStateUpdated()
    {
        // stub
    }
    #endregion Internal
}
