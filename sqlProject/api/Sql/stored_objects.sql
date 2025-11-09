
-- =============================================
-- VIEWS
-- =============================================

-- View: User Content Access
-- Shows what content each user can access based on their subscriptions
CREATE OR REPLACE VIEW user_content_access AS
SELECT DISTINCT 
    u.id as user_id,
    u.first_name,
    u.last_name,
    u.email,
    m.id as media_id,
    m.name as media_name,
    m.type as media_type,
    m.age_limit,
    s.name as subscription_name,
    g.name as genre_name
FROM users u
JOIN users_subscriptions us ON u.id = us."UsersId"
JOIN subscriptions s ON us."SubscriptionsId" = s.id
JOIN genres_subscriptions gs ON s.id = gs."SubscriptionsId"
JOIN genres g ON gs."GenresId" = g.id
JOIN medias_genres mg ON g.id = mg."GenresId"
JOIN medias m ON mg."MediasId" = m.id
ORDER BY u.id, m.name;

-- View: Popular Content
-- Shows content ranked by average rating and review count
CREATE OR REPLACE VIEW popular_content AS
SELECT 
    m.id,
    m.name,
    m.type,
    m.release,
    COUNT(r.media_id) as review_count,
    ROUND(AVG(r.rating::numeric), 2) as average_rating,
    CASE 
        WHEN COUNT(r.media_id) >= 5 AND AVG(r.rating) >= 4.0 THEN 'Highly Rated'
        WHEN COUNT(r.media_id) >= 3 AND AVG(r.rating) >= 3.5 THEN 'Well Liked'
        WHEN COUNT(r.media_id) >= 1 THEN 'Average'
        ELSE 'No Reviews'
    END as rating_category
FROM medias m
LEFT JOIN reviews r ON m.id = r.media_id
GROUP BY m.id, m.name, m.type, m.release
ORDER BY average_rating DESC NULLS LAST, review_count DESC;

-- View: User Activity Summary
-- Combines user profiles, watchlists, and review activity
CREATE OR REPLACE VIEW user_activity_summary AS
SELECT 
    u.id as user_id,
    u.first_name,
    u.last_name,
    p.id as profile_id,
    p.name as profile_name,
    p.is_child,
    COUNT(DISTINCT wlm."MediasId") as watchlist_items,
    COUNT(DISTINCT r.media_id) as reviews_written,
    ROUND(AVG(r.rating::numeric), 2) as average_rating_given,
    MAX(r.created_at) as last_review_date,
    MAX(wl.created_at) as last_watchlist_update
FROM users u
JOIN profiles p ON u.id = p.user_id
LEFT JOIN watch_lists wl ON p.id = wl.profile_id
LEFT JOIN watch_lists_medias wlm ON wl.profile_id = wlm."WatchListsProfileId"
LEFT JOIN reviews r ON p.id = r.profile_id
GROUP BY u.id, u.first_name, u.last_name, p.id, p.name, p.is_child
ORDER BY u.id, p.id;

-- View: Content Cast and Crew
-- Shows detailed cast and crew information for each media item
CREATE OR REPLACE VIEW content_cast_crew AS
SELECT 
    m.id as media_id,
    m.name as media_name,
    m.type,
    p.first_name || ' ' || p.last_name as person_name,
    p.birth_date,
    p.gender,
    r.name as role_name,
    CASE r.name
        WHEN 'Director' THEN 1
        WHEN 'Producer' THEN 2
        WHEN 'Writer' THEN 3
        WHEN 'Actor' THEN 4
        ELSE 5
    END as role_priority
FROM medias m
JOIN medias_persons_roles mpr ON m.id = mpr.media_id
JOIN persons p ON mpr.person_id = p.id
JOIN roles r ON mpr.role_id = r.id
ORDER BY m.id, role_priority, p.last_name;

-- View: Subscription Revenue Analysis
-- Shows revenue and user distribution by subscription type
CREATE OR REPLACE VIEW subscription_revenue_analysis AS
SELECT 
    s.id,
    s.name as subscription_name,
    s.price,
    COUNT(us."UsersId") as active_users,
    s.price * COUNT(us."UsersId") as total_revenue,
    ROUND(COUNT(us."UsersId") * 100.0 / SUM(COUNT(us."UsersId")) OVER (), 2) as user_percentage
