using YAXLib;
using YAXLib.Enums;
using YAXLib.Options;

namespace Twileloop.SessionGuard.Engines
{
    public static class XmlHelper
    {
        public static string Serialize<T>(T obj)
        {
            var serializer = new YAXSerializer(typeof(T), new SerializerOptions
            {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Ignore
            });
            return serializer.Serialize(obj);
        }

        public static T Deserialize<T>(string xml)
        {
            var serializer = new YAXSerializer(typeof(T), new SerializerOptions
            {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Ignore
            });
            return (T)serializer.Deserialize(xml);
        }
    }
}
