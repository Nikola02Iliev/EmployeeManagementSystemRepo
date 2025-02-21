using Microsoft.EntityFrameworkCore;
using ServerLibrary.Context;
using ServerLibrary.Helpers;
using ServerLibrary.Repositorities.Contracts;
using ServerLibrary.Repositorities.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Connection to the database
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection not found!")));

//Dependency Injection
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();

//JWT
builder.Services.Configure<JwtSection>(builder.Configuration.GetSection("JwtSection"));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
