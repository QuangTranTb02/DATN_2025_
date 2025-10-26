namespace TrapSystem{
    public interface IVictim{
        void TakeDamage(int damage);
        void PlayAnimation(int animationId);
        void Die();
    }
}