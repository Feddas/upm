using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Feddas
{
    public class CropImagePreload : MonoBehaviour
    {
        [Tooltip("Set of images where the computation for an image crop should be done ahead of time.")]
        [SerializeField]
        private List<CropImageTraits> cropImagesPreloading = new List<CropImageTraits>();

        private void Start()
        {
            SetDirty();
        }

        [ContextMenu("SetDirty")]
        private void SetDirty()
        {
            foreach (var item in cropImagesPreloading)
            {
                if (item.Sprite == null) // Verify OnEnable() was called. If it wasn't, accessing this field will cause the sprite to attempt to regenerate.
                {
                    Debug.LogException(new System.Exception($"{item.name} could not be cropped."));
                }
            }
        }

        /// <summary> A method to help debug if there is a current issue with CropImagePreload. </summary>
        [ContextMenu("NumNull")]
        private void NumNull()
        {
            Debug.Log("There are this many that are null:" + cropImagesPreloading.Count(c => c.Sprite == null));
        }
    }
}
