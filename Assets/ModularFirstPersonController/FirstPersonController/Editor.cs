#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FirstPersonController)), InitializeOnLoad]
public class FirstPersonControllerEditor : Editor
{
    private SerializedProperty _moveAction;
    private SerializedProperty _lookAction;
    private SerializedProperty _jumpAction;
    private SerializedProperty _sprintAction;
    private SerializedProperty _crouchAction;
    private SerializedProperty _zoomAction;

    private SerializedProperty _playerCamera;
    private SerializedProperty _fov;
    private SerializedProperty _invertCamera;
    private SerializedProperty _cameraCanMove;
    private SerializedProperty _mouseSensitivity;
    private SerializedProperty _maxLookAngle;

    private SerializedProperty _lockCursor;
    private SerializedProperty _crosshair;
    private SerializedProperty _crosshairImage;
    private SerializedProperty _crosshairColor;

    private SerializedProperty _enableZoom;
    private SerializedProperty _holdToZoom;
    private SerializedProperty _zoomFOV;
    private SerializedProperty _zoomStepTime;

    private SerializedProperty _playerCanMove;
    private SerializedProperty _walkSpeed;
    private SerializedProperty _maxVelocityChange;

    private SerializedProperty _enableSprint;
    private SerializedProperty _unlimitedSprint;
    private SerializedProperty _sprintSpeed;
    private SerializedProperty _sprintDuration;
    private SerializedProperty _sprintCooldown;
    private SerializedProperty _sprintFOV;
    private SerializedProperty _sprintFOVStepTime;

    private SerializedProperty _useSprintBar;
    private SerializedProperty _hideBarWhenFull;
    private SerializedProperty _sprintBarBG;
    private SerializedProperty _sprintBar;
    private SerializedProperty _sprintBarWidthPercent;
    private SerializedProperty _sprintBarHeightPercent;

    private SerializedProperty _enableJump;
    private SerializedProperty _jumpPower;

    private SerializedProperty _enableCrouch;
    private SerializedProperty _holdToCrouch;
    private SerializedProperty _crouchHeight;
    private SerializedProperty _speedReduction;

    private SerializedProperty _enableHeadBob;
    private SerializedProperty _joint;
    private SerializedProperty _bobSpeed;
    private SerializedProperty _bobAmount;

