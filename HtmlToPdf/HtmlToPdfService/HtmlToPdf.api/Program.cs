using HtmlToPdf.api.Core.DependencyInjections;
using HtmlToPdf.core.Helpers;
using HtmlToPdf.core.Interfaces;
using HtmlToPdf.core;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using PuppeteerSharp; // Added for HeaderNames

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

const string ToAllowSpecificOrigins = "_allowSpecificOrigins"; // Added constant

// Chained IServiceCollection Services
builder.Services
    .AddAppConfiguration(builder.Configuration)
    .AddDataServices();


builder.Services.AddSingleton<ITransformHelper, TransformHelper>();
builder.Services.AddSingleton<IHtmlToPdfService, HtmlToPdfService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: ToAllowSpecificOrigins,
                      policyBuilder =>
                      {
                          policyBuilder.WithOrigins("http://localhost:5173")
                                 .AllowAnyMethod()
                                 .AllowAnyHeader()
                                 .AllowCredentials();
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