using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystemActions m_InputActions;
    
    public Vector2 MoveDirection { get; private set; }

    public bool IsLightAttackInput { get; private set; } = false;
    public bool IsHeavyAttackInput { get; private set; } = false;    
    public ComboType LatestComboTypeInput { get; private set; } = ComboType.None;
    
    public bool IsLockOnTarget { get; private set; } = false;
    
    public Vector2 LookDirection { get; private set; } = Vector2.zero;

    private void Awake()
    {
        m_InputActions = new InputSystemActions();
    }

    private void OnEnable()
    {
        m_InputActions.Player.Move.performed += move =>
        {
            MoveDirection = move.ReadValue<Vector2>();
        };
        m_InputActions.Player.Move.canceled += move =>
        {
            MoveDirection = Vector2.zero;
        };
        
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

        m_InputActions.Player.LockOnTarget.performed += intput =>
        {
            IsLockOnTarget = !IsLockOnTarget;
        };
        
        m_InputActions.Player.Look.performed += intput => LookDirection = intput.ReadValue<Vector2>();
        m_InputActions.Player.Look.canceled += intput => LookDirection = Vector2.zero;
        
        m_InputActions.Enable();
    }

    private void OnDisable()
    {
        m_InputActions.Disable();
    }        
}