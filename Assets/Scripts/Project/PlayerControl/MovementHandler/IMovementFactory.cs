using UnityEngine;

namespace Project.PlayerControl.Movement{

    public interface IMovementHandlerFactory{
        IPlayerMovementHandler CreateMovementHandler(PlayerMovementStats stats, Transform target);
    }

    public abstract class ScriptableMovementHandlerFactory : ScriptableObject, IMovementHandlerFactory{
        public abstract IPlayerMovementHandler CreateMovementHandler(PlayerMovementStats stats, Transform target);
    }
}