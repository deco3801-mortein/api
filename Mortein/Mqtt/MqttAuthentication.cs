using System.Security.Cryptography.X509Certificates;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace Mortein.Mqtt;

/// <summary>
/// Provides authentication for the MQTT service.
/// </summary>
public static class MqttAuthentication
{
    private static readonly string credentialBucketName = "api-mqtt-certificate";
    private static readonly string objectName = "api.pfx";

    private static readonly AmazonS3Client s3 = new(
        Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
        Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"),
        RegionEndpoint.APSoutheast2
    );

    private static async Task<GetObjectResponse?> GetCredentialFileObject()
    {
        return await s3.GetObjectAsync(credentialBucketName, objectName);
    }

    /// <summary>
    /// Return the <see cref="X509Certificate2"/> with which to authenticate to the MQTT broker.
    /// </summary>
    /// 
    /// <returns>The <see cref="X509Certificate2"/> with which to authenticate to the MQTT broker</returns>
    public static async Task<X509Certificate2> GetAwsMqttCertificate()
    {
        var credentialFileObject = (await GetCredentialFileObject())!;

        byte[] credentialBytes;

        using var credentialFileStream = credentialFileObject.ResponseStream;
        using var memoryStream = new MemoryStream();
        {
            credentialFileStream.CopyTo(memoryStream);
            credentialBytes = memoryStream.ToArray();
        }

        return new(credentialBytes);
    }
}
