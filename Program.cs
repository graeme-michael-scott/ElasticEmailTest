/*
    Elastic Email Test
    Author: Graeme Scott
    Date: October 15, 2022
    Description: Resolve IP addresses from DNS MX record(s) for the provided domain(s) 
*/

// Required to use LookupClient
using DnsClient;

// Required to use IPEndPoint & IPAddress
using System.Net; 

// Required to use StringBuilder
using System.Text;

// Required to use Regex
using System.Text.RegularExpressions;

namespace ElasticEmail {
    // Create class for Program
    class Program {
        /* 
            INIT FUNCTIONS (Main Method & Checking Flags) 
        */

        // Init Program
        static async Task Main(string[] args) {
            // Create a string array for each part of the unicode for the Start Program emoji
            string[] startEmoji = {"1F6A6"};

            // Write a line to the console to notify the user that the program has now begun its task
            Alert("Starting Program", startEmoji);

            // Parse the user input and determine which function will be run depending on the flag
            await CheckFlags(args);

            // Create a string array for each part of the unicode for the End Program emoji
            string[] endEmoji = {"1F6D1"};

            // Once all async functions have completed, write a line to the console to notify the 
            // user that the program has completed its task
            Alert("End Program", endEmoji);
        }

        // Parse the user input and determine which function will be run depending on the flag
        public static async Task CheckFlags(string[] args) {
            // Check to see if any arguements have been entered by the user
            if (args.Length == 0) {
                // If there are no arguements provided, throw the default error message
                Error();
            
            // If the first arguement's first character is a dash "-", 
            // We can safetly assume the user is using a flag and contunue
            // to parse the arguement to figure out which function will be run depending on the flag
            } else if (args[0].Substring(0, 1) == "-") {
                // Set the first arguement to a string called "flag" so it is easier to read
                string flag = args[0];

                // Check if there is an arguement provided after the initial flag arguement
                if (args.Length > 1) {

                    // Create a switch/case statement to determine which function to run
                    // depending on the flag
                    switch(flag) {

                        // If flag = "-dns"
                        case "-dns":
                            // The "-dns" flag requires two arguements (a DNS server and a domain)
                            // after the initial flag arguement to run, so we need to make sure 
                            // the "args" array is atleast 3 items in size
                            // (Item #1: the flag itself | Item #2: the DNS server | Item 3: the domain)
                            if (args.Length > 2) {
                                // Create an array of strings to store the specified domain
                                // because ResolveIP expects an array of strings as the first arguement
                                string[] dnsContents = {args[2]};

                                // Run the ResolveIP function and await for the result
                                await ResolveIP(dnsContents, args[1]);
                            
                            // If the "-dns" flag only has one arguement after the initial flag arguement
                            } else {
                                // Throw the default error message
                                Error();
                            }

                            break;

                        // If flag = "-input"
                        case "-input":    
                            // Run the Input function and await for the result
                            await Input(args[1]);
                            break;
                        
                        // If flag = "-output"
                        case "-output":
                            // The "-output" flag requires two arguements (a file path and a domain)
                            // after the initial flag arguement to run, so we need to make sure 
                            // the "args" array is atleast 3 items in size
                            // (Item #1: the flag itself | Item #2: the file path | Item 3: the domain)
                            if (args.Length > 2) {
                                // Create an array of strings to store the specified domain
                                // because ResolveIP expects an array of strings as the second arguement
                                string[] outputContents = {args[2]};

                                // Run the Output function and await for the result
                                await Output(args[1], outputContents);
                            // If the "-output" flag only has one arguement after the initial flag arguement
                            } else {
                                // Throw the default error message
                                Error();
                            }

                            break;
                    }

                // If there is no arguement provided after the initial flag arguement,
                // throw an appropriate error message depending on the flag
                } else {
                    // Create a switch/case statement to determine which error to write
                    // to the console depending on the flag
                    switch(flag) {
                        // If flag = "-dns"
                        case "-dns":
                            // Throw an appropriate Error Message for the lack of DNS server
                            // and domain in the arguements
                            Error("Please enter a DNS server followed by a URL");
                            break;

                        // If flag = "-input"
                        case "-input":   
                            // Throw an appropriate Error Message for the lack of a
                            // file path in the arguements 
                            Error("Please enter a file path to read from");
                            break;
                        
                        // If flag = "-output"
                        case "-output":
                            // Throw an appropriate Error Message for the lack of a
                            // file path in the arguements 
                            Error("Please enter a file path to export to");
                            break;
                    }
                }
            // If there is atleast one arguement and the first arguement is not a flag
            } else {
                // If we have made it this far, the user did not enter either
                // the '-dns', '-input' or '-output' flags as the first arguement
                // Which means we can safely pass all of the arguements to the ResolveIP function
                await ResolveIP(args, "");
            }
        }

