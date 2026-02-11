// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.CommandLine.Parsing;
using Azure.Mcp.Core.Commands;
using Azure.Mcp.Core.Services.Telemetry;
using Azure.Mcp.Tools.Postgres.Options;
using Azure.Mcp.Tools.Postgres.Services;
using Microsoft.Extensions.Logging;

namespace Azure.Mcp.Tools.Postgres.Commands;

/// <summary>
/// Consolidated hierarchical list command for PostgreSQL resources.
/// Routes to appropriate list operation based on provided parameters:
/// - No server: Lists servers
/// - Server only: Lists databases in server
/// - Server + Database: Lists tables in database
/// </summary>
public sealed class PostgresListCommand(ILogger<PostgresListCommand> logger) : BasePostgresCommand<PostgresListOptions>(logger)
{
    private const string CommandTitle = "List PostgreSQL Resources";

    private readonly Option<string> _serverOption = PostgresOptionDefinitions.Server.AsOptional();
    private readonly Option<string> _databaseOption = PostgresOptionDefinitions.Database.AsOptional();

    public override string Name => "list";

    public override string Description => "Lists PostgreSQL resources hierarchically - servers, databases, or tables - based on the provided parameters. Without a server name, lists all PostgreSQL servers in the resource group. With a server name, lists databases in that server. With both server and database names, lists tables in that database. This unified command simplifies resource discovery across the PostgreSQL hierarchy.";

    public override string Title => CommandTitle;

    public override ToolMetadata Metadata => new() { Destructive = false, ReadOnly = true, Idempotent = true };

    protected override void RegisterOptions(Command command)
    {
        base.RegisterOptions(command);
        command.AddOption(_serverOption);
        command.AddOption(_databaseOption);
    }

    protected override PostgresListOptions BindOptions(ParseResult parseResult)
    {
        var options = base.BindOptions(parseResult);
        options.Server = parseResult.GetValueForOption(_serverOption);
        options.Database = parseResult.GetValueForOption(_databaseOption);
        return options;
    }

    public override async Task<CommandResponse> ExecuteAsync(CommandContext context, ParseResult parseResult)
    {
        try
        {
            var options = BindOptions(parseResult);
            if (!Validate(parseResult.CommandResult, context.Response).IsValid)
            {
                return context.Response;
            }

            // Validation: database requires server
            if (!string.IsNullOrEmpty(options.Database) && string.IsNullOrEmpty(options.Server))
            {
                context.Response.Message = "Database parameter requires a server parameter.";
                context.Response.StatusCode = 400;
                return context.Response;
            }

            IPostgresService pgService = context.GetService<IPostgresService>() ?? throw new InvalidOperationException("PostgreSQL service is not available.");

            // Route to appropriate list operation based on parameters
            if (!string.IsNullOrEmpty(options.Database))
            {
                // List tables in database
                List<string> tables = await pgService.ListTablesAsync(
                    options.Subscription!,
                    options.ResourceGroup!,
                    options.User!,
                    options.Server!,
                    options.Database);

                context.Response.Results = tables?.Count > 0 ?
                    ResponseResult.Create(
                        new PostgresListCommandResult(Tables: tables),
                        PostgresJsonContext.Default.PostgresListCommandResult) :
                    null;
            }
            else if (!string.IsNullOrEmpty(options.Server))
            {
                // List databases in server
                List<string> databases = await pgService.ListDatabasesAsync(
                    options.Subscription!,
                    options.ResourceGroup!,
                    options.User!,
                    options.Server);

                context.Response.Results = databases?.Count > 0 ?
                    ResponseResult.Create(
                        new PostgresListCommandResult(Databases: databases),
                        PostgresJsonContext.Default.PostgresListCommandResult) :
                    null;
            }
            else
            {
                // List servers in resource group
                List<string> servers = await pgService.ListServersAsync(
                    options.Subscription!,
                    options.ResourceGroup!,
                    options.User!);

                context.Response.Results = servers?.Count > 0 ?
                    ResponseResult.Create(
                        new PostgresListCommandResult(Servers: servers),
                        PostgresJsonContext.Default.PostgresListCommandResult) :
                    null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred listing PostgreSQL resources.");
            HandleException(context, ex);
        }

        return context.Response;
    }

    /// <summary>
    /// Result that can contain servers, databases, or tables depending on the query.
    /// Only one list will be populated per response.
    /// </summary>
    internal record PostgresListCommandResult(
        List<string>? Servers = null,
        List<string>? Databases = null,
        List<string>? Tables = null);
}
