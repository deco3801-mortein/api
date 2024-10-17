# Mortein API

In order to authenticate to the AWS MQTT broker, pass a client certificate and private key into
the following command to generate the required PFX file:

The environment required to build and run this application is defined by this repository's
dev container [configuration file](./.devcontainer/devcontainer.json).

Either open this repository in a dev container using a
[supporting tool](https://containers.dev/supporting) or replicate the required environment manually.

## Deployment

Create an AWS account and authenticate to the account from your working environment.

Create an IAM role named `lambda-role` with the following AWS managed permissions policies attached:

-   AmazonS3ReadOnlyAccess
-   AWSLambdaBasicExecutionRole

Create an RDS database instance using version 16.4 of the PostgreSQL engine.

Create an AWS IoT certificate with the following policies attached:

-   iot:Connect
-   iot:Publish
-   iot:Receive
-   iot:Subscribe

Generate a PFX certificate from the AWS IoT private key and certificate using the following command:

```
openssl pkcs12 -export -out api.pfx -inkey private.pem.key -in certificate.pem.crt
```

Create an S3 bucket named `api-mqtt-certificate` and upload the generated `api.pfx` file.

From within the `Mortein` directory, deploy the API using the `dotnet lambda deploy-serverless`
command, passing the `--template-parameters` argument with the parameters defined in the
_Parameters_ section of the `serverless-template.json` file. These parameters come from the hostname
and credentials of your RDS instance, as well as the domain name of the default AWS IoT domain
configuration. The MQTT client ID can be any string.
