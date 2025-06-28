// PlayerInputActions.cs (자동 생성)
using System;
using UnityEngine.InputSystem;

public class PlayerInputActions : IDisposable
{
    private InputActionAsset m_InputActionAsset;
    public PlayerActions Actions { get; }

    public PlayerInputActions()
    {
        m_InputActionAsset = InputActionAsset.FromJson(@"..."); // 자동 생성된 JSON 내용
        Actions = new PlayerActions(this);
    }

    public void Dispose() => UnityEngine.Object.Destroy(m_InputActionAsset);

    public struct PlayerActions
    {
        private PlayerInputActions wrapper;

        public PlayerActions(PlayerInputActions wrapper) { this.wrapper = wrapper; }
        public InputAction Move => wrapper.m_InputActionAsset.FindAction("Player/Move");
        public InputAction Attack => wrapper.m_InputActionAsset.FindAction("Player/Attack");
        public InputAction Parry => wrapper.m_InputActionAsset.FindAction("Player/Parry");
        public InputAction Block => wrapper.m_InputActionAsset.FindAction("Player/Block");

        public void Enable() => wrapper.m_InputActionAsset.Enable();
        public void Disable() => wrapper.m_InputActionAsset.Disable();
    }
}
