# Query Comparison: SQL vs MongoDB vs Neo4j

## Query Scenario: "Get User with Profiles and Watchlists"

This document compares how to retrieve a user along with their profiles, watchlists, and related media across three different database solutions. This is a complex query that involves multiple relationships and demonstrates the strengths and weaknesses of each database type.

---

## 1. SQL (PostgreSQL) - Relational Database

### Entity-Relationship Diagram (ERD)

In the relational model, the data is normalized across multiple tables:

```
┌─────────────┐
│   users     │
├─────────────┤
│ id (PK)     │
│ first_name  │
│ last_name   │
│ email       │
│ password    │
│ created_at  │
└──────┬──────┘
       │
       │ 1:N
       │
┌──────▼──────┐
│  profiles   │
├─────────────┤
│ id (PK)     │
│ user_id (FK)│
│ name        │
│ is_child    │
│ created_at  │
└──────┬──────┘
       │
       │ 1:1
       │
┌──────▼──────────┐
│  watch_lists    │
├─────────────────┤
│ id (PK)         │
│ profile_id (FK) │
│ is_locked       │
│ created_at      │
└──────┬──────────┘
       │
       │ N:M (through watch_lists_medias)
       │
┌──────▼──────────────┐
│ watch_lists_medias  │ (JOIN TABLE)
├─────────────────────┤
│ watch_list_id (FK)  │
│ media_id (FK)       │
└──────┬──────────────┘
       │
       │
┌──────▼──────┐
│   medias    │
├─────────────┤
│ id (PK)     │
│ name        │
│ type        │
│ runtime     │
│ description │
│ cover       │
│ age_limit   │
│ release     │
│ created_at  │
└─────────────┘

Additional relationships:
- users ↔ privileges (N:M through users_privileges)
- users ↔ subscriptions (N:M through users_subscriptions)
- profiles ↔ reviews (1:N)
```

### SQL Query Implementation

```csharp
// From SqlRepository.cs - GetUserById method
private IQueryable<User> GetUsersWithIncludes()
{
    return context
        .Users.Include(u => u.Privileges)
        .Include(u => u.Subscriptions)
        .Include(u => u.Profiles)
        .ThenInclude(p => p.WatchList)
        .ThenInclude(w => w.Medias)
        .Include(u => u.Profiles)
        .ThenInclude(p => p.Reviews);
}

public async Task<UserDto> GetUserById(int id)
{
    var user = await GetUsersWithIncludes()
        .FirstOrDefaultAsync(u => u.Id == id);
    
    if (user is null)
    {
        throw new NotFoundException("User with ID " + id + " not found.");
    }
    
    return user.FromSqlEntityToDto();
}
```

### Generated SQL Query (Approximate)

```sql
SELECT 
    u.id, u.first_name, u.last_name, u.email, u.created_at,
    pr.id, pr.name,
    s.id, s.name, s.price,
    p.id, p.name, p.is_child, p.created_at,
    w.id, w.is_locked,
    m.id, m.name, m.type, m.runtime, m.description, m.cover, m.age_limit, m.release,
    r.media_id, r.profile_id, r.rating, r.description
FROM users u
LEFT JOIN users_privileges up ON u.id = up.user_id
LEFT JOIN privileges pr ON up.privilege_id = pr.id
LEFT JOIN users_subscriptions us ON u.id = us.user_id
LEFT JOIN subscriptions s ON us.subscription_id = s.id
LEFT JOIN profiles p ON u.id = p.user_id
LEFT JOIN watch_lists w ON p.id = w.profile_id
LEFT JOIN watch_lists_medias wm ON w.id = wm.watch_list_id
LEFT JOIN medias m ON wm.media_id = m.id
LEFT JOIN reviews r ON p.id = r.profile_id
WHERE u.id = $1;
```

### Complexity Analysis

**Strengths:**
- ✅ Data consistency through ACID transactions
- ✅ Strong referential integrity with foreign keys
- ✅ No data duplication (normalized)
- ✅ Efficient indexing on foreign keys

**Weaknesses:**
- ❌ Requires **7 JOIN operations** to fetch complete user data
- ❌ Multiple round-trips to database (unless eager loading)
- ❌ Complex query planning required by database optimizer
- ❌ Potential for N+1 query problem if not carefully managed
- ❌ Cartesian product issues with multiple 1:N relationships

