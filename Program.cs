/*
    Elastic Email Test
    Author: Graeme Scott
    Date: October 15, 2022
    Description: Resolve IP addresses from DNS MX record(s) for the provided domain(s) 
*/

using DnsClient;
using System.Net; 
using System.Text;
using System.Text.RegularExpressions;

namespace ElasticEmail {
    class Program {
        /* 
            INIT FUNCTIONS (Main Method & Checking Flags) 
        */

        static async Task Main(string[] args) {
            string[] startEmoji = {"1F6A6"};
            Alert("Starting Program", startEmoji);

            await CheckFlags(args);

            string[] endEmoji = {"1F6D1"};
            Alert("End Program", endEmoji);
        }

        public static async Task CheckFlags(string[] args) {
            if (args.Length == 0) {
                Error();
            } else if (args[0].Substring(0, 1) == "-") {
                string flag = args[0];

                if (args.Length > 1) {
                    switch(flag) {
                        case "-dns":
                            if (args.Length > 2) {
                                string[] dnsContents = {args[2]};
                                await ResolveIP(dnsContents, args[1]);
                            } else {
                                Error();
                            }

                            break;

                        case "-input":    
                            await Input(args[1]);
                            break;
                        
                        case "-output":
                            if (args.Length > 2) {
                                string[] outputContents = {args[2]};
                                await Output(args[1], outputContents);
                            } else {
                                Error();
                            }

                            break;
                    }
                } else {
                    switch(flag) {
                        case "-dns":
                            Error("Please enter a DNS server followed by a URL");
                            break;

                        case "-input":   
                            Error("Please enter a file path to read from");
                            break;

                        case "-output":
                            Error("Please enter a file path to export to");
                            break;
                    }
                }
            } else {
                await ResolveIP(args, "");
            }
        }

        /* 
            SYNCHRONOUS FUNCTIONS (Error Handling, Alerts & Emoji Parsing) 
        */

        public static void Error(string error = "Please enter a URL") {
            string[] emoji = {"2757"};

            Console.WriteLine();
            Console.Write(PrintEmoji(emoji));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(error);
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void Success(string success) {
            string[] emoji = {"2705"};

            Console.WriteLine();
            Console.Write(PrintEmoji(emoji));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(success);
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void Alert(string alert, string[] emoji) {
            Console.WriteLine();

            if (emoji.Length > 0) {
                Console.Write(PrintEmoji(emoji));
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(alert);
            Console.WriteLine();
            Console.ResetColor();
        }

        public static string PrintEmoji(string[] unicodeParts) {
            StringBuilder unicodeBuilder = new StringBuilder();

            foreach (string unicodePart in unicodeParts) {
                unicodeBuilder.Append(char.ConvertFromUtf32(Convert.ToInt32(unicodePart,16)));
            }

            return unicodeBuilder.ToString() + " ";
        }

        public static bool CheckDomain(string domain) {
            Regex domainRegex = new Regex(@"^((?!-))(xn--)?[a-z0-9][a-z0-9-_]{0,61}[a-z0-9]{0,1}\.(xn--)?([a-z0-9\-]{1,61}|[a-z0-9-]{1,30}\.[a-z]{2,})$"); 
            return domainRegex.IsMatch(domain);
        }

        public static bool CheckDns(string dns) {
            Regex dnsRegex = new Regex(@"^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");;
            return dnsRegex.IsMatch(dns);
        }

        /* 
            ASYNCHRONOUS FUNCTIONS (Processing Data) 
        */

        public static async Task<List<string>> Input(string path) {
            string[] readFile = {};

            if (File.Exists(path)) {
                readFile = File.ReadAllLines(path);
                Success($"The file \"{path}\" was read successfully!");
            } else {
                Error($"The file \"{path}\" does not exist");
            }

            return await ResolveIP(readFile, "");
        }

        public static async Task<List<string>> ResolveIP(string[] args, string dns) {
            List<string> records = new List<string>();
            List<string> errors = new List<string>();
   
            await Task.Run(async () => {
                await Parallel.ForEachAsync(args, new ParallelOptions(), async (url, ct) => {
                    if (CheckDomain(url)) {
                        LookupClient lookup = new LookupClient();

                        if (dns != "") {
                            if (CheckDns(dns)) {
                                lookup = new LookupClient(IPAddress.Parse(dns));
                            } else {
                                errors.Add($"\"{dns}\" is not a valid IP Address and will not resolve to any of the provided domains");
                            }
                        }

                        var result = await lookup.QueryAsync(url, QueryType.MX);

                        if (result != null) {
                            var mxRecords = result.Answers?.MxRecords();

                            if (mxRecords != null && mxRecords.Count() > 0) {
                                foreach (var mxRecord in mxRecords) {
                                    StringBuilder reportBuilder = new StringBuilder();

                                    reportBuilder.Append(mxRecord.DomainName.Original)
                                                .Append(" MX Preference = ")
                                                .Append(mxRecord.Preference)
                                                .Append(", mail exchanger = ")
                                                .Append(mxRecord.Exchange.Original)
                                                .Append(" ")
                                                .Append(Dns.GetHostAddresses(mxRecord.Exchange.Original)[0]);
                                    
                                    records.Add(reportBuilder.ToString());
                                }
                            } else {
                                errors.Add($"\"{url}\" was unable to resolve and will be ignored");
                            }
                        }
                    } else {
                        errors.Add($"\"{url}\" is not a valid domain and will be ignored");
                    }
                });
            });

            foreach(string error in errors) {
                Error(error);
            }

            Success("Results:");

            if (records.Count() > 0) {
                foreach(string record in records) {
                    Console.WriteLine(record);
                }

                Success("IPs have been resolved!");
            } else {
                Error("No IPs were resolved :(");
            }

            return records;
        }

        public static async Task<List<string>> Output(string fileName, string[] contents) {
            List<string> resolved = await ResolveIP(contents, "");
            
            await File.WriteAllLinesAsync(fileName, resolved);

            Success($"The file \"{fileName}\" was exported successfully!");

            return resolved;
        }
    }
}