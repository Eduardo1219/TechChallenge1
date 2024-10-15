using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Prometheus;
using System.Reflection;
using System.Text.Json.Serialization;
using TechChallenge1.DomainInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Tech Challenge",
        Description = "Tech Challenge FIAP",
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddFluentValidationAutoValidation().AddValidatorsFromAssembly(typeof(Program).Assembly, ServiceLifetime.Transient);

builder.Services.AddInfraestructure(builder.Configuration);

builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

var app = builder.Build();


var counter = Metrics.CreateCounter("fiaptc", "Counts requests to the fiaptc",
    new CounterConfiguration
    {
        LabelNames = new[] { "method", "endpoint" }
    });

app.Use((context, next) =>
{
    counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
    return next();
});


app.UseMetricServer(settings => settings.EnableOpenMetrics = false);
app.UseHttpMetrics();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();
app.UseHttpMetrics(options =>

{
    options.RequestDuration.Enabled = true;
    options.RequestCount.Enabled = true;

});
app.MapHealthChecks("/healthz");


app.MapControllers();


app.Run();
