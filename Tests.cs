using System;
using Xunit;

namespace ElasticEmail {
    public class Tests {
        /* 
            ASYNCHRONOUS TESTS (Processing Data) 
        */

        [Fact]
        public async Task TestResolveReal() {
            string[] domains = {"gmail.com", "aol.com", "hotmail.com"};
            List<string> records = await Task.Run(() => Program.ResolveIP(domains, ""));

            Assert.True(records.Count() > 0);
        }

        [Fact]
        public async Task TestResolveFake() {
            string[] domains = {"gmail", "aol", "hotmail"};
            List<string> records = await Task.Run(() => Program.ResolveIP(domains, ""));

            Assert.False(records.Count() > 0);
        }

        [Fact]
        public async Task TestDnsReal() {
            string[] domains = {"gmail.com"};
            List<string> records = await Task.Run(() => Program.ResolveIP(domains, "8.8.8.8"));

            Assert.True(records.Count() > 0);
        }

        [Fact]
        public async Task TestDnsFake() {
            string[] domains = {"gmail.com"};
            List<string> records = await Task.Run(() => Program.ResolveIP(domains, "123"));

            Assert.True(records.Count() > 0);
        }

        [Fact]
        public async Task TestOutputReal() {
            string[] domains = {"gmail.com"};
            List<string> records = await Task.Run(() => Program.Output("../../../output.txt", domains));

            Assert.True(records.Count() > 0);
        }

        [Fact]
        public async Task TestOutputFake() {
            string[] domains = {"gmail"};
            List<string> records = await Task.Run(() => Program.Output("../../../output.txt", domains));

            Assert.False(records.Count() > 0);
        }

        [Fact]
        public async Task TestInputReal() {
            List<string> records = await Task.Run(() => Program.Input("../../../domains.txt"));

            Assert.True(records.Count() > 0);
        }

        [Fact]
        public async Task TestInputFake() {
            List<string> records = await Task.Run(() => Program.Input("./does_not_exist.txt"));

            Assert.False(records.Count() > 0);
        }

        /* 
            SYNCHRONOUS TESTS (Error Handling) 
        */

        [Fact]
        public void CheckDomainReal() {
            Assert.True(Program.CheckDomain("google.com"));
        }

        [Fact]
        public void CheckDomainFake() {
            Assert.False(Program.CheckDomain("google"));
        }

        [Fact]
        public void CheckDnsReal() {
            Assert.True(Program.CheckDns("8.8.8.8"));
        }

        [Fact]
        public void CheckDnsFake() {
            Assert.False(Program.CheckDns("123"));
        }
    }
}