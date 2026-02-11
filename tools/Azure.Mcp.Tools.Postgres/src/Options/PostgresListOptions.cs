// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Azure.Mcp.Tools.Postgres.Options;

/// <summary>
/// Options for the consolidated PostgreSQL list command.
/// Supports hierarchical listing: servers, databases, or tables.
/// </summary>
public class PostgresListOptions : BasePostgresOptions
{
    // Inherits:
    // - User (from BasePostgresOptions)
    // - Server (from BasePostgresOptions)
    // - Database (from BasePostgresOptions)
    // - Subscription, ResourceGroup, Tenant, RetryPolicy (from SubscriptionOptions)
}
