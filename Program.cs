var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

// Lista na memória para guardar os agendamentos temporariamente
var agendamentos = new List<Agendamento>();

app.MapGet("/", () => "API da Vintage Blade Barber está voando!");

// Rota de Agendamento com validação de duplicidade
app.MapPost("/agendar", (Agendamento novoAgendamento) =>
{
    if (string.IsNullOrEmpty(novoAgendamento.Cliente))
    {
        return Results.BadRequest("Nome do cliente é obrigatório.");
    }
    
    if (string.IsNullOrEmpty(novoAgendamento.Data))
    {
        return Results.BadRequest("A data e o horário são obrigatórios.");
    }

    // 🧠 VALIDAÇÃO INTELEGENTE: Procura se já existe alguém no mesmo horário
    bool horarioOcupado = agendamentos.Any(a => a.Data == novoAgendamento.Data);

    if (horarioOcupado)
    {
        // Se o horário já existir, rejeita o agendamento e avisa o site
        return Results.BadRequest("Este horário já está reservado por outro cliente. Escolha outro momento!");
    }
    
    // Se estiver livre, adiciona com sucesso
    agendamentos.Add(novoAgendamento);
    return Results.Ok(new { mensagem = "Agendamento realizado com sucesso!" });
});

app.Run();

public record Agendamento(string Cliente, string Data);
