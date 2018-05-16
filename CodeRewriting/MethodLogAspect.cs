using Newtonsoft.Json;
using PostSharp.Aspects;
using System;
using System.IO;

namespace CodeRewriting
{
	[Serializable]
	public class MethodLogAspect : OnMethodBoundaryAspect
	{
		private string _logFileName = "CodeRewritingMethodLog.txt";
		public override void OnEntry(MethodExecutionArgs args)
		{
			var method = args.Method;
			var arguments = args.Arguments;
			Serialize(new { Name = method.Name, Time = DateTime.Now, Args = arguments });
		}

		public override void OnSuccess(MethodExecutionArgs args)
		{
			Serialize(new { ReturnValue = args.ReturnValue });
		}

		private void Serialize(object obj)
		{
			using (StreamWriter sw = new StreamWriter(_logFileName, append: true))
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				JsonSerializer serializer = new JsonSerializer();
				try
				{
					serializer.Serialize(writer, obj);
				}
				catch (Exception)
				{

					serializer.Serialize(writer, "Not serializable");
				}
				sw.Write(";");  // Add Delimeter for deserialization
			}
		}
	}
}

