# PostgreSQL to Neo4j Migration Tool

This console application migrates data from a PostgreSQL streaming platform database to a Neo4j graph database, transforming relational data into a graph structure optimized for queries about users, media content, and relationships.

## ✅ Updated Implementation

This migration tool now references the existing API project models directly, eliminating code duplication and ensuring consistency with your database schema.

### Key Improvements

- **Reuses API Models**: Imports models from `api.Models` instead of duplicating them
- **Extension Methods**: Clean transformation from API models to graph models
- **Automatic Schema Mapping**: Maps API model properties to graph node properties
- **Simplified Data Access**: Uses actual API model structure for database queries

## Features

- **Complete Data Migration**: Migrates all entities including users, profiles, media, episodes, genres, persons, and relationships
- **Graph Schema Creation**: Automatically creates constraints and indexes in Neo4j for optimal performance
- **Batch Processing**: Efficiently processes large datasets using batch operations
- **Comprehensive Logging**: Detailed logging with progress tracking and error handling
- **Configuration-Driven**: Easy setup through appsettings.json
- **Statistics Reporting**: Migration completion with detailed statistics

## Graph Schema

The migration creates the following node types:

- **User**: Platform users with authentication and personal information
- **Profile**: User viewing profiles with preferences and settings
- **WatchList**: User-created content collections
- **Media**: Movies, TV shows, and other content
- **Episode**: Individual episodes of TV series
- **Person**: Actors, directors, producers, and other industry professionals
- **Genre**: Content categorization (Action, Drama, Comedy, etc.)
- **Role**: Professional roles in media production
- **Subscription**: Platform subscription tiers
- **Privilege**: System permissions and access rights

## Relationships

The graph includes these relationship types:

- `OWNS`: Users own profiles
- `HAS_WATCHLIST`: Profiles have watchlists
- `CONTAINS`: Watchlists contain media items
- `HAS_EPISODE`: Media has episodes
- `BELONGS_TO_GENRE`: Media belongs to genres
- `WORKED_ON`: Persons worked on media (with role properties)
- `REVIEWED`: Users reviewed media (with rating and review properties)
- `SUBSCRIBES_TO`: Users subscribe to subscription plans
- `GIVES_ACCESS_TO`: Subscriptions give access to genres
- `HAS_PRIVILEGE`: Users have system privileges

## Prerequisites

- .NET 9.0 or later
- PostgreSQL database with streaming platform schema
- Neo4j database (version 4.0 or later recommended)
- Network access to both databases

## Setup

1. **Configure Connection Strings**

    Update `appsettings.json` with your database connection details:

    ```json
    {
        "ConnectionStrings": {
            "PostgreSql": "Host=localhost;Database=your_postgres_db;Username=your_user;Password=your_password"
        },
        "Neo4j": {
            "Uri": "bolt://localhost:7687",
            "User": "neo4j",
            "Password": "your_neo4j_password"
        }
    }
    ```

2. **Ensure Database Access**
    - PostgreSQL: User needs SELECT permissions on all tables
    - Neo4j: User needs full database access for schema creation and data insertion

## Usage

### Command Line

```bash
# Build the project
dotnet build

# Run the migration
dotnet run

# Run with custom configuration
dotnet run --ConnectionStrings:PostgreSql="your_custom_connection_string"
```

### Expected Output

```
🚀 PostgreSQL to Neo4j Migration Tool
======================================
Validating configuration...
Configuration validation passed

Starting migration...
This may take several minutes depending on data size.

[INFO] Starting migration process...
[INFO] Initializing Neo4j database...
[INFO] Created 10 constraints and 10 indexes
[INFO] Migrating nodes...
[INFO] Migrated 1,500 User nodes
[INFO] Migrated 3,200 Profile nodes
[INFO] Migrated 890 WatchList nodes
[INFO] Migrated 15,000 Media nodes
[INFO] Migrated 45,000 Episode nodes
[INFO] Migrated 5,000 Person nodes
[INFO] Migrated 25 Genre nodes
[INFO] Migrated 50 Role nodes
[INFO] Migrated 10 Subscription nodes
[INFO] Migrated 15 Privilege nodes
[INFO] Migrating relationships...
[INFO] Migration completed successfully in 00:05:23

✅ Migration completed successfully!
```

## Architecture

The migration tool follows a service-oriented architecture:

- **Program.cs**: Entry point with dependency injection setup and API project reference
- **MigrationOrchestrator**: Coordinates the migration workflow
- **PostgreSqlDataService**: Extracts data from PostgreSQL using API models
- **Neo4jMigrationService**: Creates schema and inserts data into Neo4j
- **Models/GraphModels.cs**: Graph node definitions and transformation helpers

### Model Mapping

```csharp
// Extension methods convert API models to graph models
var graphUser = apiUser.ToGraphUser();
var graphProfile = apiProfile.ToGraphProfile();
```

## Benefits of API Project Reference

1. **No Code Duplication**: Single source of truth for data models
2. **Automatic Updates**: Model changes in API automatically reflected in migration tool
3. **Type Safety**: Compile-time checking ensures model compatibility
4. **Simplified Maintenance**: Only need to update models in one place

## Troubleshooting

### Common Issues

1. **Build Errors**
    - Ensure API project builds successfully first
    - Check that project reference path is correct
    - Verify package version compatibility

2. **Model Property Mismatches**
    - API models may have different property names than expected
    - Extension methods handle property mapping and provide defaults for missing fields
    - Check actual API model definitions in case of mapping issues

## License

This migration tool is part of the streaming platform project and follows the same licensing terms.
