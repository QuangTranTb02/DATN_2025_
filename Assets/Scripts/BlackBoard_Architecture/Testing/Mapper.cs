namespace BlackboardSystem.Testing{
    [System.Serializable]
    public class Mapper : IBlackboardMapper
    {
        public TestData data;

        public void MapDataTo(Blackboard blackboard)
        {
            blackboard.RegisterKey<string>("testString", data.testString);
            blackboard.RegisterKey<float>("testFloat", data.testFloat);
            blackboard.RegisterKey<int>("testInt", data.testInt);
        }
    }

    [System.Serializable]
    public class TestData{
        public string testString;
        public float testFloat;
        public int testInt;
    }
}