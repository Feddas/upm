using System;
using System.Collections.Generic;
using UnityEngine;

namespace Feddas
{
    /// <summary>
    /// A set of GameObject visuals to be toggled on for one TState in a dictionary of TStates.
    /// </summary>
    /// <typeparam name="TStates"> Available states. </typeparam>
    [Serializable]
    public class VisualState<TStates>
        : ISerializationCallbackReceiver
        where TStates : Enum
    {
        /// <summary> Names this element when it is used in an array. </summary>
        [HideInInspector]
        [SerializeField]
        private string name;

        [HideInInspector]
        [Tooltip("Visual state where these gameobjects will be active.")]
        [SerializeField]
        private TStates activeOn;

        [Tooltip("Gameobjects that should be active for a given state. Then will be set inactive for other states. Do not include gameobjects that are active for all states.")]
        [SerializeField]
        private List<GameObject> activeGameObjects = new List<GameObject>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualState{TStates}"/> class.
        /// This instance will be linked to a single visual state type, <paramref name="activeOn"/>. </summary>
        /// <param name="activeOn"> How this visual state will be indexed. </param>
        public VisualState(TStates activeOn)
        {
            this.activeOn = activeOn;
        }

        /// <summary> A unique identifer for the instance of this state. </summary>
        public TStates ActiveOn => activeOn;

        /// <summary> Gameobjects to activate for a given state. </summary>
        public List<GameObject> ActiveGameObjects => activeGameObjects;

        /// <inheritdoc />
        public void OnBeforeSerialize() => this.OnValidate();

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
        }

        private void OnValidate()
        {
            name = $"{this.activeOn.ToString()} - {ActiveGameObjects.Count}";
        }
    }
}
