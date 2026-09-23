# Pimentinha Kids — Sistema (CRBVendas)

Sistema interno em ASP.NET Core MVC (.NET 8) para controle de clientes, fornecedores, contatos (agenda de visitas) e vendas/comissões.

## Stack

- ASP.NET Core MVC — .NET 8
- Entity Framework Core 9 + Pomelo (MySQL)
- Bootstrap 5 / jQuery Validation

## Estrutura

```
CRBVendas/
├── Controllers/     # Controllers MVC (Clientes, Fornecedores, Contatos, Vendas, Home)
├── Models/           # Entidades e ViewModels
├── Repositories/     # Acesso a dados (EF Core)
├── Services/         # Regras de negócio e validação
├── Data/              # AppDbContext
├── Migrations/        # Migrations do EF Core
└── Views/              # Razor Views
```

## Como rodar localmente

1. Instale o [.NET 8 SDK](https://dotnet.microsoft.com/download) e tenha um MySQL acessível.
2. Copie o arquivo de exemplo de configuração e ajuste a connection string:

   ```bash
   cd CRBVendas
   cp appsettings.Development.json.example appsettings.Development.json
   ```

   Edite `appsettings.Development.json` com o usuário/senha do seu MySQL local. **Esse arquivo não é versionado** (está no `.gitignore`) — nunca commite credenciais reais.

3. Aplique as migrations:

   ```bash
   dotnet ef database update
   ```

4. Rode a aplicação:

   ```bash
   dotnet run
   ```

Em produção, a connection string deve vir de variável de ambiente (`ConnectionStrings__DefaultConnection`) ou de um cofre de segredos — nunca de `appsettings.json`, que fica versionado com placeholder vazio.

## Regras de negócio principais

- Comissão calculada por faixa de desconto ofertado na venda (0/10% → 15%, 20% → 10%, 25% → 7%, 30% → 5%).
- Uma venda é "faturada" ou "pendente" com base na previsão de faturamento do mês consultado.
- Agenda de contatos ordena primeiro quem está com visita atrasada, depois pela próxima visita mais próxima.
