using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SGCAv1;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;

namespace SGCAIntegrationTest.API
{
    public class CaixaAPITest
    {
        private readonly HttpClient _client;

        public CaixaAPITest()
        {
            var server = new TestServer(new WebHostBuilder().UseEnvironment("Development").UseStartup<Startup>());
            _client = server.CreateClient();

        }

        [Theory]
        [InlineData("GET")]
        public async Task CaixaGetAllTestAsync(string method)
        {
            //arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/caixa");

            //act
            var response = await _client.SendAsync(request);

            //assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Theory]
        [InlineData("GET", 1)]
        public async Task CaixaGetStatusTestAsync(string method, int? id=null)
        {
            //arrange
            var request = new HttpRequestMessage(new HttpMethod(method), $"/api/caixa/{id}/getstatus");

            //act
            var response = await _client.SendAsync(request);

            //assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Theory]
        [InlineData("POST")]
        public async Task CaixaPostAllTestAsync(string method)
        {
            //arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/caixa");

            //act
            var response = await _client.SendAsync(request);

            //assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
    }
}