        /* 
            SYNCHRONOUS FUNCTIONS (Error Handling, Alerts & Emoji Parsing) 
        */

        // Notify user with an Error message
        public static void Error(string error = "Please enter a URL") {
            // Create a string array for each part of the unicode for the Error Message emoji
            string[] emoji = {"2757"};
            
            // Write blank line to create spacing and make console easier to read
            Console.WriteLine();

            // Write provided emoji to the console
            Console.Write(PrintEmoji(emoji));

            // Change text color to reflect Error (red)
            Console.ForegroundColor = ConsoleColor.Red;

            // Write provided Error Message to the console
            Console.Write(error);
            
            // Create a new blank line because "Write()" prints to the console inline
            Console.WriteLine();

            // Reset the color so when other Console.WriteLine() functions are user,
            // the text appears as the default White colour
            Console.ResetColor();
        }

        // Notify user with an Success message
        public static void Success(string success) {
            // Create a string array for each part of the unicode for the Success Message emoji
            string[] emoji = {"2705"};
            
            // Write blank line to create spacing and make console easier to read
            Console.WriteLine();

            // Write provided emoji to the console
            Console.Write(PrintEmoji(emoji));

            // Change text color to reflect Success (green)
            Console.ForegroundColor = ConsoleColor.Green;

            // Write provided Success Message to the console
            Console.Write(success);
            
            // Create a new blank line because "Write()" prints to the console inline
            Console.WriteLine();

            // Reset the color so when other Console.WriteLine() functions are user,
            // the text appears as the default White colour
            Console.ResetColor();
        }

        // Notify user with an Alert message
        public static void Alert(string alert, string[] emoji) {
            // Write blank line to create spacing and make console easier to read
            Console.WriteLine();

            // Check if an emoji was provided
            if (emoji.Length > 0) {
                // Write provided emoji to the console
                Console.Write(PrintEmoji(emoji));
            }

            // Change text color to reflect Alert (yellow)
            Console.ForegroundColor = ConsoleColor.Yellow;

            // Write provided Alert Message to the console
            Console.Write(alert);

            // Create a new blank line because "Write()" prints to the console inline
            Console.WriteLine();

            // Reset the color so when other Console.WriteLine() functions are user,
            // the text appears as the default White colour
            Console.ResetColor();
        }

        // Build an emoji from unicode to display in each of the above message functions
        public static string PrintEmoji(string[] unicodeParts) {
            // Create a new StringBuilder() to assemble all unicodeParts to create an emoji
            StringBuilder unicodeBuilder = new StringBuilder();

            // Loop through each provided unicode part
            foreach (string unicodePart in unicodeParts) {
                // Append each unicode part to the StringBuilder
                unicodeBuilder.Append(char.ConvertFromUtf32(Convert.ToInt32(unicodePart,16)));
            }

            // Convert the StringBuilder to a regular string and add a space " " 
            // so that the emojis have some breathing room between the message
            return unicodeBuilder.ToString() + " ";
        }

        // Check to see if the domain provided follows the correct formatting
        public static bool CheckDomain(string domain) {
            // Create a regular expression to match on domain names
            Regex domainRegex = new Regex(@"^((?!-))(xn--)?[a-z0-9][a-z0-9-_]{0,61}[a-z0-9]{0,1}\.(xn--)?([a-z0-9\-]{1,61}|[a-z0-9-]{1,30}\.[a-z]{2,})$"); 

            // Return true or false depending on if the domain name provided matches the regex
            return domainRegex.IsMatch(domain);
        }

        // Check to see if the DNS server provided follows the correct formatting
        public static bool CheckDns(string dns) {
            // Create a regular expression to match on IP Addresses
            Regex dnsRegex = new Regex(@"^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");;

            // Return true or false depending on if the IP Address provided matches the regex
            return dnsRegex.IsMatch(dns);
        }

        /* 
            ASYNCHRONOUS FUNCTIONS (Processing Data) 
        */

        // Read a file from the specified file path and resolve each domain
        // The file must be formatted so that each domain is on its own separate line
        public static async Task<List<string>> Input(string path) {
            // Create a string array to store each individual line from the specified file
            string[] readFile = {};

            // If the provided file path exists
            if (File.Exists(path)) {
                // ReadAllLines() from the file path and store each line in an array
                readFile = File.ReadAllLines(path);

                // Write a message to the console to tell the user that the file was read successfully
                Success($"The file \"{path}\" was read successfully!");

            // If the provided file path does not exists
            } else {
                // Notify the user that the file path does not exist
                Error($"The file \"{path}\" does not exist");
            }

            // Send our new array to be resolved by the ResolveIP function and
            // return the resolved IP string List once each domain has been resolved
            return await ResolveIP(readFile, "");
        }

