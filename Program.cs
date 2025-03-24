using System.Text;
using System.Text.Json.Serialization;
using E_commerce.Data;
using E_commerce.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

var connString = builder.Configuration.GetConnectionString("Ecommerce");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Allow Angular frontend
                  .AllowAnyMethod()  // Allow GET, POST, PUT, DELETE
                  .AllowAnyHeader()  // Allow all headers
                  .AllowCredentials(); // Allow cookies if needed
        });
});
builder.Services.AddSqlite<EcommerceContext>(connString);
builder.Services.AddControllers();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddSingleton<PaymentGatewayService>();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddAuthentication(options => // Without this, ASP.NET Core won’t know how to handle authentication and will allow any request to access protected endpoints.
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Specifies that JWT Bearer authentication is used as the default authentication method.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Ensures that unauthorized users receive a JWT authentication challenge when trying to access protected endpoints.
}).AddJwtBearer(option =>
{
    option.RequireHttpsMetadata = false; // Allows jwt to work over http too instead of just https if false
    option.SaveToken = true; //Saves the token in the current authentication context (useful when accessing it in middleware or other parts of the app).
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Ensures the token was issued by a trusted Issuer.
        ValidateAudience = true, // Ensures the token is intended for this API.
        ValidateLifetime = true, // Ensures the token hasn’t expired.
        ValidateIssuerSigningKey = true, // Ensures the token was signed with a valid secret key.
        ValidIssuer = jwtSettings["Issuer"], // Ensures the token was signed with a valid secret key.
        ValidAudience = jwtSettings["Audience"], // The expected Audience (from appsettings.json).
        IssuerSigningKey = new SymmetricSecurityKey(secretKey) // The secret key used to validate the token signature.
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// <snippet_UseWebSockets>
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);
// </snippet_UseWebSockets>

//Enables authorization and authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAngularApp");

await app.MigrateDbAsync();

app.Run();