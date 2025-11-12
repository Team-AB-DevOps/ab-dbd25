-- Role groups
CREATE ROLE app_readonly;
CREATE ROLE app_readwrite;
CREATE ROLE app_admin;

-- Grant CONNECT explicitly on the target database
GRANT CONNECT ON DATABASE abdevopsdb TO app_readonly;
GRANT CONNECT ON DATABASE abdevopsdb TO app_readwrite;
GRANT CONNECT ON DATABASE abdevopsdb TO app_admin;

-- TABLE PERMISSIONS --

-- For readonly role
GRANT SELECT ON ALL TABLES IN SCHEMA public TO app_readonly; -- Existing tables
-- Ensure future tables created by the DB owner (alihmdevops) grant read privileges
ALTER DEFAULT PRIVILEGES FOR ROLE alihmdevops IN SCHEMA public -- Future tables
    GRANT SELECT ON TABLES TO app_readonly;

-- For readwrite role
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO app_readwrite;
ALTER DEFAULT PRIVILEGES FOR ROLE alihmdevops IN SCHEMA public
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO app_readwrite;

-- Allow the read-write role and the concrete app user to create objects in the schema
-- (needed so runtime migrations / EF can create tables like __EFMigrationsHistory)
GRANT USAGE, CREATE ON SCHEMA public TO app_readwrite;
GRANT USAGE, CREATE ON SCHEMA public TO app_readonly;

-- For admin role (includes sequences for SERIAL columns)
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO app_admin;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO app_admin;
ALTER DEFAULT PRIVILEGES FOR ROLE alihmdevops IN SCHEMA public
    GRANT ALL ON TABLES TO app_admin;
ALTER DEFAULT PRIVILEGES FOR ROLE alihmdevops IN SCHEMA public
    GRANT ALL ON SEQUENCES TO app_admin;

-- END TABLE PERMISSIONS --

-- Create users and assign them to role groups
CREATE USER app_reader WITH PASSWORD '${APP_READER_PASSWORD}';
CREATE USER app_writer WITH PASSWORD '${APP_WRITER_PASSWORD}';
CREATE USER admin_user WITH PASSWORD '${ADMIN_PASSWORD}';

GRANT app_readonly TO app_reader;
GRANT app_readwrite TO app_writer;
GRANT app_admin TO admin_user;
