namespace Project.InputSystem{
    public interface IInputHandler{
        void OnRegisteredTo(InputSystemEventStorage eventStorage);
        void OnUnregisteredTo(InputSystemEventStorage eventStorage);
    }
}