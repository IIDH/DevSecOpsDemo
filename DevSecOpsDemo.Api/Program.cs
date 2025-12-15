var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoint 1: GET /api/health
// Retorna un JSON indicando que el servicio está "ok"
app.MapGet("/api/health", () =>
{
    return Results.Ok(new { status = "ok" });
})
.WithName("GetHealth")
.WithOpenApi();

// Endpoint 2: POST /api/suma
// Recibe dos números enteros y retorna su suma
app.MapPost("/api/suma", (SumaRequest? request) =>
{
    // Validar que el body no sea nulo
    if (request == null)
    {
        return Results.BadRequest(new { error = "El body de la solicitud no puede ser nulo" });
    }

    // Calcular y retornar la suma
    var resultado = request.A + request.B;
    return Results.Ok(new { resultado });
})
.WithName("PostSuma")
.WithOpenApi();

app.Run();

// DTO para el endpoint de suma
record SumaRequest(int A, int B);

// Hacer la clase Program accesible para las pruebas de integración
public partial class Program { }
