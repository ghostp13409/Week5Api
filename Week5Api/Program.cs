using Microsoft.EntityFrameworkCore;
using Week5Api.Data;
using Week5Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb")
    );

var app = builder.Build();

// in-memory state for usage and accounts
const string ApiKeyHeader = "X-Api-Key";
const string SecretKey = "MY_SECRET_KEY_123";
var clientUsage = new Dictionary<string, int>();
var accounts = new Dictionary<int, Account>
{
    {1, new Account { Id = 1, Name = "Alice", Balance = 100m }},
    {2, new Account { Id = 2, Name = "Bob", Balance = 50m }}
};

// global exception handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new ErrorMessage { error = "ServerError", message = "An unexpected error occurred." });
    }
});

// simple API key authentication middleware
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var key) || key != SecretKey)
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new ErrorMessage { error = "Unauthorized", message = "Missing or invalid API key." });
        return;
    }
    await next();
});

app.MapGet("/hello", () => "Your API has been updated through CI and CD");

app.MapGet("/error", () =>
{
    // intentional exception
    int x = 0;
    var y = 1 / x;
    return "won't reach";
});

app.MapGet("/usage", (HttpContext context) =>
{
    var key = context.Request.Headers[ApiKeyHeader].ToString();
    if (string.IsNullOrEmpty(key))
    {
        context.Response.StatusCode = 400;
        return Results.Json(new ErrorMessage { error = "InvalidParameter", message = "API key required" });
    }
    if (!clientUsage.ContainsKey(key)) clientUsage[key] = 0;
    clientUsage[key]++;
    return Results.Json(new { clientId = key, callCount = clientUsage[key] });
});

app.MapPost("/transfer", (AccountTransfer request) =>
{
    if (!accounts.ContainsKey(request.FromId) || !accounts.ContainsKey(request.ToId))
    {
        return Results.BadRequest(new { status = "Failed", error = "AccountNotFound" });
    }
    var from = accounts[request.FromId];
    var to = accounts[request.ToId];
    if (from.Balance < request.Amount)
    {
        return Results.Json(new { status = "Failed", error = "InsufficientFunds" });
    }
    // simulate transaction
    from.Balance -= request.Amount;
    to.Balance += request.Amount;
    return Results.Json(new { status = "Success" });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();



//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
