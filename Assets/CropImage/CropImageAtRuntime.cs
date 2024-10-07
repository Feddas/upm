using UnityEngine;
using UnityEngine.UI;

namespace Feddas
{
    /// <summary>
    /// Are you not sure what you'll mess up if you crop a raw *.png or other image file in an image editor?
    /// CropImageAtRuntime will crop the image only at runtime to ensure modifying the original doesn't mess something else up.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class CropImageAtRuntime : MonoBehaviour
    {
        [Tooltip("X,Y are treated as the center of the crop. W,H are percentages of size.")]
        [ContextMenuItem("Force new crop", "DoCrop")]
        [SerializeField]
        private Rect cropArea = Rect.MinMaxRect(0f, 0f, .8f, .8f);

        [Tooltip("Overrides this crop with whatever is found in the trait")]
        [SerializeField]
        private CropImageTraits cropTrait;

        // Backing fields for properties. AKA don't use these fields, use the associated property.
        private RectTransform cachedRectTransform;
        private Image originalImage;

        private RectTransform CachedRectTransform
        {
            get
            {
                if (cachedRectTransform == null)
                {
                    cachedRectTransform = this.GetComponent<RectTransform>();
                }

                return cachedRectTransform;
            }
        }

        private Image OriginalImage
        {
            get
            {
                if (originalImage == null)
                {
                    originalImage = this.GetComponent<Image>();
                }

                return originalImage;
            }
        }

        public static Sprite DoCrop(Rect cropArea, Texture2D originalTexture, Vector2 pivot)
        {
            var textureCropArea = CropAreaMappedToSize(cropArea, originalTexture.width, originalTexture.height);
            return Sprite.Create(originalTexture, textureCropArea, pivot, 100, 0, SpriteMeshType.FullRect); // https://forum.unity.com/threads/any-way-to-speed-up-sprite-create.529525/
        }

        /// <summary> Convert percentages of cropArea to the dimensions of a texture. </summary>
        private static Rect CropAreaMappedToSize(Rect cropArea, int textureWidth, int textureHeight)
        {
            return Rect.MinMaxRect(
                (.5f + cropArea.position.x - (cropArea.width / 2)) * textureWidth,
                (.5f + cropArea.position.y - (cropArea.height / 2)) * textureHeight,
                (.5f + cropArea.position.x + (cropArea.width / 2)) * textureWidth,
                (.5f + cropArea.position.y + (cropArea.height / 2)) * textureHeight);
        }

        private void Start()
        {
            if (cropTrait)
            {
                OriginalImage.sprite = cropTrait.Sprite;
            }
            else
            {
                OriginalImage.sprite = DoCrop();
            }
        }

        /// <summary>
        /// Crops the texture to the dimensions of cropArea
        /// https://stackoverflow.com/questions/65339693/crop-an-image-using-another-rect-in-unity .
        /// </summary>
        private Sprite DoCrop()
        {
            return DoCrop(cropArea, (Texture2D)OriginalImage.mainTexture, OriginalImage.sprite.pivot);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1.0f, 0.5f, 0.0f); // Orange

            // Convert rect to center & percentages before drawing it
            // HACK: rectTransform.rect.size only works if the anchor values are the same. see http://answers.unity.com/answers/1132809/view.html
            var percentToSize = CachedRectTransform.rect.size * CachedRectTransform.lossyScale // This is the images size in global space
                * cropArea.size;
            var center = (cropArea.position * CachedRectTransform.lossyScale // scale by global scale
                * 100) // change to 0-1 instead of from 0-100
                + (Vector2)this.transform.position; // find relative center
            Gizmos.DrawWireCube(center, percentToSize); // as suggested by https://forum.unity.com/threads/visulization-of-a-rect.558430/
        }
    }
}
