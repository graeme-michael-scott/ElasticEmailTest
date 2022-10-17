using System;
using Xunit;

namespace ElasticEmail {
    public class Tests {
        /* 
            ASYNCHRONOUS TESTS (Processing Data) 
        */

        [Fact]
        // Test Resolving IP's with real domain names
        public async Task TestResolveReal() {
            // Create string array to store real domain names
            string[] domains = {"gmail.com", "aol.com", "hotmail.com"};

            // Await the ResolveIP function and save records in a string List
            List<string> records = await Task.Run(() => Program.ResolveIP(domains, ""));

            // Returns true because "gmail.com", "aol.com", "hotmail.com"
            // all match the domain name regex
            Assert.True(records.Count() > 0);
        }

        [Fact]
        // Test Resolving IP's with fake domain names
        public async Task TestResolveFake() {
            // Create string array to store fake domain names
            string[] domains = {"gmail", "aol", "hotmail"};

            // Await the ResolveIP function and save records in a string List
            List<string> records = await Task.Run(() => Program.ResolveIP(domains, ""));

            // Returns false because "gmail", "aol", "hotmail"
            // do not match the domain name regex
            Assert.False(records.Count() > 0);
        }

        [Fact]
        // Test DNS Server IP resolution with a real IP Address
        public async Task TestDnsReal() {
            // Create string array to store domain names
            string[] domains = {"gmail.com"};

            // Await the ResolveIP function and save records in a string List
            List<string> records = await Task.Run(() => Program.ResolveIP(domains, "8.8.8.8"));

            // Returns true because "8.8.8.8" matches the DNS regex
            Assert.True(records.Count() > 0);
        }

        [Fact]
        // Test DNS Server IP resolution with a fake IP Address
        public async Task TestDnsFake() {
            // Create string array to store domain names
            string[] domains = {"gmail.com"};

            // Await the ResolveIP function and save records in a string List
            List<string> records = await Task.Run(() => Program.ResolveIP(domains, "123"));

            // Returns true because despite "123" not matching the DNS regex,
            // the program defaults to using any IP address if there is no match
            Assert.True(records.Count() > 0);
        }

        [Fact]
        // Test outputting a file with real domains
        public async Task TestOutputReal() {
            // Create string array to store real domain names
            string[] domains = {"gmail.com"};

            // Await the Output function and save records in a string List
            List<string> records = await Task.Run(() => Program.Output("../../../output.txt", domains));

            // Returns false because "gmail.com" matches the domain name regex
            Assert.True(records.Count() > 0);
        }

        [Fact]
        // Test outputting a file with fake domains
        public async Task TestOutputFake() {
            // Create string array to store fake domain names
            string[] domains = {"gmail"};

            // Await the Output function and save records in a string List
            List<string> records = await Task.Run(() => Program.Output("../../../output.txt", domains));

            // Returns false because "gmail" does not match the domain name regex
            Assert.False(records.Count() > 0);
        }

        [Fact]
        // Test inputting a real file
        public async Task TestInputReal() {
            // Await the Input function and save records in a string List
            List<string> records = await Task.Run(() => Program.Input("../../../domains.txt"));

            // Returns true because "domains.txt" does exist in the root of this project folder
            Assert.True(records.Count() > 0);
        }

        [Fact]
        // Test inputting a fake file
        public async Task TestInputFake() {
            // Await the Input function and save records in a string List
            List<string> records = await Task.Run(() => Program.Input("./does_not_exist.txt"));

            // Returns true because "does_not_exist.txt" does exist in this project folder
            Assert.False(records.Count() > 0);
        }

        /* 
            SYNCHRONOUS TESTS (Error Handling) 
        */

        [Fact]
        // Test a real domain
        public void CheckDomainReal() {
            // Returns true because "google" does match the domain name regex
            Assert.True(Program.CheckDomain("google.com"));
        }

        [Fact]
        // Test a fake domain
        public void CheckDomainFake() {
            // Returns false because "google" doesn't match the domain name regex
            Assert.False(Program.CheckDomain("google"));
        }

        [Fact]
        // Test a real IP Address
        public void CheckDnsReal() {
            // Returns true because "8.8.8.8" does match the IP Address regex
            Assert.True(Program.CheckDns("8.8.8.8"));
        }

        [Fact]
        // Test a fake IP Address
        public void CheckDnsFake() {
            // Returns false because "123" doesn't match the IP Address regex
            Assert.False(Program.CheckDns("123"));
        }
    }
}