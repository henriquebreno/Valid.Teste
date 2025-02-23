Documentação do Projeto
Visão Geral
Este projeto segue uma arquitetura mínima em camadas e utiliza o Dapper como ORM para interação com o banco de dados SQL Server. Ele fornece uma API de autenticação baseada em Cookies, permitindo login e logout.

Tecnologias Utilizadas
.NET 7
ASP.NET Core Minimal API
Dapper (para acesso ao banco de dados)
AutoMapper (para mapeamento de objetos)
Autenticação baseada em Cookies
Políticas de Autorização com Claims
Swagger (para documentação da API)
Configuração do Banco de Dados
O projeto inicia criando automaticamente a tabela ProfileParameters caso ela não exista e insere um usuário admin.

A configuração da string de conexão está definida como:

json
Copiar
Editar
"DefaultConnection": "Server=sql.bsite.net\\MSSQL2016;Database=loja_valid;User Id=loja_valid;Password=admin;"
Endpoints
Autenticação
Login
URL: POST /api/auth/login
Corpo da requisição:
json
Copiar
Editar
{
  "ProfileName": "admin"
}
Resposta de Sucesso:
json
Copiar
Editar
{
  "message": "Login successful."
}
Logout
URL: POST /api/auth/logout
Resposta de Sucesso:
json
Copiar
Editar
{
  "message": "Logout successful."
}
Configuração da Autenticação
O projeto utiliza autenticação baseada em Cookies:

csharp
Copiar
Editar
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
        options.AccessDeniedPath = "/api/auth/access-denied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
Políticas de Autorização
O projeto define duas políticas de autorização baseadas em Claims:

csharp
Copiar
Editar
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanEditProfile", policy =>
        policy.Requirements.Add(new ProfileOperationRequirement("CanEdit")))
    .AddPolicy("CanDeleteProfile", policy =>
        policy.Requirements.Add(new ProfileOperationRequirement("CanDelete")));
Inicialização do Banco de Dados
Durante a inicialização do projeto, o banco de dados é verificado e a tabela ProfileParameters é criada automaticamente, caso não exista. Um usuário admin também é inserido.

Gestão do ProfileParameter
O ProfileParameter é utilizado para armazenar configurações específicas de perfis dentro do sistema.

Criar um novo ProfileParameter
URL: POST /api/profiles
Corpo da requisição:
json
Copiar
Editar
{
  "ProfileName": "admin",
  "Parameters": {
    "CanEdit": "true",
    "CanDelete": "false"
  }
}
Resposta de Sucesso:
json
Copiar
Editar
{
  "message": "Profile created successfully."
}
Consultar um ProfileParameter
URL: GET /api/profiles/{profileName}
Resposta de Sucesso:
json
Copiar
Editar
{
  "ProfileName": "admin",
  "Parameters": {
    "CanEdit": "true",
    "CanDelete": "false"
  }
}
Atualizar um ProfileParameter
URL: PUT /api/profiles/{profileName}
Corpo da requisição:
json
Copiar
Editar
{
  "ProfileName": "admin",
  "Parameters": {
    "CanEdit": "false",
    "CanDelete": "true"
  }
}
Resposta de Sucesso: 204 No Content
Excluir um ProfileParameter
URL: DELETE /api/profiles/{profileName}
Resposta de Sucesso: 204 No Content
Background Service
O projeto inclui um serviço em segundo plano que é executado a cada 5 minutos. Este serviço realiza a atualização de dados de perfil de forma automatizada, exceto para o perfil "admin". A cada execução, ele gera dados de perfil aleatórios utilizando a biblioteca Faker, como nome de usuário, status, grupos e outras informações relacionadas a cada perfil. Esses dados gerados são então serializados e atualizados no banco de dados.

O serviço continua rodando indefinidamente até ser interrompido, realizando a atualização dos perfis periodicamente a cada 5 minutos.

Como Executar o Projeto
Clone o repositório.
Configure a string de conexão no appsettings.json.
Execute o comando:
bash
Copiar
Editar
dotnet run
Acesse o Swagger em https://localhost:<porta>/swagger/index.html.
Contato
Para mais informações ou dúvidas, entre em contato com o desenvolvedor.
