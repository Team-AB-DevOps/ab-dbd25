-- Dummy data for the streaming platform database

-- Insert Genres
INSERT INTO genres (name) VALUES 
('Action'),
('Comedy'),
('Drama'),
('Horror'),
('Sci-Fi'),
('Romance'),
('Thriller'),
('Documentary'),
('Animation'),
('Crime');

-- Insert Privileges
INSERT INTO privileges (name) VALUES 
('ADMIN'),
('USER');

-- Insert Roles
INSERT INTO roles (name) VALUES 
('Director'),
('Actor'),
('Producer'),
('Writer'),
('Cinematographer'),
('Editor'),
('Composer'),
('Voice Actor');

-- Insert Subscriptions
INSERT INTO subscriptions (name, price) VALUES 
('Basic Entertainment', 100),     -- Comedy, Drama
('Action & Thriller', 100),      -- Action, Thriller, Crime
('Premium All Access', 300),     -- All genres
('Family & Animation', 50),     -- Animation, Comedy, Romance
('Horror & Sci-Fi', 100),        -- Horror, Sci-Fi
('Documentary', 50);

-- Insert Users
INSERT INTO users (first_name, last_name, email, password, created_at) VALUES 
('John', 'Doe', 'john.doe@email.com', '$2a$10$hashedpassword1', NOW()),
('Jane', 'Smith', 'jane.smith@email.com', '$2a$10$hashedpassword2', NOW()),
('Mike', 'Johnson', 'mike.johnson@email.com', '$2a$10$hashedpassword3', NOW()),
('Sarah', 'Williams', 'sarah.williams@email.com', '$2a$10$hashedpassword4', NOW()),
('David', 'Brown', 'david.brown@email.com', '$2a$10$hashedpassword5', NOW());

-- Insert Persons (Actors, Directors, etc.)
INSERT INTO persons (first_name, last_name, birth_date, gender, created_at) VALUES 
('Leonardo', 'DiCaprio', '1974-11-11', 'Male', NOW()),
('Scarlett', 'Johansson', '1984-11-22', 'Female', NOW()),
('Christopher', 'Nolan', '1970-07-30', 'Male', NOW()),
('Margot', 'Robbie', '1990-07-02', 'Female', NOW()),
('Ryan', 'Gosling', '1980-11-12', 'Male', NOW()),
('Greta', 'Gerwig', '1983-08-04', 'Female', NOW()),
('Matthew', 'McConaughey', '1969-11-04', 'Male', NOW()),
('Anne', 'Hathaway', '1982-11-12', 'Female', NOW()),
('Quentin', 'Tarantino', '1963-03-27', 'Male', NOW()),
('Uma', 'Thurman', '1970-04-29', 'Female', NOW());

-- Insert Medias
INSERT INTO medias (name, type, runtime, description, cover, age_limit, release, created_at) VALUES 
('Inception', 'Movies', 148, 'A thief who steals corporate secrets through dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.', 'https://example.com/inception.jpg', 13, '2010-07-16', NOW()),
('The Dark Knight', 'Movies', 152, 'When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.', 'https://example.com/darknight.jpg', 13, '2008-07-18', NOW()),
('Barbie', 'Movies', 114, 'Barbie and Ken are having the time of their lives in the colorful and seemingly perfect world of Barbie Land.', 'https://example.com/barbie.jpg', 7, '2023-07-21', NOW()),
('Interstellar', 'Movies', 169, 'A team of explorers travel through a wormhole in space in an attempt to ensure humanity''s survival.', 'https://example.com/interstellar.jpg', 13, '2014-11-07', NOW()),
('Stranger Things', 'Series', 45, 'When a young boy disappears, his mother, a police chief and his friends must confront terrifying supernatural forces in order to get him back.', 'https://example.com/strangerthings.jpg', 16, '2016-07-15', NOW()),
('Breaking Bad', 'Series', 47, 'A high school chemistry teacher diagnosed with inoperable lung cancer turns to manufacturing and selling methamphetamine in order to secure his family''s future.', 'https://example.com/breakingbad.jpg', 18, '2008-01-20', NOW()),
('The Office', 'Series', 22, 'A mockumentary on a group of typical office workers, where the workday consists of ego clashes, inappropriate behavior, and tedium.', 'https://example.com/theoffice.jpg', 13, '2005-03-24', NOW()),
('Avatar', 'Movies', 162, 'A paraplegic Marine dispatched to the moon Pandora on a unique mission becomes torn between following his orders and protecting the world he feels is his home.', 'https://example.com/avatar.jpg', 13, '2009-12-18', NOW());

-- Insert Profiles
INSERT INTO profiles (user_id, name, is_child, created_at) VALUES 
(1, 'John Main', false, NOW()),
(1, 'Johnny Jr', true, NOW()),
(2, 'Jane Profile', false, NOW()),
(2, 'Kids Profile', true, NOW()),
(3, 'Mike Personal', false, NOW()),
(4, 'Sarah Main', false, NOW()),
(4, 'Family Kids', true, NOW()),
(5, 'David Profile', false, NOW());

