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

## Backup do banco de dados

Use `scripts/backup-db.sh` para gerar um dump compactado (`.sql.gz`) do MySQL. Veja as instruções completas de configuração (arquivo de credenciais, agendamento via cron e cópia para um servidor remoto) nos comentários no topo do próprio script.

Backup manual:

```bash
./scripts/backup-db.sh
```

Restaurar um backup:

```bash
gunzip -c ~/backups/crbvendas/crbvendas_AAAAMMDD_HHMMSS.sql.gz \
  | mysql -u crbvendas -p crbvendas
```

Recomendado: agende o script no `cron` (diariamente) e configure `VPS_HOST` para que cada backup também seja copiado para fora desta máquina — um backup que só existe no mesmo disco do banco não protege contra falha de disco.

## Regras de negócio principais

- Comissão calculada por faixa de desconto ofertado na venda (0/10% → 15%, 20% → 10%, 25% → 7%, 30% → 5%).
- Uma venda é "faturada" ou "pendente" com base na previsão de faturamento do mês consultado.
- Agenda de contatos ordena primeiro quem está com visita atrasada, depois pela próxima visita mais próxima.
