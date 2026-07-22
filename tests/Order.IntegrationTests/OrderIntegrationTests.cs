using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Order.API.Features.CreateOrder;
using Order.API.Infrastructure;
using Order.API.Models;
using Shared.Events;
using System.Net;
using System.Net.Http.Json;

namespace Order.IntegrationTests
{
    public class OrderIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public OrderIntegrationTests(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            _factory = factory;
        }

        public class OrderResponse
        {
            public Guid Id { get; set; }
        }

        [Fact]
        public async Task CreateOrder_WithSameRequestId_ShouldReturnExistingOrder()
        {
            var requestId = Guid.NewGuid();

            _httpClient.DefaultRequestHeaders.Add("X-Request-Id", requestId.ToString());

            var orderItemsDto = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto(Guid.Parse("018f4e2a-7b91-7c3d-8a2f-000000000001"), 1) 
            };

            var orderCommand = new CreateOrderCommand(
                Guid.Empty, 
                "Wall Street", 
                "New-York", 
                "USA",
                "23310",
                orderItemsDto);

            var first = await _httpClient.PostAsJsonAsync("/api/orders", orderCommand);
            first.EnsureSuccessStatusCode();
            var firstResponse = await first.Content.ReadFromJsonAsync<OrderResponse>();

            var second = await _httpClient.PostAsJsonAsync("/api/orders", orderCommand);
            second.EnsureSuccessStatusCode();
            var secondResponse = await second.Content.ReadFromJsonAsync<OrderResponse>();

            Assert.Equal(firstResponse!.Id, secondResponse!.Id);

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            var count = await dbContext.Orders.CountAsync(o => o.RequestId == requestId);

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CreateOrder_ConcurrentRequestsWithSameRequestId_ShouldCreateOnlyOneOrder()
        {
            var requestId = Guid.NewGuid();

            var orderItemsDto = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto(Guid.Parse("018f4e2a-7b91-7c3d-8a2f-000000000001"), 1)
            };

            var orderCommand = new CreateOrderCommand(
                Guid.Empty,
                "Wall Street",
                "New-York",
                "USA",
                "23310",
                orderItemsDto);

            var client1 = _factory.CreateClient();
            client1.DefaultRequestHeaders.Add("X-Request-Id", requestId.ToString());

            var client2 = _factory.CreateClient();
            client2.DefaultRequestHeaders.Add("X-Request-Id", requestId.ToString());

            var task1 = client1.PostAsJsonAsync("api/orders", orderCommand);
            var task2 = client2.PostAsJsonAsync("api/orders", orderCommand);

            var responses = await Task.WhenAll(task1, task2);

            foreach (var r in responses)
                r.EnsureSuccessStatusCode();

            var bodies = await Task.WhenAll(
                responses.Select(r => r.Content.ReadFromJsonAsync<OrderResponse>()));

            Assert.Equal(bodies[0]!.Id, bodies[1]!.Id);

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            var count = await dbContext.Orders.CountAsync(o => o.RequestId == requestId);

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CreateOrder_WithInvalidData_ShouldReturnError()
        {
            var emptyList = new List<CreateOrderItemDto>();

            var invalidDataOrder = new CreateOrderCommand(
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "",
                "",
                "",
                "",
                emptyList);

            var orderRequest = await _httpClient.PostAsJsonAsync("api/orders", invalidDataOrder);
            var response = orderRequest.StatusCode;

            Assert.Equal(HttpStatusCode.BadRequest, response);
        }
    }
}
