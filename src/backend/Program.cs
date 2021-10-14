using Ardalis.GuardClauses;
using backend.Options;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AzureAdOptions>(builder.Configuration.GetSection(AzureAdOptions.SectionName));

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection(AzureAdOptions.SectionName));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<IConfidentialClientApplication>(provider =>
{
    var azureAdOptions = provider.GetService<IOptions<AzureAdOptions>>()!.Value;
    Guard.Against.Null(azureAdOptions, "IOptions<AzureAdOptions>");

    return ConfidentialClientApplicationBuilder.Create(azureAdOptions.ClientId)
        .WithClientSecret(azureAdOptions.ClientSecret)
        .WithAuthority(AzureCloudInstance.AzurePublic, Guid.Parse(azureAdOptions.TenantId))
        .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
