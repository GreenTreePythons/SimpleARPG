using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystemActions m_InputActions;
    
    public Vector2 MoveDirection { get; private set; }
    public Vector2 CameraRotateInput { get; private set; }

    public bool IsLightAttackInput { get; private set; } = false;
    public bool IsHeavyAttackInput { get; private set; } = false;    
    public ComboType LatestComboTypeInput { get; private set; } = ComboType.None;
    
    public bool IsMoving { get; private set; } = false;

    private void Awake()
    {
        m_InputActions = new InputSystemActions();
    }

    private void OnEnable()
    {
        m_InputActions.Player.Move.performed += move =>
        {
            MoveDirection = move.ReadValue<Vector2>();
            IsMoving = MoveDirection.magnitude > 0.01f;
        };
        m_InputActions.Player.Move.canceled += move =>
        {
            MoveDirection = Vector2.zero;
            IsMoving = false;
        };
        m_InputActions.Player.CameraRotate.performed += rotate => CameraRotateInput = rotate.ReadValue<Vector2>();
        m_InputActions.Player.CameraRotate.canceled += rotate => CameraRotateInput = Vector2.zero;
        
        m_InputActions.Player.LightAttack.performed += intput =>
        {
            IsLightAttackInput = true;
            LatestComboTypeInput = ComboType.Light;
        };
        m_InputActions.Player.LightAttack.canceled += intput =>
        {
            IsLightAttackInput = false;
            LatestComboTypeInput = ComboType.None;
        };
        m_InputActions.Player.HeavyAttack.performed += intput =>
        {
            IsHeavyAttackInput = true;
            LatestComboTypeInput = ComboType.Heavy;
        };
        m_InputActions.Player.HeavyAttack.canceled += intput =>
        {
            IsHeavyAttackInput = false;
            LatestComboTypeInput = ComboType.None;
        };
        
        m_InputActions.Enable();
    }

    private void OnDisable()
    {
        m_InputActions.Disable();
    }        
}