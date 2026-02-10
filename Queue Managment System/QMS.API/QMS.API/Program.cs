using Microsoft.EntityFrameworkCore;
using Npgsql;
using QMS.Application.Services;
using QMS.Core.Enums;
using QMS.Core.Interfaces;
using QMS.Core.Interfaces.Repositories;
using QMS.Infrastructure.Context;
using QMS.Infrastructure.Repositories;
using QMS.UI.Hubs;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<QMSDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"),
        npgsqlOptions =>
        {
            npgsqlOptions.MapEnum<CustomerType>();
            npgsqlOptions.MapEnum<ServiceType>();
            npgsqlOptions.MapEnum<TicketStatus>();
            npgsqlOptions.MapEnum<DeskType>();
        });
});

NpgsqlConnection.GlobalTypeMapper.MapEnum<CustomerType>();
NpgsqlConnection.GlobalTypeMapper.MapEnum<ServiceType>();
NpgsqlConnection.GlobalTypeMapper.MapEnum<TicketStatus>();
NpgsqlConnection.GlobalTypeMapper.MapEnum<DeskType>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IDeskRepository, DeskRepository>();

builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IDeskService, DeskService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://localhost:7192")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapHub<QueueHub>("/queueHub");

app.Run();
