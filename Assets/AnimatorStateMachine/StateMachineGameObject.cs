using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows an Animator GameStateBehaviour to interop with
/// GameObjects in the scene through its stateMachine variable
/// </summary>
/// <typeparam name="TStateInterop"> Conduit to pass full signature of this class to GameStateBehaviours.
/// Should always be used as `YourClassName : StateMachineGameObject<YourClassName>` </typeparam>
[RequireComponent(typeof(Animator))]
public abstract class StateMachineGameObject<TStateInterop> : MonoBehaviour
    where TStateInterop : MonoBehaviour
{
    protected Animator animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = this.GetComponent<Animator>();
            }
            return _animator;
        }
    }
    private Animator _animator;

    protected virtual void Awake()
    {
        this.ConfigureAllStateBehaviours();
    }

    protected virtual void OnEnable()
    {
        this.EnableAllStateBehaviours();
    }

    protected virtual void OnDisable()
    {
        this.DisableAllStateBehaviours();
    }

    public static IEnumerable<GameStateBehaviour<TStateInterop>> GetStateBehaviours(Animator animator)
    {
        StateMachineBehaviour[] behaviours = animator.GetBehaviours<StateMachineBehaviour>();
        foreach (StateMachineBehaviour behaviour in behaviours)
        {
            GameStateBehaviour<TStateInterop> stateBehaviour = behaviour as GameStateBehaviour<TStateInterop>;
            if (stateBehaviour == null)
            {
                continue;
            }

            yield return stateBehaviour;
        }
    }

    public void ConfigureAllStateBehaviours()
    {
        foreach (GameStateBehaviour<TStateInterop> behaviour in GetStateBehaviours(animator))
        {
            behaviour.InitializeWithContext(animator, this as TStateInterop);
        }
    }

    public void DisableAllStateBehaviours()
    {
        foreach (GameStateBehaviour<TStateInterop> behaviour in GetStateBehaviours(animator))
        {
            behaviour.Disable();
        }
    }

    public void EnableAllStateBehaviours()
    {
        foreach (GameStateBehaviour<TStateInterop> behaviour in GetStateBehaviours(animator))
        {
            behaviour.Enable();
        }
    }
}
