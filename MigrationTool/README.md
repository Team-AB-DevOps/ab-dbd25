# PostgreSQL to MongoDB Migration Tool

This console application migrates data from your PostgreSQL database (sqlProject) to MongoDB (documentProject).

## Prerequisites

1. **PostgreSQL database** must be running and accessible
2. **MongoDB database** must be running (via docker-compose in documentProject)
3. **.NET 9.0** SDK installed

## Setup

1. **Restore packages**:

    ```bash
    cd MigrationTool
    dotnet restore
    ```

2. **Update connection strings** in `appsettings.json` if needed:
    - PostgreSQL: Update if your PostgreSQL connection differs
    - MongoDB: Update if your MongoDB connection differs

3. **Ensure databases are running**:

    ```bash
    # Start PostgreSQL (if not already running)
    cd ../sqlProject
    docker-compose up -d

    # Start MongoDB
    cd ../documentProject
    docker-compose up -d
    ```

## Usage

### Interactive Mode (Recommended for first-time users)

```bash
dotnet run
```

This will show a menu with options to test connections or run migration.

### Command Line Mode

**Test database connections:**

```bash
dotnet run test
```

**Run full migration:**

```bash
dotnet run migrate
```

**Show help:**

```bash
dotnet run help
```

## Migration Process

The tool migrates data with embedded documents optimized for MongoDB:

### MongoDB Collections Created:

1. ✅ **subscriptions** - Simple subscription data
2. ✅ **accounts** - User authentication data extracted from users table
3. ✅ **users** - Users with embedded:
    - Subscriptions (as foreign key references)
    - Privileges (as string arrays)
    - Profiles (as embedded documents) containing:
        - Watchlists (embedded with media references)
        - Reviews (embedded documents)
4. ✅ **medias** - Media with embedded:
    - Genres (as string arrays)
    - Episodes (as foreign key references)
    - Credits (embedded documents with person roles)
5. ✅ **persons** - Person data
6. ✅ **episodes** - Episode data

### Key Improvements:

- **Embedded Documents**: Related data is nested within parent documents for better performance
- **Denormalized Data**: Frequently accessed data is embedded to reduce joins
- **MongoDB Optimized**: Schema designed for MongoDB's document-based strengths

## Configuration

### appsettings.json

- `ConnectionStrings:PostgresConnection`: PostgreSQL connection string
- `ConnectionStrings:MongoConnection`: MongoDB connection string
- `Migration:BatchSize`: Number of records to process at once (default: 1000)
- `Migration:EnableProgressLogging`: Show progress during migration (default: true)

### Environment Variables

The tool automatically loads environment variables from:

- `../sqlProject/api/.env` (PostgreSQL settings)
- `../documentProject/.env` (MongoDB settings)

## Troubleshooting

### Connection Issues

1. **PostgreSQL**: Ensure the database is running and connection string is correct
2. **MongoDB**: Ensure Docker container is running and authentication is set up

### Migration Issues

1. **Duplicate data**: The tool doesn't check for existing data - ensure MongoDB collections are empty
2. **Memory issues**: Reduce `BatchSize` in appsettings.json for large datasets
3. **Foreign key issues**: The migration order is designed to handle relationships correctly

### Verify Migration

After migration, you can connect to MongoDB and verify data:

```bash
# Connect to MongoDB container
docker exec -it mongo-db mongosh

# In MongoDB shell:
use ab_database_mongo

# Check collections
show collections

# Check document counts
db.subscriptions.countDocuments()
db.accounts.countDocuments()
db.users.countDocuments()
db.medias.countDocuments()
db.persons.countDocuments()
db.episodes.countDocuments()

# Check embedded data structure
db.users.findOne() // See embedded profiles, reviews, watchlists
db.medias.findOne() // See embedded genres and credits
```

## Development

The project structure:

- `Program.cs`: Main entry point and CLI handling
- `Services/PostgresService.cs`: PostgreSQL connection and operations
- `Services/MongoService.cs`: MongoDB connection and operations
- `Services/MigrationService.cs`: Main migration logic
- `appsettings.json`: Configuration settings

To add new migration logic or modify existing migrations, update the relevant methods in `MigrationService.cs`.
