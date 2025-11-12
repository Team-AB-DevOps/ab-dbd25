#!/bin/bash
set -e

echo "Initializing users with environment variables..."

# Check if required environment variables are set
if [[ -z "$APP_READER_PASSWORD" || -z "$APP_WRITER_PASSWORD" || -z "$ADMIN_PASSWORD" ]]; then
	echo "ERROR: Required environment variables are not set!"
	echo "APP_READER_PASSWORD: ${APP_READER_PASSWORD:-NOT_SET}"
	echo "APP_WRITER_PASSWORD: ${APP_WRITER_PASSWORD:-NOT_SET}"
	echo "ADMIN_PASSWORD: ${ADMIN_PASSWORD:-NOT_SET}"
	exit 1
fi

echo "Environment variables verified. Processing users.sql..."

# Use sed to substitute environment variables (more reliable than envsubst)
sed "s/\${APP_READER_PASSWORD}/$APP_READER_PASSWORD/g; s/\${APP_WRITER_PASSWORD}/$APP_WRITER_PASSWORD/g; s/\${ADMIN_PASSWORD}/$ADMIN_PASSWORD/g" \
	/docker-entrypoint-initdb.d/users.sql |
	psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB"

echo "User initialization completed successfully!"