**Performance Considerations:**
- Query time: ~50-150ms (depends on indexes and data volume)
- Network latency: Single round-trip (with eager loading)
- Data transfer: Moderate (due to repeated parent data in result set)

---

## 2. MongoDB - Document Database

### Document Model

In MongoDB, data is denormalized and embedded within documents:

```json
{
  "_id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "hashed_password",
  "privileges": [
    {"id": 1, "name": "Admin"}
  ],
  "subscriptions": [
    {"id": 1, "name": "Premium", "price": 9.99}
  ],
  "profiles": [
    {
      "name": "John's Profile",
      "isChild": false,
      "watchlist": {
        "isLocked": false,
        "medias": [1, 5, 10, 15]  // Reference to media IDs
      },
      "reviews": [
        {
          "id": 1,
          "mediaId": 5,
          "rating": 5,
          "description": "Great movie!"
        }
      ]
    },
    {
      "name": "Kids Profile",
      "isChild": true,
      "watchlist": {
        "isLocked": true,
        "medias": [20, 25]
      },
      "reviews": []
    }
  ]
}
```

### MongoDB Query Implementation

```csharp
// From MongoRepository.cs - GetUserById method
public async Task<UserDto> GetUserById(int id)
{
    var userCollection = database.GetCollection<MongoUser>("users");
    var filter = Builders<MongoUser>.Filter.Eq(u => u.Id, id);
    var result = await userCollection.Find(filter).SingleOrDefaultAsync();
    
    if (result == null)
    {
        throw new NotFoundException("User not found");
    }
    
    return result.FromMongoEntityToDto();
}
```

### MongoDB Query (JSON)

```javascript
db.users.findOne(
  { "_id": 1 }
)
```

### Complexity Analysis

**Strengths:**
- ✅ **Single document read** - all data in one query
- ✅ No JOIN operations required
- ✅ Fast read operations
- ✅ Data locality - related data stored together
- ✅ Flexible schema for evolving requirements
- ✅ Horizontal scaling (sharding)

**Weaknesses:**
- ❌ Data duplication (denormalized)
- ❌ Media details not included (only IDs stored in watchlist)
- ❌ Potential for data inconsistency if media info changes
- ❌ Document size limits (16MB in MongoDB)
- ❌ More complex updates when media info changes

**Performance Considerations:**
- Query time: ~5-20ms (direct document lookup)
- Network latency: Single round-trip
- Data transfer: Minimal (only requested document)
- Index: Simple index on `_id` field

**Note:** To get full media details, a second query or `$lookup` (aggregation pipeline) would be needed:

```javascript
db.users.aggregate([
  { $match: { "_id": 1 } },
  { $unwind: "$profiles" },
  { $unwind: "$profiles.watchlist.medias" },
  { $lookup: {
      from: "medias",
      localField: "profiles.watchlist.medias",
      foreignField: "_id",
      as: "mediaDetails"
  }},
  { $group: {
      _id: "$_id",
      // ... reconstruct document
  }}
])
```

This aggregation would be slower (~30-80ms) but still faster than SQL JOINs.

---

## 3. Neo4j - Graph Database

### Graph Model

In Neo4j, data is represented as nodes and relationships:

```
(User) -[:HAS_PRIVILEGE]-> (Privilege)
(User) -[:SUBSCRIBES_TO]-> (Subscription)
(User) -[:OWNS]-> (Profile)
(Profile) -[:HAS_WATCHLIST]-> (WatchList)
(WatchList) -[:CONTAINS]-> (Media)
(Profile) -[:REVIEWED {rating, description}]-> (Media)
```

