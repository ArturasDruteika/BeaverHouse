using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Invector.vCharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region Variables       

        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode strafeInput = KeyCode.Tab;
        public KeyCode sprintInput = KeyCode.LeftShift;

        [Header("Camera Input")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        [Header("Input Backend")]
        [Tooltip("Use the old Input Manager (Input.GetAxis/KeyCode). Disable this when using New Input System only.")]
        public bool useLegacyInput = false;

        [HideInInspector] public vThirdPersonController cc;
        [HideInInspector] public vThirdPersonCamera tpCamera;
        [HideInInspector] public Camera cameraMain;

        #endregion

        protected virtual void Start()
        {
            InitilizeController();
            InitializeTpCamera();
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            cc.ControlRotationType();       // handle the controller rotation type
        }

        protected virtual void Update()
        {
            InputHandle();                  // update the input methods
            cc.UpdateAnimator();            // updates the Animator Parameters
        }

        public virtual void OnAnimatorMove()
        {
            cc.ControlAnimatorRootMotion(); // handle root motion animations 
        }

        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            cc = GetComponent<vThirdPersonController>();

            if (cc != null)
                cc.Init();
        }

        protected virtual void InitializeTpCamera()
        {
            if (tpCamera == null)
            {
                tpCamera = FindFirstObjectByType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }

        protected virtual void InputHandle()
        {
            MoveInput();
            CameraInput();
            SprintInput();
            StrafeInput();
            JumpInput();
        }

        public virtual void MoveInput()
        {
            var move = ReadMoveInput();
            cc.input.x = move.x;
            cc.input.z = move.y;
        }

        protected virtual void CameraInput()
        {
            if (!cameraMain)
            {
                if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = Camera.main;
                    cc.rotateTarget = cameraMain.transform;
                }
            }

            if (cameraMain)
            {
                cc.UpdateMoveDirection(cameraMain.transform);
            }

            if (tpCamera == null)
                return;

            var look = ReadLookInput();
            var X = look.x;
            var Y = look.y;

            tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput()
        {
            if (GetStrafeDown())
                cc.Strafe();
        }

        protected virtual void SprintInput()
        {
            if (GetSprintDown())
                cc.Sprint(true);
            else if (GetSprintUp())
                cc.Sprint(false);
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove;
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            if (GetJumpDown() && JumpConditions())
                cc.Jump();
        }

        protected virtual Vector2 ReadMoveInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (!useLegacyInput)
            {
                Vector2 move = Vector2.zero;

                if (Gamepad.current != null)
                    move = Gamepad.current.leftStick.ReadValue();

                if (Keyboard.current != null)
                {
                    Vector2 keyboardMove = Vector2.zero;
                    if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) keyboardMove.x -= 1f;
                    if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) keyboardMove.x += 1f;
                    if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) keyboardMove.y -= 1f;
                    if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) keyboardMove.y += 1f;

                    if (keyboardMove.sqrMagnitude > 0f)
                        move = Vector2.ClampMagnitude(keyboardMove, 1f);
                }

                return move;
            }
#endif
            return new Vector2(Input.GetAxis(horizontalInput), Input.GetAxis(verticallInput));
        }

        protected virtual Vector2 ReadLookInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (!useLegacyInput)
            {
                Vector2 look = Vector2.zero;

                if (Mouse.current != null)
                    look += Mouse.current.delta.ReadValue() * 0.02f;

                if (Gamepad.current != null)
                    look += Gamepad.current.rightStick.ReadValue();

                return look;
            }
#endif
            return new Vector2(Input.GetAxis(rotateCameraXInput), Input.GetAxis(rotateCameraYInput));
        }

        protected virtual bool GetStrafeDown()
        {
#if ENABLE_INPUT_SYSTEM
            if (!useLegacyInput)
            {
                var keyboard = Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame;
                var gamepad = Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame;
                return keyboard || gamepad;
            }
#endif
            return Input.GetKeyDown(strafeInput);
        }

        protected virtual bool GetSprintDown()
        {
#if ENABLE_INPUT_SYSTEM
            if (!useLegacyInput)
            {
                var keyboard = Keyboard.current != null && Keyboard.current.leftShiftKey.wasPressedThisFrame;
                var gamepad = Gamepad.current != null && Gamepad.current.leftStickButton.wasPressedThisFrame;
                return keyboard || gamepad;
            }
#endif
            return Input.GetKeyDown(sprintInput);
        }

        protected virtual bool GetSprintUp()
        {
#if ENABLE_INPUT_SYSTEM
            if (!useLegacyInput)
            {
                var keyboard = Keyboard.current != null && Keyboard.current.leftShiftKey.wasReleasedThisFrame;
                var gamepad = Gamepad.current != null && Gamepad.current.leftStickButton.wasReleasedThisFrame;
                return keyboard || gamepad;
            }
#endif
            return Input.GetKeyUp(sprintInput);
        }

        protected virtual bool GetJumpDown()
        {
#if ENABLE_INPUT_SYSTEM
            if (!useLegacyInput)
            {
                var keyboard = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
                var gamepad = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;
                return keyboard || gamepad;
            }
#endif
            return Input.GetKeyDown(jumpInput);
        }

        #endregion       
    }
}