using UnityEngine;
using UnityEngine.InputSystem;

namespace MazePlace.Game
{
    /// <summary>Orbit camera for looking down on the maze: right-drag to rotate, scroll to zoom.</summary>
    [RequireComponent(typeof(Camera))]
    public sealed class OverviewCamera : MonoBehaviour
    {
        [SerializeField] private float degreesPerPixel = 0.25f;
        [SerializeField] private float zoomStep = 0.1f;
        [SerializeField] private float minPitch = 15f;
        [SerializeField] private float maxPitch = 89f;

        private Camera viewCamera;
        private Vector3 focus;
        private float yaw;
        private float pitch = 60f;
        private float distance = 50f;
        private float maxDistance = 100f;

        public Camera Camera => viewCamera != null ? viewCamera : viewCamera = GetComponent<Camera>();

        /// <summary>Resets the view to fit the whole of <paramref name="bounds"/>.</summary>
        public void Frame(Bounds bounds)
        {
            var radius = bounds.extents.magnitude;
            var verticalFov = Camera.fieldOfView;
            var horizontalFov = Camera.VerticalToHorizontalFieldOfView(verticalFov, Camera.aspect);
            var fitFov = Mathf.Min(verticalFov, horizontalFov) * Mathf.Deg2Rad;

            focus = bounds.center;
            yaw = 0f;
            pitch = 60f;
            distance = radius / Mathf.Sin(fitFov / 2);
            maxDistance = distance * 2;
            Camera.farClipPlane = maxDistance + radius * 2;
            Apply();
        }

        public void SetFocusHeight(float height)
        {
            focus.y = height;
            Apply();
        }

        private void LateUpdate()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            // Left clicks belong to the HUD buttons.
            if (mouse.rightButton.isPressed)
            {
                var delta = mouse.delta.ReadValue() * degreesPerPixel;
                yaw += delta.x;
                pitch = Mathf.Clamp(pitch - delta.y, minPitch, maxPitch);
            }

            // Scroll magnitude varies wildly between platforms and devices, so only its sign is used.
            var scroll = mouse.scroll.ReadValue().y;
            if (scroll != 0f)
                distance = Mathf.Clamp(distance * (1f - Mathf.Sign(scroll) * zoomStep), 2f, maxDistance);

            Apply();
        }

        private void Apply()
        {
            var rotation = Quaternion.Euler(pitch, yaw, 0f);
            transform.SetPositionAndRotation(focus - rotation * Vector3.forward * distance, rotation);
        }
    }
}
