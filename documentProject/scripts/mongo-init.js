print('Starting MongoDB initialization script...');

// Use hardcoded database name
db = db.getSiblingDB('ab_database_mongo');

// Create application user with hardcoded credentials
db.createUser({
    user: 'appuser',
    pwd: 'apppassword123',
    roles: [
        {
            role: 'readWrite',
            db: 'ab_database_mongo',
        },
    ],
});

// Create a sample collection to verify everything works
db.createCollection('test_collection');
db.test_collection.insertOne({
    message: 'MongoDB initialized successfully',
    createdAt: new Date(),
});

print('MongoDB initialization completed - database: ab_database_mongo, user: appuser created');
