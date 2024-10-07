using UnityEngine;

namespace Feddas
{
    /// <summary>
    /// CropImageTraits uses a ScriptableObject as runtime storage for non serializable data.
    /// https://forum.unity.com/threads/scriptableobject-variable-type-mismatch-when-its-clearly-one-type.715892/.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCropImageTraits", menuName = "Scriptable Objects/UI Kit/Create New Crop Image Traits Asset", order = 0)]
    public class CropImageTraits : ScriptableObject
    {
        [Tooltip("Sprite to be cropped.")]
        [SerializeField]
        private Sprite source;

        [Tooltip("X,Y are treated as the center of the crop. W,H are percentages of size.")]
        [ContextMenuItem("Force new crop", "DoCrop")]
        [SerializeField]
        private Rect cropArea = Rect.MinMaxRect(0f, 0f, .8f, .8f);

        /// <summary> <seealso cref="sprite"/> is a backing field for a property. Use the <seealso cref="Sprite"/> property instead. </summary>
        private Sprite sprite;

        /// <summary> Source sprite after it has been cropped. </summary>
        public Sprite Sprite
        {
            get
            {
                if (sprite == null)
                {
                    Debug.LogException(new System.Exception($"{this.name} had an error preloading. Ensure this sprite is added to CropImagePreload in the _init scene."));

                    // attempt to recalculate crop
                    Dirty();
                }

                return sprite;
            }
            private set
            {
                sprite = value;
            }
        }

        public void Dirty()
        {
            Sprite = CropImageAtRuntime.DoCrop(cropArea, source.texture, source.pivot);
        }

        private void OnEnable()
        {
            if (source == null)
            {
                Sprite = null;
                Debug.LogException(new System.Exception($"{this.name} needs to have a sprite set."));
            }
            else if (sprite == null)
            { // Note: the line above is using the sprite backing field to avoid the "Hasn't preloaded" exception that should only be circumvented during OnEnable's initialization.
                Dirty();
            }
        }

        private void OnDisable()
        {
            // This scriptable object will automatically be flagged for Garbage Collection if a scene never makes a reference to it. The lost reference means the asset will need to be recomputed.
        }
    }
}
