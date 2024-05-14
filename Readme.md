FORMAT: 1A

# Cloudflare DDNS API Proxy: 
## *A simple GET proxy to put DNS Update requests on Cloudflare*

## Problem 

If you own or manage a TP-Link Omada router, you can configure DDNS using its Services config. It supports a bunch of well known providers plus a custom implementation.
It uses a request with this structure:
```
http://[USERNAME]:[PASSWORD]@someddnsprovider.com/update?hostname=[DOMAIN]&myip=[IP]
```
The router then injects the placeholders into the route and sends the request on the configured time.

However, providers like Cloudflare does not allows GET methods on their API, plus it expects a JSON Payload and token authentication [See the documentation here](https://developers.cloudflare.com/api/operations/dns-records-for-a-zone-update-dns-record)

## Solution

This has a minimal webapi the proxies this API with a GET method that can be used on an Omada router or any other device that works the same way.

This webapi can be deployed standalone, containerized [^1] or using a Lambda (which is what I did...) [^2]
## API

## Cloudflare DNS 

```/cloudflare/{ZoneId}/dns_records/{DnsRecordId}{?ZoneId}&DnsRecordId,Ip,Domain,Proxy,Type}```

### Update DNS Recod [GET]

+ Parameters

	+ ZoneId: (string) Your Cloudflare Zone Id
	+ DnsRecordId: (string) Your Record Id. Can be retrieved from Cloudflare's API. [^3]
	+ Ip: (string) The IP that will be registered
	+ Domain: (string) Domain Name to Register
	+ Proxy: (string) Cloudflare's proxy (enable or disable)
	+ Type: (string) Type of DNS Record (A, AAAA, CNET, etc)

+ Response (status from cloudflare) (application/json)

	{...from cloudflare respose...}

+ Headers

	+ Authorization: (string) BASIC Authorization supported. For Cloudflare, create an API Key, put anything on the username and send the key as Password. [^4]
## Disclaimer

Use As-Is, at your own risk. MIT License. 

[^1]: TODO
[^2]: Sure, I could have done everything in API gateway, but hindsight is 2020...
[^3]: Maybe one day I'll just get it from the API, using the name only...
[^4]: This is awful but Omada Routers expects username:password. I could put the key in a secrets store on AWS, but I prefer the flexibility of this approach (easier to setup)
