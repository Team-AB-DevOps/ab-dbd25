# ab-dbd25

## Getting started

#### 1. Create your environment file

Use .env.sample as a template for own .env

#### 2. Start the database containers

```bash
docker compose up -d
```

Four docker containers get created - one for the relational database, document database and graph database. The final container is the backend/API, which seeds the relational database automatically.

Afterwards you can use the migration tools, to migrate the relational data to the document- and graph databases.
