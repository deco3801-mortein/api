namespace Mortein;

/// <summary>
/// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync
/// which is the actual Lambda function entry point.
/// </summary>
public class LambdaEntryPoint : Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
{
    /// <summary>
    /// The builder has configuration, logging and Amazon API Gateway already configured. The
    /// startup class needs to be configured in this method using the UseStartup() method.
    /// </summary>
    ///
    /// <param name="builder">The IWebHostBuilder to configure.</param>
    protected override void Init(IWebHostBuilder builder)
    {
        builder.UseStartup<Startup>();
    }
}
