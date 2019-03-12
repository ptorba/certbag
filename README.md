# CertBag

This is a simple webapp to generate and keep track of client TLS certificates.

# Why?

There are situations when you need to generate PKCS#12 certificates by hand
and then send them via email to some users to allow them access to your other application.

All you need is a CA key and cert packed into a PFX file.


# How to run

Certbag is available as a docker image here.
There is one environment variable:
* `CA_PATH`: path to ca key and crt bundled in a PFX file. You can make one with `openssl pkcs12 -export -out Cert.p12 -in cert.pem -inkey key.pem`

```
docker run -v /ca/path:/mnt/ca -e CA_PATH=/mnt/ca/ca.pfx -p 80:80 ptorba/certbag
```

# Features

* Keep track of the certificates that were created

# Deployment

Put it behind a HTTPS proxy, please.
