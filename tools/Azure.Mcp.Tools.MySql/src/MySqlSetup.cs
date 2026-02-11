// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Mcp.Core.Areas;
using Azure.Mcp.Core.Commands;
using Azure.Mcp.Tools.MySql.Commands.Database;
using Azure.Mcp.Tools.MySql.Commands.Server;
using Azure.Mcp.Tools.MySql.Commands.Table;
using Azure.Mcp.Tools.MySql.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Azure.Mcp.Tools.MySql;

public class MySqlSetup : IAreaSetup
{
    public string Name => "mysql";

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IMySqlService, MySqlService>();
    }

    public void RegisterCommands(CommandGroup rootGroup, ILoggerFactory loggerFactory)
    {
        var mysql = new CommandGroup(Name, "MySQL operations - Commands for managing Azure Database for MySQL Flexible Server resources. Includes operations for listing servers and databases, executing SQL queries, managing table schemas, and configuring server parameters.");
        rootGroup.AddSubGroup(mysql);

        // Consolidated hierarchical list command
        mysql.AddCommand("list", new MySqlListCommand(loggerFactory.CreateLogger<MySqlListCommand>()));

        var database = new CommandGroup("database", "MySQL database operations");
        mysql.AddSubGroup(database);
        database.AddCommand("query", new DatabaseQueryCommand(loggerFactory.CreateLogger<DatabaseQueryCommand>()));

        var table = new CommandGroup("table", "MySQL table operations");
        mysql.AddSubGroup(table);

        var schema = new CommandGroup("schema", "MySQL table schema operations");
        table.AddSubGroup(schema);
        schema.AddCommand("get", new TableSchemaGetCommand(loggerFactory.CreateLogger<TableSchemaGetCommand>()));

        var server = new CommandGroup("server", "MySQL server operations");
        mysql.AddSubGroup(server);

        var config = new CommandGroup("config", "MySQL server configuration operations");
        server.AddSubGroup(config);
        config.AddCommand("get", new ServerConfigGetCommand(loggerFactory.CreateLogger<ServerConfigGetCommand>()));

        var param = new CommandGroup("param", "MySQL server parameter operations");
        server.AddSubGroup(param);
        param.AddCommand("get", new ServerParamGetCommand(loggerFactory.CreateLogger<ServerParamGetCommand>()));
        param.AddCommand("set", new ServerParamSetCommand(loggerFactory.CreateLogger<ServerParamSetCommand>()));
    }
}
