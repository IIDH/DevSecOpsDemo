namespace DevSecOpsDemo.Tests;

/// <summary>
/// Pruebas de integración para el endpoint POST /api/suma
/// </summary>
public class SumaEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SumaEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region Pruebas Requeridas

    /// <summary>
    /// PRUEBA REQUERIDA 2: POST /api/suma con números válidos - caso exitoso
    /// Verificar código HTTP correcto y resultado de la suma
    /// </summary>
    [Fact]
    public async Task PostSuma_WithValidNumbers_ReturnsOkWithCorrectSum()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new { A = 5, B = 3 };

        // Act
        var response = await client.PostAsJsonAsync("/api/suma", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, "debe retornar HTTP 200 para una solicitud válida");

        var result = await response.Content.ReadFromJsonAsync<SumaResponse>();
        result.Should().NotBeNull();
        result!.Resultado.Should().Be(8, "5 + 3 debe ser igual a 8");
    }

    /// <summary>
    /// PRUEBA REQUERIDA 3: POST /api/suma con body nulo - caso inválido
    /// Verificar código HTTP de error y mensaje de error
    /// </summary>
    [Fact]
    public async Task PostSuma_WithNullBody_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var content = new StringContent("", Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/suma", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, "debe retornar HTTP 400 para body nulo");

        var errorContent = await response.Content.ReadAsStringAsync();
        errorContent.Should().Contain("error", "la respuesta debe contener un mensaje de error");
        
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Error.Should().NotBeNullOrEmpty("debe incluir un mensaje de error descriptivo");
    }

    #endregion

    #region Pruebas Adicionales - Casos Exitosos

    /// <summary>
    /// Prueba adicional: Suma con números negativos
    /// </summary>
    [Fact]
    public async Task PostSuma_WithNegativeNumbers_ReturnsCorrectSum()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new { A = -10, B = 25 };

        // Act
        var response = await client.PostAsJsonAsync("/api/suma", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SumaResponse>();
        result!.Resultado.Should().Be(15, "-10 + 25 debe ser igual a 15");
    }

    /// <summary>
    /// Prueba adicional: Suma con valores cero
    /// </summary>
    [Fact]
    public async Task PostSuma_WithZeroValues_ReturnsZero()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new { A = 0, B = 0 };

        // Act
        var response = await client.PostAsJsonAsync("/api/suma", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SumaResponse>();
        result!.Resultado.Should().Be(0, "0 + 0 debe ser igual a 0");
    }

    /// <summary>
    /// Prueba adicional: Suma con números grandes
    /// </summary>
    [Fact]
    public async Task PostSuma_WithLargeNumbers_ReturnsCorrectSum()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new { A = 1000000, B = 2000000 };

        // Act
        var response = await client.PostAsJsonAsync("/api/suma", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SumaResponse>();
        result!.Resultado.Should().Be(3000000, "1000000 + 2000000 debe ser igual a 3000000");
    }

    /// <summary>
    /// Prueba adicional: Suma con números positivos y negativos mezclados
    /// </summary>
    [Fact]
    public async Task PostSuma_WithMixedPositiveNegative_ReturnsCorrectSum()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new { A = 100, B = -50 };

        // Act
        var response = await client.PostAsJsonAsync("/api/suma", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SumaResponse>();
        result!.Resultado.Should().Be(50, "100 + (-50) debe ser igual a 50");
    }

    /// <summary>
    /// Prueba adicional: Verificar que retorna Content-Type JSON
    /// </summary>
    [Fact]
    public async Task PostSuma_ReturnsJsonContentType()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new { A = 1, B = 1 };

        // Act
        var response = await client.PostAsJsonAsync("/api/suma", request);

        // Assert
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    #endregion

    #region Pruebas Adicionales - Casos de Error

    /// <summary>
    /// Prueba adicional: POST con JSON inválido/malformado
    /// </summary>
    [Fact]
    public async Task PostSuma_WithInvalidJson_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var content = new StringContent("{invalid json}", Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/suma", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, "debe retornar HTTP 400 para JSON inválido");
    }

    /// <summary>
    /// Prueba adicional: POST sin Content-Type header
    /// </summary>
    [Fact]
    public async Task PostSuma_WithoutContentType_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var content = new StringContent("{\"A\":5,\"B\":3}");

        // Act
        var response = await client.PostAsync("/api/suma", content);

        // Assert
        // Puede retornar 400 o 415 (Unsupported Media Type) dependiendo de la configuración
        response.IsSuccessStatusCode.Should().BeFalse("debe fallar sin Content-Type apropiado");
    }

    #endregion

    // DTOs para deserializar las respuestas
    private record SumaResponse(int Resultado);
    private record ErrorResponse(string Error);
}
