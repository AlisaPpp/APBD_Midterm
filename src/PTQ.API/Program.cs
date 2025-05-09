using System.Text.Json;
using System.Text.Json.Nodes;
using PTQ.Repositories;
using PTQ.Application;
using PTQ.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PersonalDatabase");

builder.Services.AddTransient<IQuizRepository>(_ => new QuizRepository(connectionString));
builder.Services.AddTransient<IQuizService, QuizService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/quizzes", (IQuizService quizService) =>
{
    var quizzes = quizService.GetAllQuizzes();
    return Results.Ok(quizzes);
});

app.MapGet("/api/quizzes/{id}", (IQuizService quizService, int id) =>
{
    var quiz = quizService.GetSpecificQuiz(id);
    if (quiz == null)
        return Results.NotFound($"Quiz with id {id} not found");
    return Results.Ok(quiz);
});

app.MapPost("/api/quizzes", async (IQuizService quizService, HttpRequest request) =>
{
    using (var reader = new StreamReader(request.Body))
    {
        string rawJson = await reader.ReadToEndAsync();
        var json = JsonNode.Parse(rawJson);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        var quizInfo = JsonSerializer.Deserialize<AddANewQuizDTO>(json, options);

        if (quizInfo != null)
        {
            bool success = quizService.AddQuiz(quizInfo);
            if (!success)
                return Results.Created();
            else
                return Results.StatusCode(500);
        }
    }
    return Results.BadRequest();
});


app.Run();
