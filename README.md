
# Mindly API

API ASP.NET Core (.NET 8) para gestão de pacientes, psicólogos e rotinas.

---

## Autores

- **ADONAY RODRIGUES DA ROCHA** - RM558782
- **PEDRO HENRIQUE MARTINS DOS REIS** - RM555306
- **THAMIRES RIBEIRO CRUZ** - RM558128

---


## Sobre a Solução

O Mindly é uma API RESTful desenvolvida em ASP.NET Core (.NET 8) para facilitar o gerenciamento de pacientes, psicólogos e rotinas de acompanhamento. A solução foi pensada para clínicas, consultórios e profissionais de saúde mental que desejam digitalizar e organizar seus processos.

Principais recursos:
- Cadastro e consulta de pacientes e psicólogos
- Registro de rotinas diárias associadas a pacientes
- Validações de dados (ex: telefone, senha)
- Versionamento de API
- Health check e observabilidade
- Deploy facilitado via Docker/Render

---

## Endpoints e Exemplo de JSON

### Paciente

**Exemplo de JSON para criação:**
```json
{
   "nome": "Adonay",
   "email": "adonay@example.com",
   "senha": "123",
   "telefone": "00000000000",
   "observacao": "Transplantado renal"
}
```

**Endpoints:**
- `POST   /api/v1/Pacientes` — Cria um novo paciente
- `GET    /api/v1/Pacientes` — Lista pacientes (paginado)
- `GET    /api/v1/Pacientes/{id}` — Detalha um paciente
- `PUT    /api/v1/Pacientes/{id}` — Atualiza um paciente
- `DELETE /api/v1/Pacientes/{id}` — Remove um paciente

**Observações:**
- Não envie o campo `id` ao criar. Ele é gerado automaticamente.
- O campo `senha` deve ter no mínimo 3 caracteres.
- O campo `telefone` deve conter exatamente 11 dígitos numéricos.

---

### Psicologo

**Exemplo de JSON para criação:**
```json
{
   "nome": "Maria",
   "email": "maria@example.com",
   "senha": "abc123"
}
```

**Endpoints:**
- `POST   /api/v1/Psicologos` — Cria um novo psicólogo
- `GET    /api/v1/Psicologos` — Lista psicólogos (paginado)
- `GET    /api/v1/Psicologos/{id}` — Detalha um psicólogo
- `PUT    /api/v1/Psicologos/{id}` — Atualiza um psicólogo
- `DELETE /api/v1/Psicologos/{id}` — Remove um psicólogo

**Observações:**
- Não envie o campo `id` ao criar. Ele é gerado automaticamente.

---

### Rotina

**Exemplo de JSON para criação:**
```json
{
   "pacienteId": 10,
   "data": "2025-11-21T19:25:47.684Z",
   "comeuBem": true,
   "dormiuBem": true,
   "desabafo": "Estou triste, pois, tive dificuldades ao fazer um trabalho da faculdade.",
   "rotinaDia": "Acordei, trabalhei e estudei"
}
```

**Endpoints:**
- `POST   /api/v1/Rotinas` — Cria uma nova rotina
- `GET    /api/v1/Rotinas` — Lista rotinas (paginado)
- `GET    /api/v1/Rotinas/{id}` — Detalha uma rotina
- `PUT    /api/v1/Rotinas/{id}` — Atualiza uma rotina
- `DELETE /api/v1/Rotinas/{id}` — Remove uma rotina

**Observações:**
- Não envie o campo `id` ao criar. Ele é gerado automaticamente.
- O campo `pacienteId` deve referenciar um paciente existente.

---

## Funcionamento do CRUD

Todos os endpoints seguem o padrão RESTful:
- `POST` cria um novo registro e retorna o objeto criado (com id gerado).
- `GET` lista ou detalha registros.
- `PUT` atualiza um registro existente (requer o id na URL).
- `DELETE` remove um registro pelo id.

As respostas de criação (`POST`) retornam status 201 e o objeto criado.
As operações de atualização e remoção retornam status 204 (No Content) ou 200/404 conforme o caso.

Todos os endpoints protegidos exigem o header `X-Admin-Password`.

---

## Como rodar localmente

```powershell
dotnet restore
dotnet run --project Mindly.csproj
```

Acesse a documentação Swagger em: `http://localhost:5000/swagger` (ou porta indicada no console).

Health check: `http://localhost:5000/health`

---

## Como rodar os testes

```powershell
cd Tests
dotnet test
```

### Quais testes são feitos?
- Teste de health check (`/health`)
- (Adicione mais testes em `Tests/MindlyApiTests.cs` para cobrir endpoints de pacientes, psicólogos e rotinas)

---

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

---


## Variáveis de Ambiente Principais

| Chave                    | Uso                                                        |
|--------------------------|------------------------------------------------------------|
| `ORACLE_CONNECTION_STRING` | String de conexão ao banco Oracle                         |
| `AUTH__ADMINPASSWORD`      | Senha admin consumida pelo filtro `AdminProtectAttribute` |
| `ASPNETCORE_URLS`          | Bind de endereço/porta em ambiente de container           |

---


## Migrações EF Core

A aplicação tenta aplicar `db.Database.Migrate()` no startup. Garanta permissões de criação de tabelas no schema do usuário Oracle.

---


## Observabilidade

OpenTelemetry configurado para instrumentação ASP.NET Core básica. Ajustar exportação conforme necessidade (nenhum exporter explícito incluso).

---


## Segurança

- Trocar senha admin padrão.
- Nunca commitar connection string com credenciais reais.
- Usar variáveis de ambiente no painel Render.