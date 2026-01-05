# Database Comparison - Quick Reference Guide

## Query Pattern Cheat Sheet

### 1. Simple Read (Get by ID)

```sql
-- SQL
SELECT * FROM users WHERE id = 1;
-- Complexity: O(log n) with index
-- Time: ~5ms
```

```javascript
// MongoDB
db.users.findOne({ "_id": 1 })
// Complexity: O(1) with _id
// Time: ~2ms
```

```cypher
// Neo4j
MATCH (u:User) WHERE u.id = 1 RETURN u
// Complexity: O(log n) with index
// Time: ~3ms
```

---

### 2. One-to-Many Relationship

```sql
-- SQL: User → Profiles
SELECT u.*, p.*
FROM users u
LEFT JOIN profiles p ON u.id = p.user_id
WHERE u.id = 1;
-- Requires: JOIN operation
-- Time: ~10-20ms
```

```javascript
// MongoDB: Embedded
db.users.findOne({ "_id": 1 })
// Profiles are embedded in user document
// Time: ~2-5ms
```

```cypher
// Neo4j: Relationship traversal
MATCH (u:User)-[:OWNS]->(p:Profile)
WHERE u.id = 1
RETURN u, collect(p) as profiles
// Direct pointer traversal
// Time: ~5-10ms
```

---

### 3. Many-to-Many Relationship

```sql
-- SQL: User → Watchlist → Media (through join table)
SELECT u.*, m.*
FROM users u
JOIN profiles p ON u.id = p.user_id
JOIN watch_lists w ON p.id = w.profile_id
JOIN watch_lists_medias wm ON w.id = wm.watch_list_id
JOIN medias m ON wm.media_id = m.id
WHERE u.id = 1;
-- Requires: 4 JOINs
-- Time: ~50-100ms
```

```javascript
// MongoDB: Reference + Lookup
db.users.aggregate([
  { $match: { "_id": 1 } },
  { $unwind: "$profiles" },
  { $unwind: "$profiles.watchlist.medias" },
  { $lookup: {
      from: "medias",
      localField: "profiles.watchlist.medias",
      foreignField: "_id",
      as: "mediaDetails"
  }}
])
// Requires: Aggregation pipeline
// Time: ~30-60ms
```

```cypher
// Neo4j: Multi-hop traversal
MATCH (u:User)-[:OWNS]->(p:Profile)-[:HAS_WATCHLIST]->(w:WatchList)-[:CONTAINS]->(m:Media)
WHERE u.id = 1
RETURN u, collect({profile: p, media: m})
// Direct relationship following
// Time: ~20-40ms
```

---

### 4. Complex Filtering & Aggregation

```sql
-- SQL: Count users per subscription type
SELECT s.name, COUNT(u.id) as user_count
FROM subscriptions s
LEFT JOIN users_subscriptions us ON s.id = us.subscription_id
LEFT JOIN users u ON us.user_id = u.id
GROUP BY s.name;
-- Strong aggregation capabilities
-- Time: ~20-50ms
```

```javascript
// MongoDB: Aggregation Framework
db.users.aggregate([
  { $unwind: "$subscriptions" },
  { $group: {
      _id: "$subscriptions.name",
      user_count: { $sum: 1 }
  }}
])
// Good aggregation capabilities
// Time: ~30-70ms
```

```cypher
// Neo4j: Pattern matching + aggregation
MATCH (s:Subscription)<-[:SUBSCRIBES_TO]-(u:User)
RETURN s.name, count(u) as user_count
// Good for relationship-based aggregation
// Time: ~15-40ms
```

---

## Key Metrics Summary

| Operation | SQL | MongoDB | Neo4j |
|-----------|-----|---------|-------|
| **Single document/record read** | 5ms | 2ms | 3ms |
| **1:N relationship** | 10-20ms | 2-5ms | 5-10ms |
| **M:N relationship** | 50-100ms | 30-60ms | 20-40ms |
| **Deep relationship (3+ levels)** | 100-300ms | 60-150ms | 30-80ms |
| **Aggregation** | 20-50ms | 30-70ms | 15-40ms |
| **Full-text search** | 30-100ms | 20-60ms | 40-120ms |

---

## Common Interview Questions & Answers

### Q1: "Why did you use SQL for this project?"

**Good Answer:**
> "We used PostgreSQL for our main data storage because our application requires strong ACID guarantees for user accounts, subscriptions, and payment data. The relational model ensures data consistency through foreign key constraints and transactions. However, we recognized that the deeply nested nature of user profiles and watchlists results in multiple JOINs, which impacts read performance. This is why we also implemented MongoDB and Neo4j versions to demonstrate different approaches."

### Q2: "What's the main disadvantage of your SQL implementation?"

**Good Answer:**
> "The main disadvantage is query complexity and performance for deeply nested data. For example, fetching a user with all their profiles and watchlists requires 7 JOIN operations:
> - users → users_privileges → privileges
> - users → users_subscriptions → subscriptions  
> - users → profiles → watch_lists → watch_lists_medias → medias
> - profiles → reviews
>
> This results in a cartesian product and significantly more data transfer than necessary. Each additional relationship adds another JOIN, which increases query planning time and execution time."

