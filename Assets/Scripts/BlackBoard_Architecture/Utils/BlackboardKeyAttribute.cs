namespace BlackboardSystem
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class BBKeyAttribute : System.Attribute
    {
        public readonly string KeyName;
        public BBKeyAttribute(string keyName)
        {
            KeyName = keyName;
        }
    }
}