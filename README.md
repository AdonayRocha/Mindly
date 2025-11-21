# Mindly API

API ASP.NET Core (.NET 8) para gestão de pacientes, psicólogos e rotinas.

## Executar localmente

```powershell
dotnet restore
dotnet run --project Mindly.csproj
```

Health check: `http://localhost:5000/health` (ou porta indicada no console).

## Proteção admin

Enviar senha via header: `X-Admin-Password: <valor>`.
Configuração vem de `Auth:AdminPassword` (appsettings) ou variável de ambiente `AUTH__ADMINPASSWORD`.

## Connection String Oracle

Definida por (precedência):
1. `ConnectionStrings:Oracle` em `appsettings*.json`
2. Variável de ambiente `ORACLE_CONNECTION_STRING`
3. Fallback placeholder.

Formato exemplo:
```
User Id=USUARIO;Password=SENHA;Data Source=host:porta/servico
```

## Deploy em Render (Docker)

Já incluso `Dockerfile` multi-stage e `render.yaml`.

### Passos
1. Crie repositório público GitHub (se ainda não). Push do código.
2. No painel Render: New + Blueprint → selecione o repositório contendo `render.yaml`.
3. Ajuste variáveis de ambiente seguras:
   - `ORACLE_CONNECTION_STRING`
   - `AUTH__ADMINPASSWORD` (trocar `admin`).
4. Deploy inicia automaticamente; porta exposta 8080.

### Sem Docker (alternativa)
Se preferir criar manualmente um Web Service sem usar `render.yaml`:
Build Command:
```
dotnet restore; dotnet publish -c Release -o out
```
Start Command:
```
dotnet out/Mindly.dll
```
Defina também env var `ASPNETCORE_URLS` para `http://0.0.0.0:$PORT` (Render injeta `$PORT`).

## Variáveis de Ambiente Principais

| Chave | Uso |
|-------|-----|
| `ORACLE_CONNECTION_STRING` | String de conexão ao banco Oracle |
| `AUTH__ADMINPASSWORD` | Senha admin consumida pelo filtro `AdminProtectAttribute` |
| `ASPNETCORE_URLS` | Bind de endereço/porta em ambiente de container |

## Migrações EF Core

A aplicação tenta aplicar `db.Database.Migrate()` no startup. Garanta permissões de criação de tabelas no schema do usuário Oracle.

## Observabilidade

OpenTelemetry configurado para instrumentação ASP.NET Core básica. Ajustar exportação conforme necessidade (nenhum exporter explícito incluso).

## Segurança

- Trocar senha admin padrão.
- Nunca commitar connection string com credenciais reais.
- Usar variáveis de ambiente no painel Render.

## Próximos passos

- Adicionar exporter OpenTelemetry (OTLP) se desejar tracing.
- Configurar Serilog sink adicional (ex: Seq, Elastic). 
- Implementar autenticação robusta se necessário.
