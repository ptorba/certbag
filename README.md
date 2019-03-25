# CertBag

This is a simple webapp to generate and keep track of client TLS certificates.

# Why?

There are situations when you need to generate PKCS#12 certificates by hand
and then send them via email to some users to allow them access to your other application.

All you need is a CA key and cert packed into a PFX file.

This is a terribly simple app but it solves a specific issue I have at work.

# How to run

Certbag is available as a docker image [here](https://hub.docker.com/r/ptorba/certbag).
There is one environment variable:
* `CA_PATH`: path to ca key and crt bundled in a PFX file. You can make one with `openssl pkcs12 -export -out Cert.p12 -in cert.pem -inkey key.pem`

The database (SQLite) that is used is mounted at `/db/cerbag.db` - you can put it in a volume or bind mount if you wish.

```
docker run -v /ca/path:/mnt/ca -e CA_PATH=/mnt/ca/ca.pfx -p 80:80 ptorba/certbag
```

# Features

* Keep track of the certificates that were created and their expiration date
* Generate new PKCS#12 files at will for commonNames known to CertBag
* import existing PKCS#12 certificate metadata

WARNING: Certbag does not store the actual certificate - only the CommonName and expiration date.

# TODO

* Handle multiple Certificate Authorities
* generate CRL for known certs
* (Maybe) notifications/reminders about expiring certs via email/slack


# Deployment

Put it behind a HTTPS proxy, please.
