// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Mcp.Core.Areas;
using Azure.Mcp.Core.Commands;
using Azure.Mcp.Tools.Postgres.Commands.Database;
using Azure.Mcp.Tools.Postgres.Commands.Server;
using Azure.Mcp.Tools.Postgres.Commands.Table;
using Azure.Mcp.Tools.Postgres.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Azure.Mcp.Tools.Postgres;

public class PostgresSetup : IAreaSetup
{
    public string Name => "postgres";

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IPostgresService, PostgresService>();
    }

    public void RegisterCommands(CommandGroup rootGroup, ILoggerFactory loggerFactory)
    {
        var pg = new CommandGroup(Name, "PostgreSQL operations - Commands for managing Azure Database for PostgreSQL Flexible Server resources. Includes operations for listing servers and databases, executing SQL queries, managing table schemas, and configuring server parameters.");
        rootGroup.AddSubGroup(pg);

        // Consolidated hierarchical list command
        pg.AddCommand("list", new PostgresListCommand(loggerFactory.CreateLogger<PostgresListCommand>()));

        var database = new CommandGroup("database", "PostgreSQL database operations");
        pg.AddSubGroup(database);
        database.AddCommand("query", new DatabaseQueryCommand(loggerFactory.CreateLogger<DatabaseQueryCommand>()));

        var table = new CommandGroup("table", "PostgreSQL table operations");
        pg.AddSubGroup(table);

        var schema = new CommandGroup("schema", "PostgreSQL table schema operations");
        table.AddSubGroup(schema);
        schema.AddCommand("get", new TableSchemaGetCommand(loggerFactory.CreateLogger<TableSchemaGetCommand>()));

        var server = new CommandGroup("server", "PostgreSQL server operations");
        pg.AddSubGroup(server);

        var config = new CommandGroup("config", "PostgreSQL server configuration operations");
        server.AddSubGroup(config);
        config.AddCommand("get", new ServerConfigGetCommand(loggerFactory.CreateLogger<ServerConfigGetCommand>()));

        var param = new CommandGroup("param", "PostgreSQL server parameter operations");
        server.AddSubGroup(param);
        param.AddCommand("get", new ServerParamGetCommand(loggerFactory.CreateLogger<ServerParamGetCommand>()));
        param.AddCommand("set", new ServerParamSetCommand(loggerFactory.CreateLogger<ServerParamSetCommand>()));
    }
}