    private void OnEnable()
    {
        _moveAction = serializedObject.FindProperty("_moveAction");
        _lookAction = serializedObject.FindProperty("_lookAction");
        _jumpAction = serializedObject.FindProperty("_jumpAction");
        _sprintAction = serializedObject.FindProperty("_sprintAction");
        _crouchAction = serializedObject.FindProperty("_crouchAction");
        _zoomAction = serializedObject.FindProperty("_zoomAction");

        _playerCamera = serializedObject.FindProperty("_playerCamera");
        _fov = serializedObject.FindProperty("_fov");
        _invertCamera = serializedObject.FindProperty("_invertCamera");
        _cameraCanMove = serializedObject.FindProperty("_cameraCanMove");
        _mouseSensitivity = serializedObject.FindProperty("_mouseSensitivity");
        _maxLookAngle = serializedObject.FindProperty("_maxLookAngle");

        _lockCursor = serializedObject.FindProperty("_lockCursor");
        _crosshair = serializedObject.FindProperty("_crosshair");
        _crosshairImage = serializedObject.FindProperty("_crosshairImage");
        _crosshairColor = serializedObject.FindProperty("_crosshairColor");

        _enableZoom = serializedObject.FindProperty("_enableZoom");
        _holdToZoom = serializedObject.FindProperty("_holdToZoom");
        _zoomFOV = serializedObject.FindProperty("_zoomFOV");
        _zoomStepTime = serializedObject.FindProperty("_zoomStepTime");

        _playerCanMove = serializedObject.FindProperty("_playerCanMove");
        _walkSpeed = serializedObject.FindProperty("_walkSpeed");
        _maxVelocityChange = serializedObject.FindProperty("_maxVelocityChange");

        _enableSprint = serializedObject.FindProperty("_enableSprint");
        _unlimitedSprint = serializedObject.FindProperty("_unlimitedSprint");
        _sprintSpeed = serializedObject.FindProperty("_sprintSpeed");
        _sprintDuration = serializedObject.FindProperty("_sprintDuration");
        _sprintCooldown = serializedObject.FindProperty("_sprintCooldown");
        _sprintFOV = serializedObject.FindProperty("_sprintFOV");
        _sprintFOVStepTime = serializedObject.FindProperty("_sprintFOVStepTime");

        _useSprintBar = serializedObject.FindProperty("_useSprintBar");
        _hideBarWhenFull = serializedObject.FindProperty("_hideBarWhenFull");
        _sprintBarBG = serializedObject.FindProperty("_sprintBarBG");
        _sprintBar = serializedObject.FindProperty("_sprintBar");
        _sprintBarWidthPercent = serializedObject.FindProperty("_sprintBarWidthPercent");
        _sprintBarHeightPercent = serializedObject.FindProperty("_sprintBarHeightPercent");

        _enableJump = serializedObject.FindProperty("_enableJump");
        _jumpPower = serializedObject.FindProperty("_jumpPower");

        _enableCrouch = serializedObject.FindProperty("_enableCrouch");
        _holdToCrouch = serializedObject.FindProperty("_holdToCrouch");
        _crouchHeight = serializedObject.FindProperty("_crouchHeight");
        _speedReduction = serializedObject.FindProperty("_speedReduction");

        _enableHeadBob = serializedObject.FindProperty("_enableHeadBob");
        _joint = serializedObject.FindProperty("_joint");
        _bobSpeed = serializedObject.FindProperty("_bobSpeed");
        _bobAmount = serializedObject.FindProperty("_bobAmount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        GUILayout.Label("Modular First Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        GUILayout.Label("By Jess Case", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        GUILayout.Label("version 1.0.1", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Input Actions (New Input System)", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_moveAction, new GUIContent("Move", "Vector2 action (WASD/Stick)."));
        EditorGUILayout.PropertyField(_lookAction, new GUIContent("Look", "Vector2 action (Mouse Delta/Stick)."));
        EditorGUILayout.PropertyField(_jumpAction, new GUIContent("Jump", "Button action."));
        EditorGUILayout.PropertyField(_sprintAction, new GUIContent("Sprint", "Button action (hold)."));
        EditorGUILayout.PropertyField(_crouchAction, new GUIContent("Crouch", "Button action (toggle or hold)."));
        EditorGUILayout.PropertyField(_zoomAction, new GUIContent("Zoom", "Button action (toggle or hold)."));

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Camera Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_playerCamera, new GUIContent("Camera", "Camera attached to the controller."));
        EditorGUILayout.Slider(_fov, _zoomFOV.floatValue, 179f, new GUIContent("Field of View", "The camera view angle. Changes the player camera directly."));
        _cameraCanMove.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Enable Camera Rotation", "Determines if the camera is allowed to move."), _cameraCanMove.boolValue);

        GUI.enabled = _cameraCanMove.boolValue;
        _invertCamera.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Invert Camera Rotation", "Inverts the up and down movement of the camera."), _invertCamera.boolValue);
        EditorGUILayout.Slider(_mouseSensitivity, .1f, 10f, new GUIContent("Look Sensitivity", "Determines how sensitive the look input is."));
        EditorGUILayout.Slider(_maxLookAngle, 40, 90, new GUIContent("Max Look Angle", "Determines the max and min angle the player camera is able to look."));
        GUI.enabled = true;

        _lockCursor.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Lock and Hide Cursor", "Turns off cursor visibility and locks it to the middle of the screen."), _lockCursor.boolValue);

        _crosshair.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Auto Crosshair", "Determines if the basic crosshair will be turned on and centered."), _crosshair.boolValue);

        if (_crosshair.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_crosshairImage, new GUIContent("Crosshair Image", "Sprite to use as the crosshair."));
            EditorGUILayout.PropertyField(_crosshairColor, new GUIContent("Crosshair Color", "Determines the color of the crosshair."));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        GUILayout.Label("Zoom", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        _enableZoom.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Enable Zoom", "Determines if the player is able to zoom in while playing."), _enableZoom.boolValue);

        GUI.enabled = _enableZoom.boolValue;
        _holdToZoom.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Hold to Zoom", "Requires holding the zoom action instead of toggling."), _holdToZoom.boolValue);
        EditorGUILayout.Slider(_zoomFOV, .1f, _fov.floatValue, new GUIContent("Zoom FOV", "Determines the field of view the camera zooms to."));
        EditorGUILayout.Slider(_zoomStepTime, .1f, 10f, new GUIContent("Step Time", "Determines how fast the FOV transitions while zooming in."));
        GUI.enabled = true;

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        _playerCanMove.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Movement", "Determines if the player is allowed to move."), _playerCanMove.boolValue);

        GUI.enabled = _playerCanMove.boolValue;
        EditorGUILayout.Slider(_walkSpeed, .1f, _sprintSpeed.floatValue, new GUIContent("Walk Speed", "Determines how fast the player will move while walking."));
        EditorGUILayout.Slider(_maxVelocityChange, .1f, 50f, new GUIContent("Max Velocity Change", "Maximum velocity change applied by the controller."));
        GUI.enabled = true;

        EditorGUILayout.Space();
        GUILayout.Label("Sprint", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        _enableSprint.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Enable Sprint", "Determines if the player is allowed to sprint."), _enableSprint.boolValue);

        GUI.enabled = _enableSprint.boolValue;
        _unlimitedSprint.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Unlimited Sprint", "Allows infinite sprint (no duration)."), _unlimitedSprint.boolValue);
        EditorGUILayout.Slider(_sprintSpeed, _walkSpeed.floatValue, 20f, new GUIContent("Sprint Speed", "Determines how fast the player will move while sprinting."));

        EditorGUILayout.Slider(_sprintDuration, 1f, 20f, new GUIContent("Sprint Duration", "How long the player can sprint while unlimited sprint is disabled."));
        EditorGUILayout.Slider(_sprintCooldown, .1f, _sprintDuration.floatValue, new GUIContent("Sprint Cooldown", "Recovery time when the player runs out of sprint."));

        EditorGUILayout.Slider(_sprintFOV, _fov.floatValue, 179f, new GUIContent("Sprint FOV", "Camera FOV while sprinting."));
        EditorGUILayout.Slider(_sprintFOVStepTime, .1f, 20f, new GUIContent("Step Time", "How fast the FOV transitions while sprinting."));

        _useSprintBar.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Use Sprint Bar", "Shows the sprint bar on screen."), _useSprintBar.boolValue);

        if (_useSprintBar.boolValue)
        {
            EditorGUI.indentLevel++;
            _hideBarWhenFull.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Hide Full Bar", "Hides sprint bar when full and fades in when sprinting."), _hideBarWhenFull.boolValue);
            EditorGUILayout.PropertyField(_sprintBarBG, new GUIContent("Bar BG", "Sprint bar background image."));
            EditorGUILayout.PropertyField(_sprintBar, new GUIContent("Bar", "Sprint bar foreground image."));
            EditorGUILayout.Slider(_sprintBarWidthPercent, .1f, .5f, new GUIContent("Bar Width", "Width of the sprint bar."));
            EditorGUILayout.Slider(_sprintBarHeightPercent, .001f, .025f, new GUIContent("Bar Height", "Height of the sprint bar."));
            EditorGUI.indentLevel--;
        }

        GUI.enabled = true;

        EditorGUILayout.Space();
        GUILayout.Label("Jump", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        _enableJump.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Enable Jump", "Determines if the player is allowed to jump."), _enableJump.boolValue);

        GUI.enabled = _enableJump.boolValue;
        EditorGUILayout.Slider(_jumpPower, .1f, 20f, new GUIContent("Jump Power", "Determines how high the player will jump."));
        GUI.enabled = true;

        EditorGUILayout.Space();
        GUILayout.Label("Crouch", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        _enableCrouch.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Enable Crouch", "Determines if the player is allowed to crouch."), _enableCrouch.boolValue);

        GUI.enabled = _enableCrouch.boolValue;
        _holdToCrouch.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Hold To Crouch", "Requires holding the crouch action instead of toggling."), _holdToCrouch.boolValue);
        EditorGUILayout.Slider(_crouchHeight, .1f, 1f, new GUIContent("Crouch Height", "Y scale of the player when crouched."));
        EditorGUILayout.Slider(_speedReduction, .1f, 1f, new GUIContent("Speed Reduction", "Percent Walk Speed is reduced by when crouched."));
        GUI.enabled = true;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Head Bob Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        _enableHeadBob.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Enable Head Bob", "Determines if the camera will bob while walking."), _enableHeadBob.boolValue);

        GUI.enabled = _enableHeadBob.boolValue;
        EditorGUILayout.PropertyField(_joint, new GUIContent("Camera Joint", "Joint object position is moved while head bob is active."));
        EditorGUILayout.Slider(_bobSpeed, 1, 20, new GUIContent("Speed", "How often a bob is completed."));
        EditorGUILayout.PropertyField(_bobAmount, new GUIContent("Bob Amount", "Amount the joint moves each axis."));
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
