print('Starting MongoDB initialization script...');

const dbName = process.env.MONGO_INITDB_DATABASE || 'ab_database_mongo';
const appUser = process.env.MONGO_APP_USER || 'appuser';
const appPassword = process.env.MONGO_APP_PW || 'apppassword123';

db = db.getSiblingDB(dbName);

db.createUser({
    user: appUser,
    pwd: appPassword,
    roles: [{ role: 'readWrite', db: dbName }],
});

db.createCollection('test_collection');
db.test_collection.insertOne({ message: 'MongoDB initialized successfully', createdAt: new Date() });

print(`MongoDB initialization completed - database: ${dbName}, user: ${appUser} created`);
