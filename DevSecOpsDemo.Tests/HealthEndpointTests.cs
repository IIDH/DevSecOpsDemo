namespace DevSecOpsDemo.Tests;

/// <summary>
/// Pruebas de integración para el endpoint GET /api/health
/// </summary>
public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// PRUEBA REQUERIDA 1: Verificar que GET /api/health devuelve HTTP 200 y status "ok"
    /// </summary>
    [Fact]
    public async Task GetHealth_ReturnsOk_WithCorrectStatus()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, "el endpoint de health debe retornar HTTP 200");
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"status\"", "la respuesta debe contener la propiedad 'status'");
        content.Should().Contain("\"ok\"", "el status debe ser 'ok'");

        // Verificar el objeto JSON completo
        var healthResponse = await response.Content.ReadFromJsonAsync<HealthResponse>();
        healthResponse.Should().NotBeNull();
        healthResponse!.Status.Should().Be("ok", "el status debe ser exactamente 'ok'");
    }

    /// <summary>
    /// Prueba adicional: Verificar que retorna Content-Type JSON
    /// </summary>
    [Fact]
    public async Task GetHealth_ReturnsJsonContentType()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/health");

        // Assert
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    /// <summary>
    /// Prueba adicional: Verificar idempotencia (múltiples llamadas retornan el mismo resultado)
    /// </summary>
    [Fact]
    public async Task GetHealth_IsIdempotent()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Realizar múltiples llamadas
        var response1 = await client.GetAsync("/api/health");
        var response2 = await client.GetAsync("/api/health");
        var response3 = await client.GetAsync("/api/health");

        var content1 = await response1.Content.ReadAsStringAsync();
        var content2 = await response2.Content.ReadAsStringAsync();
        var content3 = await response3.Content.ReadAsStringAsync();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        response3.StatusCode.Should().Be(HttpStatusCode.OK);

        content1.Should().Be(content2);
        content2.Should().Be(content3);
    }

    // DTO para deserializar la respuesta
    private record HealthResponse(string Status);
}
