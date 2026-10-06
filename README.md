# StoreBooks

StoreBooks is a small Blazor Server application for learning C#, Blazor, and book CRUD concepts. It stores book data in PostgreSQL and uses Npgsql to connect to the database.

The article below explains some of the differences between domain objects, entities, DTOs, and models:

<https://dotnetfullstackdev.substack.com/p/domain-objects-entities-dtos-and-models-same-same-but-different>

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) with Docker Compose, to run PostgreSQL
- Git, if you are cloning the repository

## Setup

1. Clone the repository and open a terminal in the project directory.
2. Create your local Compose environment file from the example:

   ```powershell
   Copy-Item .env.example .env
   ```

   Edit `.env` if needed. It supplies PostgreSQL's user, password, database name, and host port for Docker Compose. Keep credentials private; `.env` is ignored by Git.

3. Start PostgreSQL:

   ```sh
   docker compose up -d
   ```

4. Configure the application's connection string in `appsettings.json`. Set `ConnectionStrings:database` to a PostgreSQL connection string using the same database name, username, password, and host port as `.env`. For example:

   ```json
   {
     "ConnectionStrings": {
       "database": "Host=localhost;Port=5432;Database=storebooks;Username=postgres;Password=replace-me"
     }
   }
   ```

   Replace `replace-me` with the `POSTGRES_PASSWORD` value from `.env`. If you changed `POSTGRES_PORT`, use that port instead of `5432`. Do not commit real credentials in `appsettings.json`; it is tracked by Git. For a local-only override, put the connection string in `appsettings.Development.json`, which is ignored by Git.

5. Restore packages and run the application:

   ```sh
   dotnet restore
   dotnet run
   ```

6. Open the local URL printed by `dotnet run` (typically `https://localhost:####` or `http://localhost:####`) in your browser.

## Using the application

Use **Add book** to enter a book's details and save it. The book list is read from PostgreSQL. Prices are entered in cents.

Stop the app with `Ctrl+C`. Stop PostgreSQL when you no longer need it:

```sh
docker compose down
```

The Compose database uses a named volume, so stopping the container does not normally remove its data. To remove the database volume as well, run `docker compose down --volumes`; this permanently deletes the local database contents.

> **Data-loss warning:** The current application startup code calls `PurgeDatabase()`, which drops the `Books` table before recreating it. As a result, all books are deleted each time the application starts. Do not use this setup with data you need to keep.

## Troubleshooting

- If the app cannot connect to PostgreSQL, check that Docker is running, the `postgres` service is healthy, and the connection string values match `.env`.
- If the PostgreSQL port is already in use, choose a different `POSTGRES_PORT` in `.env` and use the same port in the connection string.
- If you see a certificate warning while opening the HTTPS URL, follow the [.NET HTTPS development certificate instructions](https://learn.microsoft.com/aspnet/core/security/enforcing-ssl).