        // Loop through a string array of domains and 
        // resolve each one concurrently using multithreading
        public static async Task<List<string>> ResolveIP(string[] args, string dns) {
            // Create a new string List to store the results of each resolved IP
            List<string> records = new List<string>();

            // Create a new string List to store any exceptions thrown by each unresolved IP
            List<string> errors = new List<string>();
            
            // Run an async task 
            await Task.Run(async () => {

                // Loop through a string array of domains concurrently using multithreading
                await Parallel.ForEachAsync(args, new ParallelOptions(), async (url, ct) => {
                    // If the domain name matches the domain name regex
                    if (CheckDomain(url)) {
                        // Create a new LookupClient() to get more information about the specified domain
                        LookupClient lookup = new LookupClient();

                        // Check if the "dns" arguement is not empty
                        if (dns != "") {
                            // If the specified DNS Server matches against the DNS regex
                            if (CheckDns(dns)) {
                                // Create a new LookupClient that will resolve the specified
                                // domain to the specified DNS server
                                //
                                // By setting the port to 0, the service provider will assign 
                                // an available port number between 1024 and 5000.
                                lookup = new LookupClient(IPAddress.Parse(dns));
                            // If the specified DNS Server does not match against the DNS regex
                            } else {
                                // Add an error to the errors list notifying the user that
                                // the IP Address they provided does not match agaisnt the DNS regex
                                errors.Add($"\"{dns}\" is not a valid IP Address and will not resolve to any of the provided domains");
                            }
                        }

                        // Await while LookupClient() runs a query to 
                        // collect MX information about the specified domain
                        var result = await lookup.QueryAsync(url, QueryType.MX);

                        // If LookUpClient does not return null when running a query on the specificed domain name
                        if (result != null) {
                            // Create a variable to store the MX Records so they are eaiser to reference
                            var mxRecords = result.Answers?.MxRecords();

                            // If the domain resolved successfully 
                            if (mxRecords != null && mxRecords.Count() > 0) {
                                // Loop through all MX records for this specific domain name
                                foreach (var mxRecord in mxRecords) {
                                    // Create a new StringBuilder() to 
                                    // assemble the report of resolved IPs
                                    StringBuilder reportBuilder = new StringBuilder();

                                    // Append each part of the report to the StringBuilder
                                    //
                                    // Format example: 
                                    // gmail.com MX preference = XXX, 
                                    // mail exchanger = alt3.gmail-smtp-in.l.google.com 8.8.8.8
                                    reportBuilder.Append(mxRecord.DomainName.Original)
                                                .Append(" MX Preference = ")
                                                .Append(mxRecord.Preference)
                                                .Append(", mail exchanger = ")
                                                .Append(mxRecord.Exchange.Original)
                                                .Append(" ")
                                                .Append(Dns.GetHostAddresses(mxRecord.Exchange.Original)[0]);
                                    
                                    // Add our new reportBuilder to the records string List
                                    records.Add(reportBuilder.ToString());
                                }
                            // If there are no MX records for this specific domain name
                            } else {
                                // Add an error to the errors list notifying the user that
                                // the domain they provided was unable to resolve and will be ignored
                                errors.Add($"\"{url}\" was unable to resolve and will be ignored");
                            }
                        }
                    // If the domain name does not match the domain name regex
                    } else {
                        // Add an error to the errors list notifying the user that
                        // the domain they provided does not match agaisnt the doman name regex
                        errors.Add($"\"{url}\" is not a valid domain and will be ignored");
                    }
                });
            });

            // Loop through the errors list 
            foreach(string error in errors) {
                // Notify the user of the specifc error thrown
                Error(error);
            }

            // Notify the user with a message signalling that this is the beginning of the 
            // results of their resolved IPs
            Success("Results:");

            // Check if any IPs resolved successfully and were stored in the records string List
            if (records.Count() > 0) {
                // Loop through each MX Record
                foreach(string record in records) {
                    // Write each record to the console
                    Console.WriteLine(record);
                }

                // Notify the user that IPs were resolved
                Success("IPs have been resolved!");

            // If no IPs resolved successfully and the records string List is empty
            } else {
                // Notify the user that no IPs were resolved
                Error("No IPs were resolved :(");
            }

            // Return the resolved IP string List
            return records;
        }

        // Output all resolved domains to the specified file path
        public static async Task<List<string>> Output(string fileName, string[] contents) {
            // Run the ResolveIP function and await for the result
            List<string> resolved = await ResolveIP(contents, "");
            
            // Write each string from the contents List to the path specified in fileName
            await File.WriteAllLinesAsync(fileName, resolved);

            // Write message to console to tell user that the file was output successfully
            Success($"The file \"{fileName}\" was exported successfully!");

            // Return the resolved IP string List
            return resolved;
        }
    }
}