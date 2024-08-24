using UnityEngine;

namespace LibrarySf
{
    /// <summary>
    /// When creating a List<System.Serializable> this replaces "Element #" in the Unity inspector with a custom name.
    /// Takes advantage of Unity using the first string value to name an element in a serializable list.
    /// </summary>
    public abstract class NameOnSerialize : ISerializationCallbackReceiver
    {
        /// <summary>
        /// Example implementation: public override string ElementName { get { return MeshObject.name; } }
        /// </summary>
        public abstract string ElementName
        {
            get;
        }

        [HideInInspector]
        public string Name;

        /// <summary>
        /// Called OnValidate https://answers.unity.com/questions/1345501/how-can-i-get-functionality-similar-to-onvalidate.html
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.Name = ElementName;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }
    }
}