FROM subscriptions s
LEFT JOIN users_subscriptions us ON s.id = us."SubscriptionsId"
GROUP BY s.id, s.name, s.price
ORDER BY total_revenue DESC;

-- =============================================
-- FUNCTIONS
-- =============================================

-- Function: Check if user can access specific content
CREATE OR REPLACE FUNCTION can_user_access_content(
    p_user_id INTEGER,
    p_media_id INTEGER
) RETURNS BOOLEAN AS $$
DECLARE
    has_access BOOLEAN := FALSE;
BEGIN
    -- Check if user has a subscription that includes any genre of the media
    SELECT EXISTS (
        SELECT 1
        FROM users_subscriptions us
        JOIN genres_subscriptions gs ON us."SubscriptionsId" = gs."SubscriptionsId"
        JOIN medias_genres mg ON gs."GenresId" = mg."GenresId"
        WHERE us."UsersId" = p_user_id 
        AND mg."MediasId" = p_media_id
    ) INTO has_access;
    
    RETURN has_access;
END;
$$ LANGUAGE plpgsql;

-- Function: Get content by genre with pagination
CREATE OR REPLACE FUNCTION get_content_by_genre(
    p_genre_name VARCHAR,
    p_offset INTEGER DEFAULT 0,
    p_limit INTEGER DEFAULT 20
) RETURNS TABLE (
    media_id INTEGER,
    media_name VARCHAR,
    media_type VARCHAR,
    runtime INTEGER,
    release_date DATE,
    average_rating NUMERIC,
    review_count BIGINT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        m.id,
        m.name,
        m.type,
        m.runtime,
        m.release,
        COALESCE(ROUND(AVG(r.rating::numeric), 2), 0) as avg_rating,
        COUNT(r.media_id) as review_cnt
    FROM medias m
    JOIN medias_genres mg ON m.id = mg."MediasId"
    JOIN genres g ON mg."GenresId" = g.id
    LEFT JOIN reviews r ON m.id = r.media_id
    WHERE g.name = p_genre_name
    GROUP BY m.id, m.name, m.type, m.runtime, m.release
    ORDER BY avg_rating DESC, review_cnt DESC, m.name
    OFFSET p_offset
    LIMIT p_limit;
END;
$$ LANGUAGE plpgsql;

-- =============================================
-- STORED PROCEDURES
-- =============================================

-- Procedure: Add media to watchlist with validation
CREATE OR REPLACE PROCEDURE add_to_watchlist(
    p_user_id INTEGER,
    p_profile_id INTEGER,
    p_media_id INTEGER
)
    LANGUAGE plpgsql AS $$
DECLARE
    watchlist_exists BOOLEAN;
    media_exists BOOLEAN;
    already_in_list BOOLEAN;
    profile_belongs_to_user BOOLEAN;
BEGIN
    -- Check if profile belongs to the user
    SELECT EXISTS(SELECT 1 FROM profiles WHERE id = p_profile_id AND user_id = p_user_id)
    INTO profile_belongs_to_user;
    IF NOT profile_belongs_to_user THEN
        RAISE EXCEPTION 'Profile not found for the given user';
    END IF;

    -- Check if media exists
    SELECT EXISTS(SELECT 1 FROM medias WHERE id = p_media_id) INTO media_exists;
    IF NOT media_exists THEN
        RAISE EXCEPTION 'Media not found';
    END IF;

    -- Check if watchlist exists, create if not
    SELECT EXISTS(SELECT 1 FROM watch_lists WHERE profile_id = p_profile_id) INTO watchlist_exists;
    IF NOT watchlist_exists THEN
        INSERT INTO watch_lists (profile_id, is_locked, created_at)
        VALUES (p_profile_id, false, NOW());
    END IF;

    -- Check if already in watchlist
    SELECT EXISTS(
        SELECT 1 FROM watch_lists_medias
        WHERE "WatchListsProfileId" = p_profile_id AND "MediasId" = p_media_id
    ) INTO already_in_list;
    IF already_in_list THEN
        RAISE EXCEPTION 'Media already in watchlist';
    END IF;

    -- Add to watchlist
    INSERT INTO watch_lists_medias ("WatchListsProfileId", "MediasId")
    VALUES (p_profile_id, p_media_id);
END;
$$;

-- Procedure: Submit review with validation
CREATE OR REPLACE PROCEDURE submit_review(
    p_profile_id INTEGER,
    p_media_id INTEGER,
    p_rating INTEGER,
    p_description TEXT DEFAULT NULL
)     LANGUAGE plpgsql AS $$
DECLARE
    existing_review BOOLEAN;
BEGIN
    -- Validate rating
    IF p_rating < 1 OR p_rating > 5 THEN
        RAISE EXCEPTION 'Rating must be between 1 and 5';
    END IF;
    
    -- Check if review already exists
    SELECT EXISTS(
        SELECT 1 FROM reviews 
        WHERE profile_id = p_profile_id AND media_id = p_media_id
    ) INTO existing_review;
    
    IF existing_review THEN
        -- Update existing review
        UPDATE reviews 
        SET rating = p_rating, 
            description = p_description, 
            created_at = NOW()
        WHERE profile_id = p_profile_id AND media_id = p_media_id;
    ELSE
        -- Insert new review
        INSERT INTO reviews (media_id, profile_id, description, rating, created_at)
        VALUES (p_media_id, p_profile_id, p_description, p_rating, NOW());
    END IF;
END;
$$;

-- =============================================
-- TRIGGERS
-- =============================================

-- Trigger function: Validate age-appropriate content for child profiles
CREATE OR REPLACE FUNCTION validate_child_content()
RETURNS TRIGGER AS $$
DECLARE
    is_child_profile BOOLEAN;
    content_age_limit INTEGER;
BEGIN
    -- Check if this is a child profile
    SELECT is_child INTO is_child_profile
    FROM profiles
    WHERE id = NEW.profile_id;
    
    -- If it's a child profile, check age limit
    IF is_child_profile THEN
        SELECT age_limit INTO content_age_limit
        FROM medias
        WHERE id = NEW.media_id;
        
        -- Prevent adding inappropriate content (age limit >= 18 for children)
        IF content_age_limit >= 18 THEN
            RAISE EXCEPTION 'Content not appropriate for child profile (Age limit: %)', content_age_limit;
        END IF;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create trigger for child content validation in reviews
DROP TRIGGER IF EXISTS child_content_review_trigger ON reviews;
CREATE TRIGGER child_content_review_trigger
    BEFORE INSERT OR UPDATE ON reviews
    FOR EACH ROW
    EXECUTE FUNCTION validate_child_content(); 

-- Create trigger for child content validation in watchlists
CREATE OR REPLACE FUNCTION validate_child_watchlist()
RETURNS TRIGGER AS $$
DECLARE
    is_child_profile BOOLEAN;
    content_age_limit INTEGER;
BEGIN
    -- Check if this is a child profile
    SELECT is_child INTO is_child_profile
    FROM profiles
    WHERE id = NEW."WatchListsProfileId";
    
    -- If it's a child profile, check age limit
    IF is_child_profile THEN
        SELECT age_limit INTO content_age_limit
        FROM medias
        WHERE id = NEW."MediasId";
        
        -- Prevent adding inappropriate content
        IF content_age_limit >= 18 THEN
            RAISE EXCEPTION 'Content not appropriate for child profile (Age limit: %)', content_age_limit;
        END IF;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS child_content_watchlist_trigger ON watch_lists_medias;
CREATE TRIGGER child_content_watchlist_trigger
    BEFORE INSERT ON watch_lists_medias
    FOR EACH ROW
    EXECUTE FUNCTION validate_child_watchlist();


-- =============================================
-- EXAMPLE USAGE AND TESTING
-- =============================================

-- Example queries to test the stored objects:

/*
-- Test the user content access view
SELECT * FROM user_content_access WHERE user_id = 1;

-- Test the popular content view
SELECT * FROM popular_content LIMIT 5;

-- Test content access function
SELECT can_user_access_content(1, 1) as can_access;

-- Test content by genre function
SELECT * FROM get_content_by_genre('Action', 0, 10);

-- Test adding to watchlist
CALL add_to_watchlist(1, 1, 5);

-- Test submitting a review
CALL submit_review(1, 5, 4, 'Great series!');

-- Test user activity summary
SELECT * FROM user_activity_summary WHERE user_id = 1;

*/