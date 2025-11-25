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

// Initialize counters collection for auto-incrementing IDs
db.createCollection('counters');
print('Creating counters for auto-incrementing IDs...');

// Get the maximum IDs from existing collections (if they exist)
const maxMediaId = db.medias.findOne({}, { sort: { _id: -1 } })?._id || 0;
const maxEpisodeId = db.episodes.findOne({}, { sort: { _id: -1 } })?._id || 0;
const maxUserId = db.users.findOne({}, { sort: { _id: -1 } })?._id || 0;

// Initialize counters with max existing IDs
db.counters.insertMany([
    { _id: 'media_id', sequence_value: maxMediaId },
    { _id: 'episode_id', sequence_value: maxEpisodeId },
    { _id: 'user_id', sequence_value: maxUserId }
]);

print(`Counters initialized - media: ${maxMediaId}, episode: ${maxEpisodeId}, user: ${maxUserId}`);

print(`MongoDB initialization completed - database: ${dbName}, user: ${appUser} created`);
