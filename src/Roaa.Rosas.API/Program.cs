using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Logging;
using Roaa.Rosas.API.Configurations;
using Roaa.Rosas.Auditing;
using Roaa.Rosas.Authorization;
using Roaa.Rosas.Common;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.HealthChecks.UI;
using Roaa.Rosas.Education.API.Middlewares;
using Roaa.Rosas.Framework.Configurations;
using Roaa.Rosas.Infrastructure.Persistence.SeedData.Identity;
using Roaa.Rosas.Infrastructure.Persistence.SeedData.IdentityServer4;
using Roaa.Rosas.Infrastructure.Persistence.SeedData.Management;
using Roaa.Rosas.RequestBroker;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Logging.AddDebug();
builder.Logging.AddConsole();

// TODO : allow ouer origins only
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPublicPolicy",
                          policy =>
                          {
                              policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader();
                          });
});


// Add services to the container. 
builder.Services.AddControllers(options => options.AddAudit());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddCommonConfigurations();
builder.Services.AddRequestBroker();
builder.Services.AddRosasServiceConfigurations(builder.Configuration, builder.Environment);
builder.Services.AddAuthorizationConfigurations(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region HealthChecks

builder.Services.AddHealthChecks()
                .AddIdentityServiceHealthChecks();
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProductionEnvironment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    IdentityModelEventSource.ShowPII = true;
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseCors("CorsPublicPolicy");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseIdentityServer();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = ResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false
});


app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var idS4initialiserI = scope.ServiceProvider.GetRequiredService<IdentityServerConfigurationDbInitialiser>();
    var identityInitialiser = scope.ServiceProvider.GetRequiredService<IdentityDbInitialiser>();
    var managementDbInitialiser = scope.ServiceProvider.GetRequiredService<ManagementDbInitialiser>();
    await identityInitialiser.MigrateAsync();
    await identityInitialiser.SeedAsync();
    await idS4initialiserI.MigrateAsync();
    await idS4initialiserI.SeedAsync();
    await managementDbInitialiser.MigrateAsync();
    await managementDbInitialiser.SeedAsync();
}

app.Run();
