using CustomerOnboarding.Api.Application.Interfaces;
using CustomerOnboarding.Api.Application.Services;
using CustomerOnboarding.Api.Domain.External;
using CustomerOnboarding.Api.Infrastructure.Persistence;
using CustomerOnboarding.Api.Infrastructure.Repositories;
using CustomerOnboarding.Api.Mapping;
using CustomerOnboarding.Api.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Polly;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<IOtpService, OtpServiceImplementation>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Http client for external bank API. BaseUrl points directly to the bank endpoint in config.
builder.Services.AddHttpClient<IBankClient, BankClient>(client =>
{
    var url = builder.Configuration["AlatApi:BaseUrl"] ?? string.Empty;
    client.BaseAddress = new Uri(url);
    client.Timeout = TimeSpan.FromSeconds(10);
})
.AddTransientHttpErrorPolicy(p =>
    p.WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) }));

builder.Services.AddScoped<BankService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();
app.Run();


public partial class Program { } 