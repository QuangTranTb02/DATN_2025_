namespace Project.PlayerControl.Movement
{
    public interface IPlayerMovementHandler
    {
        void ChangeTarget(UnityEngine.Transform target);
        void Update();
        void FixedUpdate();
        void ChangeDirection(UnityEngine.Vector3 direction);
        void OnJump(bool value);
    }
}