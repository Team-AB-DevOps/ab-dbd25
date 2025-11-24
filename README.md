# Streaming Platform API (ab-dbd25)

## Getting Started

### 1. Environment Setup

Create a `.env` file in the root directory using the provided sample.

```bash
cp .env.sample .env
# Edit .env with your preferred credentials if necessary
```

### 2. Start Services

Launch the database containers and API using Docker Compose:

```bash
docker compose up -d
```

This will start four containers:

- **PostgreSQL** (Relational DB)
- **MongoDB** (Document DB)
- **Neo4j** (Graph DB)
- **API** (Backend service)

> **Note:** The API container automatically seeds the PostgreSQL database on startup.

## Data Migration

Once the services are running and the SQL database is seeded, you can migrate data to the other databases.

### SQL to MongoDB (Document)

Migrate relational data to the document store.

1. Navigate to the tool directory:

    ```bash
    cd sqlProject/migration-tools/sql-to-document
    ```

2. Run the migration:
    ```bash
    dotnet run
    ```
    _Follow the interactive menu or use `dotnet run migrate` to execute immediately._

### SQL to Neo4j (Graph)

Migrate relational data to the graph database.

1. Navigate to the tool directory:

    ```bash
    cd sqlProject/migration-tools/sql-to-graph
    ```

2. Run the migration:
    ```bash
    dotnet run
    ```
