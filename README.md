# ab-dbd25

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Team-AB-DevOps_ab-dbd25&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Team-AB-DevOps_ab-dbd25)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Team-AB-DevOps_ab-dbd25&metric=bugs)](https://sonarcloud.io/summary/new_code?id=Team-AB-DevOps_ab-dbd25)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=Team-AB-DevOps_ab-dbd25&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=Team-AB-DevOps_ab-dbd25)
[![Duplicate Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=Team-AB-DevOps_ab-dbd25&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=Team-AB-DevOps_ab-dbd25)

## Getting started

#### 1. Create your environment file

Use .env.sample as a template for own .env

#### 2. Start the database containers

```bash
docker compose up -d
```

#### 3. Seed the PostgreSQL database by running the API

```bash
cd sqlProject/api/
dotnet run
```