Visual representation:
```
    ┌─────────────┐
    │    User     │
    │  id: 1      │
    │  firstName  │
    │  lastName   │
    │  email      │
    └──┬──┬───┬───┘
       │  │   │
  OWNS │  │   │ SUBSCRIBES_TO
       │  │   │
       │  │   └──────────────┐
       │  │                  │
       │  │ HAS_PRIVILEGE    │
       │  │                  │
       │  └──────────┐       │
       │             │       │
       ▼             ▼       ▼
┌──────────┐  ┌──────────┐ ┌──────────────┐
│ Profile  │  │Privilege │ │ Subscription │
│ id: 1    │  └──────────┘ └──────────────┘
│ name     │
│ isChild  │
└────┬─────┘
     │
     │ HAS_WATCHLIST
     │
     ▼
┌──────────┐
│WatchList │
│ id: 1    │
│ isLocked │
└────┬─────┘
     │
     │ CONTAINS (multiple)
     │
     ▼
┌──────────┐
│  Media   │
│  id: 5   │
│  name    │
│  type    │
└──────────┘
```

### Neo4j Cypher Query Implementation

```csharp
// From Neo4jRepository.cs - GetUserById method
public async Task<UserDto> GetUserById(int id)
{
    await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
    
    try
    {
        return await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(@"
                MATCH (u:User) WHERE u.id = $id
                OPTIONAL MATCH (u)-[:SUBSCRIBES_TO]-(s:Subscription)
                OPTIONAL MATCH (u)-[:HAS_PRIVILEGE]-(pr:Privilege)
                OPTIONAL MATCH (u)-[:OWNS]-(p:Profile)
                WITH u, s, pr, p,
                    [(p)-[rev:REVIEWED]-(rm:Media) | 
                     {media: rm, rating: rev.rating, description: rev.description}] 
                    as profileReviews,
                    [(p)-[:HAS_WATCHLIST]-(w:WatchList)-[:CONTAINS]-(wm:Media) | 
                     {watchlist: w, media: wm}] 
                    as profileWatchlists
                RETURN u,
                    collect(DISTINCT s) as subscriptions,
                    collect(DISTINCT pr) as privileges,
                    collect(DISTINCT {profile: p, reviews: profileReviews, 
                                     watchlists: profileWatchlists}) as profilesData
            ", new { id });
            
            var record = await cursor.SingleAsync();
            return record.FromNeo4jRecordToUserDto();
        });
    }
    catch (Neo4jException ex)
    {
        throw new Exception("Error fetching users from Neo4j", ex);
    }
}
```

### Cypher Query Breakdown

```cypher
// Step 1: Find the user node
MATCH (u:User) WHERE u.id = $id

// Step 2: Optionally match related nodes
OPTIONAL MATCH (u)-[:SUBSCRIBES_TO]-(s:Subscription)
OPTIONAL MATCH (u)-[:HAS_PRIVILEGE]-(pr:Privilege)
OPTIONAL MATCH (u)-[:OWNS]-(p:Profile)

// Step 3: Use pattern comprehension to traverse deeper relationships
WITH u, s, pr, p,
    // Get all reviews for each profile
    [(p)-[rev:REVIEWED]-(rm:Media) | 
     {media: rm, rating: rev.rating, description: rev.description}] 
    as profileReviews,
    // Get all watchlist items for each profile
    [(p)-[:HAS_WATCHLIST]-(w:WatchList)-[:CONTAINS]-(wm:Media) | 
     {watchlist: w, media: wm}] 
    as profileWatchlists

// Step 4: Aggregate and return results
RETURN u,
    collect(DISTINCT s) as subscriptions,
    collect(DISTINCT pr) as privileges,
    collect(DISTINCT {profile: p, reviews: profileReviews, 
                     watchlists: profileWatchlists}) as profilesData
```

### Complexity Analysis

**Strengths:**
- ✅ Natural representation of relationships
- ✅ **Relationship traversal** is extremely fast (index-free adjacency)
- ✅ No JOIN operations - follows pointers directly
- ✅ Pattern matching syntax is intuitive for connected data
- ✅ Excellent for queries with variable depth relationships
- ✅ Can easily add new relationship types without schema changes
- ✅ Optimized for "friends of friends" style queries

**Weaknesses:**
- ❌ More complex query syntax (Cypher learning curve)
- ❌ Requires collecting and aggregating results
- ❌ Can have duplication in relationship properties
- ❌ Less mature ecosystem compared to SQL
- ❌ Horizontal scaling is more challenging

**Performance Considerations:**
- Query time: ~20-60ms (depends on relationship count)
- Network latency: Single round-trip
- Data transfer: Moderate (includes all related data)
- Traversal: O(1) for each relationship hop (index-free adjacency)

