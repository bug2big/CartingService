using Carting.Data.Common;
using Carting.Data.Repositories.Carts;
using Carting.Extensions;
using Carting.Mappers;
using Carting.Models;
using Carting.Services.Carts;
using Carting.Services.MessageConsumerService;
using Carting.Settings;
using Carting.Swagger;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Mappers
builder.Services.AddTransient<CartMapper>();
// LiteDB
builder.Services.Configure<LiteDbSettings>(builder.Configuration.GetSection("LiteDbSettings"));
builder.Services.AddTransient<ILiteDbContextProvider, LiteDbContextProvider>();
// Repositories
builder.Services.AddTransient<ICartRepository, CartRepository>();
// Services
builder.Services.AddTransient<ICartService, CartService>();
builder.Services.AddTransient<IMessageConsumerService, MessageConsumerService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwtBearerOptions =>
{
    jwtBearerOptions.MapInboundClaims = false;
    jwtBearerOptions.Authority = configuration["Authentication:Authority"];
    jwtBearerOptions.Audience = configuration["Authentication:Audience"];
    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:ClientSecret"]!)),
        ValidateLifetime = true,
        RoleClaimType = JwtClaimTypes.Role
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(swaggerGenOptions =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    swaggerGenOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    swaggerGenOptions.OperationFilter<SwaggerDefaultValues>();

    swaggerGenOptions.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please provide a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
            },
            new List<string> { configuration["Authentication:Audience"]! }
        }
    });
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddApiVersioning(
    options =>
    {
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddLink(configure =>
{
    configure.AddPolicy<CartModel>(policy =>
    {
        policy
            .AddSelf(m => m.CartId)
            .AddCustomPath(m => "{cartItemId}", "DeleteCartItem", HttpMethods.Delete)
            .AddRoute(m => m.CartId, "AddItemToCart", HttpMethods.Post);
    });
});

builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddDefaultPolicy(corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(1)
    .HasApiVersion(2)
    .ReportApiVersions()
    .Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    });
}

app.UseRabbitListener();
app.UseHttpsRedirection();
app.UseCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();

app.Run();