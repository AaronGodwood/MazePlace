using MazePlace.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MazePlace.Game
{
    /// <summary>
    /// First-person walker. Holding Space under an upward shaft climbs to the next level;
    /// walking into a downward shaft drops through it.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float degreesPerPixel = 0.1f;
        [SerializeField] private float climbSpeed = 3f;
        [SerializeField] private float gravity = -20f;

        private CharacterController body;
        private Maze maze;
        private float pitch;
        private float verticalSpeed;
        private float? climbTargetHeight;

        public Camera Camera => head.GetComponent<Camera>();

        private void Awake() => body = GetComponent<CharacterController>();

        public void Spawn(Maze maze, int cell)
        {
            this.maze = maze;
            body = body != null ? body : GetComponent<CharacterController>();

            // CharacterController overrides transform changes while enabled.
            body.enabled = false;
            transform.SetPositionAndRotation(MazeLayout.CellCentre(maze.Size, cell) + Vector3.up * 0.1f, FacingOpenPassage(maze, cell));
            body.enabled = true;

            pitch = 0f;
            head.localRotation = Quaternion.identity;
            verticalSpeed = 0f;
            climbTargetHeight = null;
        }

        private static Quaternion FacingOpenPassage(Maze maze, int cell)
        {
            var headings = new[] { (Direction.North, 0f), (Direction.East, 90f), (Direction.South, 180f), (Direction.West, 270f) };
            foreach (var (direction, yaw) in headings)
                if (maze.IsOpen(cell, direction))
                    return Quaternion.Euler(0f, yaw, 0f);
            return Quaternion.identity;
        }

        private void OnEnable() => Cursor.lockState = CursorLockMode.Locked;
        private void OnDisable() => Cursor.lockState = CursorLockMode.None;

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (maze == null || keyboard == null || mouse == null) return;

            Look(mouse.delta.ReadValue());

            var climbHeld = keyboard.spaceKey.isPressed;
            var horizontal = Rising ? PullTowardsShaft() : Walk(keyboard);
            UpdateVertical(climbHeld);

            body.Move((horizontal + Vector3.up * verticalSpeed) * Time.deltaTime);
        }

        private void Look(Vector2 delta)
        {
            transform.Rotate(0f, delta.x * degreesPerPixel, 0f);
            pitch = Mathf.Clamp(pitch - delta.y * degreesPerPixel, -85f, 85f);
            head.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private Vector3 Walk(Keyboard keyboard)
        {
            var input = new Vector3(
                (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0),
                0f,
                (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0));
            return transform.TransformDirection(Vector3.ClampMagnitude(input, 1f)) * moveSpeed;
        }

        private void UpdateVertical(bool climbHeld)
        {
            if (!climbTargetHeight.HasValue && climbHeld && TryGetCell(out var cell) && maze.IsOpen(cell, Direction.Up))
                climbTargetHeight = (maze.Size.CellAt(cell).Level + 1) * MazeLayout.LevelSpacing + 0.05f;

            if (climbTargetHeight.HasValue)
            {
                // At the top the player is still over the hole, so they hover until they step onto the rim.
                if (!climbHeld || (!Rising && !OverShaft()))
                {
                    climbTargetHeight = null;
                    verticalSpeed = 0f;
                }
                else
                {
                    verticalSpeed = Rising ? climbSpeed : 0f;
                    return;
                }
            }

            // A small downward push keeps isGrounded reliable while walking.
            verticalSpeed = body.isGrounded ? -1f : verticalSpeed + gravity * Time.deltaTime;
        }

        private bool Rising => climbTargetHeight.HasValue && transform.position.y < climbTargetHeight.Value;

        private bool OverShaft()
        {
            if (!TryGetCell(out var cell)) return false;
            var offset = transform.position - MazeLayout.CellCentre(maze.Size, cell);
            var reach = MazeLayout.ShaftSize / 2 + body.radius;
            return Mathf.Abs(offset.x) < reach && Mathf.Abs(offset.z) < reach;
        }

        /// <summary>Keeps the player centred while climbing so they fit through the shaft.</summary>
        private Vector3 PullTowardsShaft()
        {
            if (!TryGetCell(out var cell)) return Vector3.zero;
            var offset = MazeLayout.CellCentre(maze.Size, cell) - transform.position;
            offset.y = 0f;
            return offset * 8f;
        }

        private bool TryGetCell(out int cell) => MazeLayout.TryGetCell(maze.Size, transform.position, out cell);
    }
}
