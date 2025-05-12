using HtmlToPdf.api.Core.DependencyInjections;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers; // Added for HeaderNames

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

const string ToAllowSpecificOrigins = "_allowSpecificOrigins"; // Added constant

// Chained IServiceCollection Services
builder.Services
    .AddAppConfiguration(builder.Configuration)
    .AddDataServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: ToAllowSpecificOrigins,
                      policyBuilder =>
                      {
                          policyBuilder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .WithHeaders(HeaderNames.ContentType, "ApimSubscriptionKey");
                      });
});

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

app.UseCors(ToAllowSpecificOrigins); // Added to apply CORS policy

app.UseAuthorization();

app.MapControllers();

app.Run();