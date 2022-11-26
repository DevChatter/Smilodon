## Cloning the Project

TODO: Add info about cloning project.

## Building the Project Locally

TODO: Add info about building locally.

## Setting up the Database Using EF Core

This project uses a postgres database accessed using EF Core.

### 1. Install EF Core CLI tool

The ef core CLI is included in the project, so you can call the following command to install the tool for local uses.

```bash
dotnet tool restore
```

### 2. Create a local database user for smilodon

Before you can migrate, you'll need to create a local user and database on your postgres server.

```postgresql
CREATE USER smilodon WITH PASSWORD 'smilodon' CREATEDB;
CREATE DATABASE smilodon;
```

### 3. Initialize your local database

Once your database and user exists, you can run the ef core migrations with the following command.

```bash
cd ./src/Infrastructure
dotnet ef database update
```

## Creating a New Database Migration

To create a new database migration:

```bash
cd ./src/Infrastructure/Persistence
dotnet ef migrations add <NAME>
```
