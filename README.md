# Mortein API

In order to authenticate to the AWS MQTT broker, pass a client certificate and private key into
the following command to generate the required PFX file:

```
openssl pkcs12 -export -out api.pfx -inkey private.pem.key -in certificate.pem.crt
```
