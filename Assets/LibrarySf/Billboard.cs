using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LibrarySf
{
    public enum BillboardModeEnum
    {
        AngleToCamera = 1,        // Billboard rotates to face away from the camera's position
        DirectionOfCamera = 2,    // Billboard rotates to always have the same forward direction as the camera, good for isometric views
        InverseCameraForward = 3, // Billboard rotates to always have the opposite forward direction as the camera, good for isometric views
    }

    public class Billboard : MonoBehaviour
    {
        [Tooltip("Camera or object to billboard towards")]
        public Transform CameraTransform;

        public BillboardModeEnum BillboardMode = BillboardModeEnum.AngleToCamera;

        private void Start()
        {
            if (CameraTransform == null)
            {
                CameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            switch (BillboardMode)
            {
                case BillboardModeEnum.AngleToCamera:
                    AngleToCamera();
                    break;
                case BillboardModeEnum.DirectionOfCamera:
                    transform.rotation = CameraTransform.rotation;
                    break;
                case BillboardModeEnum.InverseCameraForward:
                    transform.rotation = Quaternion.LookRotation(CameraTransform.forward * -1, CameraTransform.up);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Copied from https://www.reddit.com/r/Unity3D/comments/2v1fz5/making_ugui_labels_in_world_space_retain/ except OnWillRenderObject() didn't work
        /// </summary>
        private void AngleToCamera()
        {
            Vector3 cameraDirection = transform.position - CameraTransform.position;
            transform.rotation = Quaternion.LookRotation(cameraDirection);
        }
    }
}
