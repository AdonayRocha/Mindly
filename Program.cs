using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Serilog;
using OpenTelemetry.Trace;
using System.Reflection;
var builder = WebApplication.CreateBuilder(args);

// Serilog para logging estruturado
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
);

// Autenticação simplificada removida (apenas verificação manual de senha admin via atributo/middleware)

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mindly API", Version = "v1" });
    // Definição simples para exibir botão Authorize usando a senha admin
    c.AddSecurityDefinition("AdminPassword", new OpenApiSecurityScheme
    {
        Description = "Informe a senha admin configurada (header X-Admin-Password). Exemplo: admin",
        Name = "X-Admin-Password",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "AdminPassword" }
            }, new string[] {}
        }
    });

    // Incluir comentários XML no Swagger para descrever endpoints
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
builder.Services.AddHealthChecks();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            // Outras instrumentações
    );
builder.Services.AddDbContext<Mindly.Data.MindlyContext>(options =>
    options.UseOracle("User Id=RM558782;Password=fiap25;Data Source=oracle.fiap.com.br/orcl;"));
builder.Services.AddHttpClient<Mindly.Services.MindlyAiService>();

var app = builder.Build();

// Aplicar migrações automaticamente (corrige erro ORA-00942 se tabelas não existem)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<Mindly.Data.MindlyContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Loga mas não impede startup (pode faltar permissões de criação em produção)
        Serilog.Log.Error(ex, "Falha ao aplicar migrations");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
// Sem middleware de autenticação padrão; proteção feita por atributo personalizado

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

