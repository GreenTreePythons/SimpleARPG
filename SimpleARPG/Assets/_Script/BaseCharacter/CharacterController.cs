using UnityEngine;

public abstract class CharacterController : MonoBehaviour, IWeaponController
{
    [SerializeField] SwordController m_SwordController;

    public virtual void EnableWeaponHitBox()
    {
        m_SwordController.EnableHitBox();
    }

    public virtual void DisableWeaponHitBox()
    {
        m_SwordController.DisableHitBox();
    }
    
    public abstract bool IsEnemy(CharacterController other);

    // 필요하다면 공용 데이터(HP, 스탯 등)도 여기에
    // 공용 피격 처리, 공용 Move/Attack 등도 가능
}