var builder = WebApplication.CreateBuilder(args);

// 🛠️ ISSO RESOLVE O ERRO: Ativa a liberação de segurança (CORS)
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

// Ativa a regra no servidor
app.UseCors();

// Lista na memória para guardar os agendamentos temporariamente
var agendamentos = new List<Agendamento>();

// Rota de teste para ter certeza que o link funciona
app.MapGet("/", () => "API da Vintage Blade Barber está voando!");

// Rota que o botão do seu site chama para salvar
app.MapPost("/agendar", (Agendamento novoAgendamento) =>
{
    if (string.IsNullOrEmpty(novoAgendamento.Cliente))
    {
        return Results.BadRequest("Nome do cliente é obrigatório.");
    }
    
    agendamentos.Add(novoAgendamento);
    return Results.Ok(new { mensagem = "Agendamento realizado com sucesso!" });
});

app.Run();

// Estrutura do agendamento
public record Agendamento(string Cliente, string Data);
