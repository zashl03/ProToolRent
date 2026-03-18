using FluentValidation;
using MediatR;
//using ProToolRent.Api.Middleware;
using ProToolRent.Application.Commands.CreateTool;
using ProToolRent.Application.Common.Behaviors;
using ProToolRent.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
