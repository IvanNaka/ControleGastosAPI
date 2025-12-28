# Controle Gastos API

API RESTful desenvolvida em .NET 8 para gerenciamento de gastos pessoais, permitindo o controle de receitas e despesas organizadas por categorias e pessoas.
Back End Hospedado em: controlegastosapi-chapc5fgg5g6acah.brazilsouth-01.azurewebsites.net
Front End Hospedado em: https://main.d2zasgf9hme6sh.amplifyapp.com/

## Descrição

O Controle Gastos API é uma aplicação backend robusta que oferece funcionalidades completas para gestão financeira pessoal. O sistema permite que usuários registrem e monitorem suas transações financeiras, categorizem gastos e receitas, e associem movimentações a diferentes pessoas.

## Arquitetura

O projeto segue os princípios de Clean Architecture e Domain-Driven Design (DDD), organizado em quatro camadas principais:

### Camadas do Projeto

- **ControleGastos.Domain**: Contém as entidades de domínio, interfaces de repositórios e enums
- **ControleGastos.Application**: Implementa a lógica de negócios através de serviços e DTOs
- **ControleGastos.Infrastructure**: Gerencia persistência de dados, Entity Framework Core e implementações de repositórios
- **ControleGastos.Api**: Camada de apresentação com controllers e configurações da API

## Tecnologias Utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger/OpenAPI
- Repository Pattern
- Unit of Work Pattern

## Entidades Principais

### Usuario
Representa um usuário do sistema com autenticação via Azure AD.
- Id (Guid)
- Nome
- Email
- AzureAdId
- DataCriacao
- UltimaAtualizacao
- Ativo

### Pessoa
Representa indivíduos associados às transações de um usuário.
- Id (Guid)
- Nome
- Idade
- UsuarioId
- DataCriacao
- UltimaAtualizacao
- Ativo

### Categoria
Categorias para classificação de transações.
- Id (Guid)
- Descricao
- Finalidade (Receita, Despesa, Ambas)
- UsuarioId
- DataCriacao
- UltimaAtualizacao
- Ativo

### Transacao
Registros de movimentações financeiras.
- Id (Guid)
- Descricao
- Valor
- Tipo (Receita, Despesa)
- PessoaId
- CategoriaId
- UsuarioId
- DataCriacao
- UltimaAtualizacao
- Ativo

## Funcionalidades

- CRUD de Usuários
- CRUD de Pessoas
- CRUD de Categorias
- CRUD de Transações
- Soft Delete (exclusão lógica) para todas as entidades
- Global Query Filter para retornar apenas registros ativos
- Relacionamentos entre entidades
- Validação de dados
- Health Check endpoint

## Endpoints da API

### Usuarios
- POST /api/usuarios - Cria novo usuário ou retorna o existente se já cadastrado

### Pessoas
- GET /api/pessoas/usuario/{usuarioId} - Lista todas as pessoas de um usuário
- POST /api/pessoas - Cria nova pessoa
- DELETE /api/pessoas/{id} - Remove pessoa (soft delete)

### Categorias
- GET /api/categorias/usuario/{usuarioId} - Lista todas as categorias de um usuário
- POST /api/categorias - Cria nova categoria
- DELETE /api/categorias/{id} - Remove categoria (soft delete)

### Transacoes
- GET /api/transacoes/usuario/{usuarioId} - Lista todas as transações de um usuário
- POST /api/transacoes - Cria nova transação
- DELETE /api/transacoes/{id} - Remove transação (soft delete)

## Requisitos

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 ou superior (opcional)

## Padrões de Design Implementados

### Repository Pattern
Abstração da camada de acesso a dados, permitindo fácil manutenção e testabilidade.

### Unit of Work
Gerenciamento de transações e garantia de consistência nas operações de banco de dados.

### Soft Delete
Todas as entidades herdam de `BaseEntity` e suportam exclusão lógica através do campo `Ativo`.

### Global Query Filter
Filtro automático aplicado a todas as consultas, retornando apenas registros ativos.

### DTO Pattern
Uso de Data Transfer Objects para desacoplar a camada de apresentação do domínio.

## Estrutura de Pastas

```
ControleGastosAPI/
├── ControleGastos.Api/
│   ├── Controllers/
│   ├── Program.cs
│   ├── Startup.cs
│   └── appsettings.json
├── ControleGastos.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
├── ControleGastos.Domain/
│   ├── Entities/
│   ├── Enums/
│   └── Interfaces/
│       └── Repositories/
└── ControleGastos.Infrastructure/
    ├── Configurations/
    ├── Migrations/
    ├── Repositories/
    └── ControleGastosDbContext.cs
```

## Health Check

A API possui um endpoint de health check disponível em `/health` para monitoramento.


## Autor

Ivan Naka
- GitHub: [@IvanNaka](https://github.com/IvanNaka)
