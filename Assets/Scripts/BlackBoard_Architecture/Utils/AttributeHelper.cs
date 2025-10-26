using System;
using System.Reflection;

namespace BlackboardSystem
{
    public static class AttributeHelper{
        private static readonly Type attributeType = typeof(BBKeyAttribute);
        private static readonly Type blackboardInjectKeyType = typeof(BlackboardInjectKeyAttribute);
        public static void InjectKeyTo(this Blackboard source, object target, BindingFlags flags = BindingFlags.Default){
            FieldInfo[] fields = target.GetType().GetFields(flags);
            
            foreach(var field in fields){
                if(field.IsDefined(attributeType, inherit: false) == false) continue;
                BBKeyAttribute attribute = (BBKeyAttribute)field.GetCustomAttribute(attributeType);
                if(attribute != null){
                    bool hasValue = source.TryGetKey(attribute.KeyName, out BlackboardKey bkey);

                    if(hasValue == false){
                        UnityEngine.Debug.LogWarning($"Key {attribute.KeyName} not found in blackboard");
                        continue;
                    }
                    field.SetValue(target, bkey);
                }
            }
        }

        public static void InjectBlackboardTo(this Blackboard source, object target, BindingFlags flags = BindingFlags.Default){
            FieldInfo[] fields = target.GetType().GetFields(flags);
            
            foreach(var field in fields){
                if(field.IsDefined(blackboardInjectKeyType, inherit: false) == false) continue;
                BlackboardInjectKeyAttribute attribute = (BlackboardInjectKeyAttribute)field.GetCustomAttribute(blackboardInjectKeyType);
                if(attribute != null){
                    field.SetValue(target, source);
                }
            }
        }
    }
}