namespace TrapSystem{
    public sealed partial class TrapSystemInitializer : System.IDisposable{
        private static TrapSystemInitializer s_instance;
        public static bool IsInitialized => s_instance != null; 

        private readonly CentralizedTrapSystem m_internalSystem;

        // hide the default constructor
        private TrapSystemInitializer(){
            m_internalSystem = new CentralizedTrapSystem();
        } 

        public static TrapSystemInitializer CreateSystem(){
            s_instance ??= new TrapSystemInitializer();
            return s_instance;
        }

        public TrapSystemInitializer WithVictimConverter(IVictimConverter victimConverter){
            m_internalSystem._victimConvertHelper = victimConverter;
            return this;
        }

        public void Dispose()
        {
            m_internalSystem?.Dispose();
        }
    }
}