### Q3: "How does MongoDB solve the JOIN problem?"

**Good Answer:**
> "MongoDB uses document embedding to denormalize related data. In our implementation, user profiles are embedded directly within the user document as an array. This means we can retrieve a user with all their profiles in a single query without any JOIN operations. The watchlist and reviews are also embedded within each profile. However, we store media as references (IDs only) to avoid duplicating large media documents. If we need full media details, we use the `$lookup` aggregation stage, which is similar to a JOIN but typically faster due to document locality."

### Q4: "When would Neo4j outperform both SQL and MongoDB?"

**Good Answer:**
> "Neo4j excels at queries involving complex relationship traversals or pattern matching. For example:
> 1. **Recommendation systems**: 'Find movies watched by people who watched the same movies as me'
> 2. **Variable-depth relationships**: 'Find all friends within 3 degrees of separation'
> 3. **Shortest path queries**: 'How are User A and User B connected through their watchlists?'
>
> In our project, Neo4j shows its strength when querying relationships between users, their profiles, watchlists, and media. The index-free adjacency means each relationship traversal is O(1), regardless of database size. In SQL, this would require multiple self-joins or recursive CTEs."

### Q5: "What are the tradeoffs with MongoDB's embedded approach?"

**Good Answer:**
> "The main tradeoffs are:
>
> **Advantages:**
> - Very fast reads (single document lookup)
> - Data locality reduces I/O operations
> - No JOIN complexity
>
> **Disadvantages:**
> - Data duplication (denormalization)
> - Update complexity: If media details change, we'd need to update multiple user documents
> - Document size limits (16MB in MongoDB)
> - Atomic updates are limited to single documents
> - Harder to maintain referential integrity
>
> In our implementation, we mitigated this by storing only media IDs in watchlists, not full media objects. This creates a hybrid approach - embedded profiles but referenced media."

### Q6: "How would you choose between these databases for a new project?"

**Good Answer:**
> "I would choose based on:
>
> **SQL (PostgreSQL):**
> - Financial applications (banking, e-commerce with payments)
> - Strong consistency requirements
> - Complex transactions
> - Relational data with many-to-many relationships
> - Mature tooling and team expertise
>
> **MongoDB:**
> - Content management systems
> - Product catalogs
> - User profiles and session data
> - High read throughput requirements
> - Rapidly evolving schemas
> - Horizontal scaling needed
>
> **Neo4j:**
> - Social networks
> - Recommendation engines
> - Fraud detection (relationship analysis)
> - Network topology
> - Knowledge graphs
> - Complex relationship queries
>
> For our streaming platform, a hybrid approach might be best: PostgreSQL for users and subscriptions, MongoDB for media catalogs, and Neo4j for recommendations."

---

## Code Snippets for Exam

### SQL: Complex Query with Multiple JOINs

```csharp
// From your SqlRepository.cs
private IQueryable<User> GetUsersWithIncludes()
{
    return context
        .Users
        .Include(u => u.Privileges)              // JOIN 1
        .Include(u => u.Subscriptions)           // JOIN 2
        .Include(u => u.Profiles)                // JOIN 3
            .ThenInclude(p => p.WatchList)       // JOIN 4
            .ThenInclude(w => w.Medias)          // JOIN 5
        .Include(u => u.Profiles)                // Re-include for next path
            .ThenInclude(p => p.Reviews);        // JOIN 6
}
```

**Key Points to Mention:**
- Entity Framework uses lazy loading by default
- `.Include()` forces eager loading
- Each `.ThenInclude()` creates another JOIN
- This prevents N+1 query problem
- Generated SQL has LEFT JOINs

---

### MongoDB: Embedded Document Query

```csharp
// From your MongoRepository.cs
public async Task AddMediaToWatchList(int userId, int profileId, int mediaId)
{
    var userFilter = Builders<MongoUser>.Filter.Eq(u => u.Id, userId);
    var update = Builders<MongoUser>.Update.AddToSet(
        $"profiles.{profileId}.watchlist.medias",
        mediaId
    );
    await userCollection.UpdateOneAsync(userFilter, update);
}
```

**Key Points to Mention:**
- Uses MongoDB's atomic `$addToSet` operator
- Prevents duplicate entries at database level
- Single operation - no transaction needed
- Updates nested array within embedded document
- Array index notation: `profiles.0.watchlist.medias`

---

### Neo4j: Cypher Pattern Matching

```csharp
// From your Neo4jRepository.cs
public async Task AddMediaToWatchList(int userId, int profileId, int mediaId)
{
    await tx.RunAsync(@"
        MATCH (u:User) WHERE u.id = $userId
        MATCH (u)-[:OWNS]-(p:Profile) WHERE p.id = $profileId
        MATCH (m:Media) WHERE m.id = $mediaId
        MERGE (p)-[:HAS_WATCHLIST]->(w:WatchList)
        MERGE (w)-[:CONTAINS]->(m)
    ", new { userId, profileId, mediaId });
}
```