-- Insert Episodes (for series)
INSERT INTO episodes (media_id, name, season_count, episode_count, runtime, description, release, created_at) VALUES 
(5, 'The Vanishing of Will Byers', 1, 1, 47, 'On his way home from a friend''s house, young Will sees something terrifying. Nearby, a sinister secret lurks in the depths of a government lab.', '2016-07-15', NOW()),
(5, 'The Weirdo on Maple Street', 1, 2, 56, 'Lucas, Mike and Dustin try to talk to the girl they found in the woods. Hopper questions an anxious Joyce about an unsettling phone call.', '2016-07-15', NOW()),
(5, 'Holly, Jolly', 1, 3, 52, 'An increasingly concerned Nancy looks for Barb and finds out what Jonathan''s been up to. Joyce is convinced Will is trying to talk to her.', '2016-07-15', NOW()),
(6, 'Pilot', 1, 1, 58, 'Walter White, a struggling high school chemistry teacher, is diagnosed with lung cancer. He turns to a life of crime, producing and selling methamphetamine.', '2008-01-20', NOW()),
(6, 'Cat''s in the Bag...', 1, 2, 48, 'Walt and Jesse attempt to tie up loose ends. The desperate situation gets more complicated with the flip of a coin.', '2008-01-27', NOW()),
(7, 'Pilot', 1, 1, 22, 'A documentary crew arrives at the offices of Dunder Mifflin to observe the employees and their daily interactions.', '2005-03-24', NOW()),
(7, 'Diversity Day', 1, 2, 22, 'Michael''s off color remark puts a sensitivity trainer in the office for a presentation, which prompts Michael to create his own.', '2005-03-29', NOW());

-- Insert Watch Lists
INSERT INTO watch_lists (profile_id, is_locked, created_at) VALUES 
(1, false, NOW()),
(2, true, NOW()),
(3, false, NOW()),
(4, true, NOW()),
(5, false, NOW()),
(6, false, NOW()),
(7, true, NOW()),
(8, false, NOW());

-- Insert Reviews
INSERT INTO reviews (media_id, profile_id, description, rating, created_at) VALUES 
(1, 1, 'Amazing movie with incredible visuals and a mind-bending plot!', 5, NOW()),
(1, 3, 'Great concept but a bit confusing at times.', 4, NOW()),
(2, 1, 'Best Batman movie ever made. Heath Ledger was phenomenal!', 5, NOW()),
(3, 2, 'Fun and colorful! Perfect for the whole family.', 4, NOW()),
(3, 4, 'My kids loved it! Great message about being yourself.', 5, NOW()),
(4, 5, 'Emotionally powerful and scientifically fascinating.', 5, NOW()),
(5, 6, 'Nostalgic and thrilling. Reminds me of the 80s!', 4, NOW()),
(6, 8, 'Intense and well-written. Bryan Cranston is incredible.', 5, NOW()),
(7, 1, 'Hilarious workplace comedy. Steve Carell is perfect.', 4, NOW()),
(8, 3, 'Visually stunning but the story was a bit predictable.', 3, NOW());

-- Insert Many-to-Many relationships

-- Users and Privileges
INSERT INTO users_privileges ("UsersId", "PrivilegesId") VALUES 
(1, 2), -- John: USER
(2, 2), -- Jane: USER
(3, 2), -- Mike: USER
(4, 1), -- Sarah: ADMIN
(5, 2); -- David: USER

-- Users and Subscriptions
INSERT INTO users_subscriptions ("UsersId", "SubscriptionsId") VALUES 
(1, 1), -- John: Basic Entertainment
(2, 3), -- Jane: Premium All Access
(3, 2), -- Mike: Action & Thriller
(4, 4), -- Sarah: Family & Animation
(5, 5); -- David: Horror & Sci-Fi

-- Medias and Genres
INSERT INTO medias_genres ("GenresId", "MediasId") VALUES 
(1, 1), (5, 1), (7, 1), -- Inception: Action, Sci-Fi, Thriller
(1, 2), (10, 2), (7, 2), -- The Dark Knight: Action, Crime, Thriller
(2, 3), (6, 3), -- Barbie: Comedy, Romance
(3, 4), (5, 4), -- Interstellar: Drama, Sci-Fi
(3, 5), (4, 5), (5, 5), -- Stranger Things: Drama, Horror, Sci-Fi
(3, 6), (10, 6), (7, 6), -- Breaking Bad: Drama, Crime, Thriller
(2, 7), (3, 7), -- The Office: Comedy, Drama
(1, 8), (5, 8), (3, 8); -- Avatar: Action, Sci-Fi, Drama

-- Media Person Roles
INSERT INTO medias_persons_roles (media_id, person_id, role_id) VALUES 
-- Inception
(1, 1, 2), -- Leonardo DiCaprio as Actor
(1, 3, 1), -- Christopher Nolan as Director
(1, 3, 4), -- Christopher Nolan as Writer
-- The Dark Knight
(2, 3, 1), -- Christopher Nolan as Director
(2, 3, 4), -- Christopher Nolan as Writer
-- Barbie
(3, 4, 2), -- Margot Robbie as Actor
(3, 5, 2), -- Ryan Gosling as Actor
(3, 6, 1), -- Greta Gerwig as Director
(3, 6, 4), -- Greta Gerwig as Writer
-- Interstellar
(4, 7, 2), -- Matthew McConaughey as Actor
(4, 8, 2), -- Anne Hathaway as Actor
(4, 3, 1), -- Christopher Nolan as Director
-- Stranger Things
(5, 2, 2), -- Scarlett Johansson as Actor (fictional casting)
-- Breaking Bad
(6, 7, 2), -- Matthew McConaughey as Actor (fictional casting)
-- Avatar
(8, 2, 8); -- Scarlett Johansson as Voice Actor (fictional)

-- Watch Lists and Medias
INSERT INTO watch_lists_medias ("WatchListsProfileId", "MediasId") VALUES 
(1, 1), (1, 2), (1, 4), -- John Main's watchlist
(2, 3), (2, 7), -- Johnny Jr's watchlist
(3, 1), (3, 5), (3, 6), -- Jane's watchlist
(4, 3), (4, 7), -- Kids Profile watchlist
(5, 2), (5, 4), (5, 8), -- Mike's watchlist
(6, 1), (6, 3), (6, 5), -- Sarah's watchlist
(7, 3), (7, 7), -- Family Kids watchlist
(8, 6), (8, 8); -- David's watchlist
