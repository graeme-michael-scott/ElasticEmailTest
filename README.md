![Elastic Email](logo.gif)

# Goal

Resolve IP addresses from DNS MX record(s) for the provided domain(s)

### Requirements

1. Allow the user to input a single domain from the command line
   `yourApp hotmail.com`

2. Allow the user to input multiple domains from the command line and resolve each concurrently (using multiple threads)
   `yourApp gmail.com hotmail.com aol.com`

3. Allow the user to input a domain list from a file (each domain must be on a separate line inside of the file)
   `yourApp -input domains.txt`

4. Allow the user to input which DNS server will be used to resolve the domain
   `yourApp -dns 8.8.8.8 hotmail.com`

5. Allow the user to build a report for all resolved IPs which have info about mail exchanger, MX preference, and the DNS server granting answer

   ```
   gmail.com MX preference = XXX,
   mail exchanger = alt3.gmail-smtp-in.l.google.com 8.8.8.8

   gmail.com MX preference = XXX,
   mail exchanger = alt2.another-smtp.google.com 1.1.1.1

   ...
   ```

6. Allow the user to export an output file of the report
   `yourApp -output ips.txt`

7. Add automated tests in VS test project or dotnet script .csx
