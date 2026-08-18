using UnityEngine;

public class EnemyAnimationEventRelay : MonoBehaviour
{
    private Enemy enemy;

    public void Initialize(Enemy owner)
    {
        enemy = owner;
    }

    public void PlayMeleeAttackSoundFromAnimation()
    {
        if (enemy == null)
        {
            enemy = GetComponentInParent<Enemy>();
        }

        if (enemy == null) return;

        enemy.PlayMeleeAttackSoundFromAnimation();
    }
}
