using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using PomodoroTimer.Api;
using PomodoroTimer.Api.Entities;
using PomodoroTimer.Api.Persistence;
using PomodoroTimer.Api.Services;
using System.Text;

var allowSpecificOrigins = "allowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();
services.AddPersistence(options =>
{
    var dbSettings = builder.Configuration
        .GetSection("DbSettings")
        .Get<DbSettings>();

    var connectionStringBuilder = new NpgsqlConnectionStringBuilder
    {
        ConnectionString = dbSettings.ConnectionString,
        Username = dbSettings.Username,
        Password = dbSettings.Password
    };

    options.ConnectionString = connectionStringBuilder.ConnectionString;
    options.HasConsoleLogging = builder.Environment.IsDevelopment();
});

services.AddHttpContextAccessor();

services.AddCors(o =>
    o.AddPolicy(
        allowSpecificOrigins,
        p => p
            .WithOrigins("https://localhost:5001")
            .AllowAnyHeader()
            .AllowAnyMethod()));

services
    .AddIdentityCore<PomodoroUser>()
    .AddEntityFrameworkStores<PomodoroTimerDbContext>();

services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration
            .GetSection("JwtSettings")
            .Get<JwtSettings>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecurityKey))
        };
    });

services.AddScoped<TokenGenerator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(allowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();
