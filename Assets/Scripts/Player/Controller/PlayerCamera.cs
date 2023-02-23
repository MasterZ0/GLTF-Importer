using GLTFImporter.Gameplay;
using System;
using UnityEngine;

namespace GLTFImporter.Player
{
    [Serializable]
    public class PlayerCamera : PlayerControllerComponent
    {
        [Header("Camera")]
        [SerializeField] private Transform cameraTarget;

        public bool XLocked { get; private set; }
        public bool YLocked { get; private set; }

        private float xRotation;
        private float yRotation;

        private const float Threshold = 0.01f;
        public Transform CameraTarget => MainCamera.Transform;

        #region Initialization
        public override void Init(PlayerController playerController)
        {
            base.Init(playerController);

            yRotation = cameraTarget.localEulerAngles.y;

            MainCamera.SetPlayerTarget(cameraTarget);
        }
        #endregion

        public void Update()
        {
            UpdateCameraRotation();
        }

        private void UpdateCameraRotation()
        {
            Vector2 look = Controller.Inputs.Look;

            if (look.sqrMagnitude >= Threshold)
            {
                if (!XLocked)
                {
                    xRotation -= look.y * Data.MouseSensitivity;
                    xRotation = Mathf.Clamp(xRotation, Data.CameraRangeRotation.x, Data.CameraRangeRotation.y);
                }

                if (!YLocked)
                {
                    yRotation += look.x * Data.MouseSensitivity;
                    yRotation = ClampAngle(yRotation);
                }
                else
                {
                    yRotation = cameraTarget.eulerAngles.y;
                }
            }

            cameraTarget.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }

        private static float ClampAngle(float angle)
        {
            if (angle < -360f)
                angle += 360f;

            if (angle > 360f)
                angle -= 360f;

            return angle;
        }
    }
}