namespace TrapSystem{
    public interface ITrapEventSource{
        event System.Action<Trap> TrapRegisteredEvent;
        event System.Action<Trap> TrapUnregisteredEvent;
    }
}