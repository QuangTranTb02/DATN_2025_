namespace Project.PlayerControl.Movement
{
    public interface IMovementController{
        UnityEngine.Transform target {get; set;}
        UnityEngine.Vector3 velocity {get;set;}
        void SetVelocityX(float value);
        void SetVelocityY(float value);
        void SetVelocityZ(float value);
    }
}