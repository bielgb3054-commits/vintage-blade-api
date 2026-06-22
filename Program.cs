using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("NetlifyPolicy", policy =>
    {
        policy
            .WithOrigins("https://preeminent-pie-74f9fb.netlify.app")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("NetlifyPolicy");

var bookings = new List<Booking>();

app.MapGet("/api/bookings", () =>
{
    var ordered = bookings
        .OrderBy(b => b.Data)
        .ThenBy(b => b.Horario)
        .ToList();

    return Results.Ok(ordered);
});

app.MapPost("/api/bookings", (BookingCreateDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.NomeCliente) ||
        string.IsNullOrWhiteSpace(dto.WhatsApp) ||
        string.IsNullOrWhiteSpace(dto.Servico) ||
        string.IsNullOrWhiteSpace(dto.Data) ||
        string.IsNullOrWhiteSpace(dto.Horario))
    {
        return Results.BadRequest("Todos os campos são obrigatórios.");
    }

    var conflict = bookings.Any(b =>
        b.Data.Equals(dto.Data, StringComparison.OrdinalIgnoreCase) &&
        b.Horario.Equals(dto.Horario, StringComparison.OrdinalIgnoreCase));

    if (conflict)
    {
        return Results.BadRequest("Já existe um agendamento nesse dia e horário.");
    }

    var booking = new Booking
    {
        Id = Guid.NewGuid(),
        NomeCliente = dto.NomeCliente.Trim(),
        WhatsApp = dto.WhatsApp.Trim(),
        Servico = dto.Servico.Trim(),
        Data = dto.Data.Trim(),
        Horario = dto.Horario.Trim()
    };

    bookings.Add(booking);

    return Results.Ok(booking);
});

app.Run();

public class Booking
{
    public Guid Id { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string WhatsApp { get; set; } = string.Empty;
    public string Servico { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
}

public class BookingCreateDto
{
    public string NomeCliente { get; set; } = string.Empty;
    public string WhatsApp { get; set; } = string.Empty;
    public string Servico { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
}