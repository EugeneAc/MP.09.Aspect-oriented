namespace DocumentBuilderservice
{
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;

    using DynProxy;

    public class ComponentRegistration : IRegistration
	{
		public void Register(IKernelInternal kernel)
		{
			kernel.Register(
			   Component.For<MethodLoggerInterceptor>()
				   .ImplementedBy<MethodLoggerInterceptor>());
			kernel.Register(
				Component.For<IPdfDocumentBuilder>()
						 .ImplementedBy<PdfDocumentBuilder>()
						 .Interceptors(InterceptorReference.ForType<MethodLoggerInterceptor>()).Anywhere);
		}
	}
}