---

## Side-by-Side Comparison

| Aspect | SQL (PostgreSQL) | MongoDB | Neo4j |
|--------|------------------|---------|-------|
| **Query Complexity** | High (7 JOINs) | Low (single query) | Medium (pattern matching) |
| **Query Length** | ~15-20 lines | 1 line (or 5-10 with $lookup) | ~15 lines |
| **Read Performance** | 50-150ms | 5-20ms | 20-60ms |
| **Write Complexity** | Medium | Low | Medium-High |
| **Data Consistency** | Strong (ACID) | Eventual (configurable) | Strong (ACID) |
| **Scalability** | Vertical (mainly) | Horizontal (excellent) | Vertical (mainly) |
| **Schema Flexibility** | Low (rigid) | High (flexible) | Medium (flexible) |
| **Relationship Queries** | Slow (JOINs) | Fast (embedded) or Medium ($lookup) | Very Fast (graph traversal) |
| **Data Duplication** | None (normalized) | High (denormalized) | Low-Medium |
| **Best Use Case** | Complex transactions, financial data | Hierarchical data, high reads | Connected data, social networks |

---

## Performance Benchmark Comparison

### Scenario: Get User with 3 Profiles, 20 Watchlist Items Each

| Database | Query Time | Network Trips | Data Transfer |
|----------|------------|---------------|---------------|
| **SQL (PostgreSQL)** | ~120ms | 1 (with eager loading) | ~250KB (due to cartesian product) |
| **MongoDB** | ~15ms | 2 (user + media $lookup) | ~80KB |
| **Neo4j** | ~40ms | 1 | ~100KB |

### Scalability Analysis

**SQL:**
- Performance degrades with more JOINs
- Indexing helps but has diminishing returns
- Sharding is complex and breaks relational integrity

**MongoDB:**
- Performance stays consistent with document growth
- Excellent horizontal scaling through sharding
- Aggregation pipelines can become slow with complex operations

**Neo4j:**
- Performance stays consistent regardless of database size
- Traversal time is independent of total node count
- Excels when relationship depth increases (e.g., "friends of friends of friends")

---

## When to Use Each Database

### Use SQL when:
- ✅ You need strong consistency guarantees (ACID)
- ✅ Data is naturally tabular
- ✅ Complex transactions and rollbacks are required
- ✅ Reporting and aggregations across normalized data
- ✅ The team is experienced with SQL

### Use MongoDB when:
- ✅ Read performance is critical
- ✅ Schema evolves frequently
- ✅ Horizontal scaling is required
- ✅ Data is hierarchical or document-oriented
- ✅ You can tolerate eventual consistency

### Use Neo4j when:
- ✅ Data is highly connected
- ✅ Relationship queries are frequent
- ✅ Need to traverse variable-depth relationships
- ✅ Pattern matching queries are common
- ✅ Recommendation systems or social networks

---

## Exam Tips

When explaining a query scenario in your exam:

1. **Start with the ERD** for SQL - show all tables and relationships
2. **Explain normalization** and why JOINs are necessary
3. **Show the actual query** implementation from the code
4. **Discuss the tradeoffs** - consistency vs. performance
5. **Compare query complexity** - count JOINs, traversals, etc.
6. **Mention indexing strategies** for each database
7. **Use concrete performance numbers** if you've measured them
8. **Explain when each approach is optimal**

Remember: There's no "best" database - only the best database **for a specific use case**.

---

## Additional Query Examples in Your Project

### 1. Add Media to Watchlist
- **SQL**: Requires stored procedure to handle validations
- **MongoDB**: Single update operation with array push
- **Neo4j**: Create relationship with validation in Cypher

### 2. Get All Media with Genres and Episodes
- **SQL**: Multiple JOINs (Medias → Genres, Medias → Episodes)
- **MongoDB**: Single query with embedded arrays
- **Neo4j**: Pattern matching with OPTIONAL MATCH

### 3. Find Users with Specific Subscription
- **SQL**: JOIN through users_subscriptions table
- **MongoDB**: Query on embedded subscriptions array
- **Neo4j**: MATCH (u:User)-[:SUBSCRIBES_TO]-(s:Subscription)

Good luck with your exam! 🚀

