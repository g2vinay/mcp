// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.MySql.Options;

/// <summary>
/// Options for the consolidated MySQL list command.
/// Supports hierarchical listing: servers, databases, or tables.
/// </summary>
public class MySqlListOptions : MySqlDatabaseOptions
{
    // Inherits:
    // - User (from BaseMySqlOptions)
    // - Subscription, ResourceGroup, Tenant, RetryPolicy (from SubscriptionOptions)
    // - Server (from MySqlServerOptions)
    // - Database (from MySqlDatabaseOptions)
}
