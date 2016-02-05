# EppLib.NET 

[![Build status](https://ci.appveyor.com/api/projects/status/dxtxp3tjjgne87ar)](https://ci.appveyor.com/project/AdemarGonzalez/epplib-net)

EppLib.NET is a .NET library implementing the Extensible Provisioning Protocol (EPP)

HOW TO: https://github.com/ademar/EppLib.NET/wiki

EPP (defined in RFC 5730) is a widely adopted protocol used to comunicate between a domain registrar and the different domain name registries* to provision and manage domain names, host names and contact details. 

EppLib.NET provides a library that makes easy for registrars to interact with registries implementing the EPP protocol.

Our library is a complete implementation of the EPP specification. Have a look at the How to section for code examples and recipes.

* We now include EPP extensions for CIRA (the .CA registry), Nominet (the .UK registry) and IIS (the .SE registry).

# NuGet

```bash
PM> Install-Package EppLib
```
