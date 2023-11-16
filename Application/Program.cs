using API.Middlewares;
using Application.Extensions;
using Business.Interfaces;
using Business.Services;
using Data.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Security.Helpers;
using Security.Interfaces;
using Security.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

const string outputTemplate =
    "[{Timestamp:HH:mm:ss} {Level:u3}] [{RequestId}] {Message:lj}{NewLine}{Exception}";

// Add services to the container.
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")!,
        optionsBuilder => { optionsBuilder.MigrationsHistoryTable("__Migrations", DatabaseContext.Schema); }));

builder.Services.AddDbContext<IdentityDatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")!,
        optionsBuilder => { optionsBuilder.MigrationsHistoryTable("__Migrations", IdentityDatabaseContext.Schema); }));


builder.Services.AddSerilog(outputTemplate);
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddRabbitMq(builder.Configuration);
builder.Services.AddSwaggerExtension();
builder.Services.AddControllers();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.UseSwaggerExtension();

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<RequestIdMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();