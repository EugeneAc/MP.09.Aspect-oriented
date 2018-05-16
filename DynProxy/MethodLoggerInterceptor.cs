using Castle.DynamicProxy;
using Newtonsoft.Json;
using System.IO;

namespace DynProxy
{
	public class MethodLoggerInterceptor : IInterceptor
	{
		public void Intercept(IInvocation invocation)
		{
			invocation.Proceed();
			using (StreamWriter sw = new StreamWriter(@"DynamicProxyMethodLog.txt", append: true))
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.NullValueHandling = NullValueHandling.Ignore;
				serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				serializer.Serialize(writer, new { Name = TrySerialize(invocation.Method.Name), Time = System.DateTime.Now, Args = TrySerialize(invocation.Arguments) });
                sw.Write(";"); // Add Delimeter for deserialization
				serializer.Serialize(writer, new { ReturnValue = TrySerialize(invocation.ReturnValue) });
			    sw.Write(";"); // Add Delimeter for deserialization
            }
			
		}

		public object TrySerialize (object obj)
		{
			try
			{
				return JsonConvert.SerializeObject(obj);
			}
			catch (System.Exception)
			{
				return new { value = "Not serializable" };
			}
			
		}
	}
}