**Key Points to Mention:**
- `MATCH` finds existing nodes
- `MERGE` creates relationship if it doesn't exist (idempotent)
- Pattern matching syntax: `(node)-[:RELATIONSHIP]->(node)`
- No JOIN needed - follows graph edges
- Validation happens through pattern matching

---

## Visual Data Structure Comparison

### SQL: Normalized Tables
```
users table          profiles table       watch_lists table
┌────┬──────┐       ┌────┬─────────┐     ┌────┬────────────┐
│ id │ name │       │ id │ user_id │     │ id │ profile_id │
├────┼──────┤       ├────┼─────────┤     ├────┼────────────┤
│ 1  │ John │ ──┐   │ 1  │    1    │ ──┐ │ 1  │     1      │
│ 2  │ Jane │   │   │ 2  │    1    │   │ │ 2  │     2      │
└────┴──────┘   │   │ 3  │    2    │   │ └────┴────────────┘
                └──→└────┴─────────┘   │
                                        └──→ watch_lists_medias
                                            ┌────────────┬──────────┐
                                            │ watchlist  │ media_id │
                                            ├────────────┼──────────┤
                                            │     1      │    5     │
                                            │     1      │    10    │
                                            └────────────┴──────────┘
```

### MongoDB: Embedded Documents
```json
{
  "_id": 1,
  "name": "John",
  "profiles": [
    {
      "id": 1,
      "name": "Default",
      "watchlist": {
        "medias": [5, 10, 15]  ← All data in one document
      }
    },
    {
      "id": 2,
      "name": "Kids",
      "watchlist": {
        "medias": [20, 25]
      }
    }
  ]
}
```

### Neo4j: Graph Structure
```
(User:1)──OWNS──→(Profile:1)──HAS_WATCHLIST──→(WatchList:1)──CONTAINS──→(Media:5)
   │                                                 │
   │                                                 └─CONTAINS──→(Media:10)
   │
   └─────OWNS──→(Profile:2)──HAS_WATCHLIST──→(WatchList:2)──CONTAINS──→(Media:20)
```

---

## Performance Optimization Tips

### SQL Optimization
1. **Indexing**: Create indexes on foreign keys
   ```sql
   CREATE INDEX idx_profiles_user_id ON profiles(user_id);
   CREATE INDEX idx_watch_lists_profile_id ON watch_lists(profile_id);
   ```

2. **Query Optimization**: Use `EXPLAIN ANALYZE`
   ```sql
   EXPLAIN ANALYZE
   SELECT * FROM users WHERE id = 1;
   ```

3. **Connection Pooling**: Reuse database connections

### MongoDB Optimization
1. **Indexes**: Create indexes on frequently queried fields
   ```javascript
   db.users.createIndex({ "email": 1 })
   db.users.createIndex({ "profiles.watchlist.medias": 1 })
   ```

2. **Projection**: Only fetch needed fields
   ```javascript
   db.users.find({ "_id": 1 }, { "name": 1, "email": 1 })
   ```

3. **Avoid large arrays**: Don't embed unbounded arrays

### Neo4j Optimization
1. **Indexes**: Create indexes on properties used in WHERE clauses
   ```cypher
   CREATE INDEX FOR (u:User) ON (u.id)
   CREATE INDEX FOR (m:Media) ON (m.id)
   ```

2. **Use PROFILE**: Analyze query performance
   ```cypher
   PROFILE MATCH (u:User) WHERE u.id = 1 RETURN u
   ```

3. **Limit relationship types**: Be specific with relationship patterns

---

## Common Mistakes to Avoid in Exam

❌ **"MongoDB is always faster than SQL"**
- Context matters! SQL is faster for complex aggregations and transactions

✅ **"MongoDB is faster for document reads, but SQL is better for complex joins and ACID transactions"**

---

❌ **"Neo4j stores data in tables"**
- Neo4j stores nodes and relationships in a graph structure

✅ **"Neo4j uses index-free adjacency where nodes directly point to related nodes"**

---

❌ **"You should always denormalize in MongoDB"**
- Sometimes references are better (e.g., for frequently updated data)

✅ **"MongoDB encourages embedding related data, but references should be used for large or frequently changing data"**

---

❌ **"JOINs are bad"**
- JOINs are essential in relational databases to avoid duplication

✅ **"JOINs are necessary in SQL to maintain normalization, but they can impact performance with many relationships"**

---

## Final Exam Strategy

1. **Start with ERD**: Draw the relational schema
2. **Explain the query**: Show SQL with JOINs
3. **Discuss performance**: Mention query planning, cartesian products
4. **Compare to MongoDB**: Show embedded document structure
5. **Compare to Neo4j**: Show graph pattern matching
6. **Use concrete numbers**: Reference actual query times from your project
7. **Conclude with tradeoffs**: No perfect solution, depends on use case

Good luck! 🎯

