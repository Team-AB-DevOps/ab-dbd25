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
('David', 'Brown', 'david.brown@email.com', '$2a$10$hashedpassword5', NOW()),
('Emma', 'Davis', 'emma.davis@email.com', '$2a$10$hashedpassword6', NOW()),
('Robert', 'Miller', 'robert.miller@email.com', '$2a$10$hashedpassword7', NOW()),
('Lisa', 'Wilson', 'lisa.wilson@email.com', '$2a$10$hashedpassword8', NOW()),
('James', 'Moore', 'james.moore@email.com', '$2a$10$hashedpassword9', NOW()),
('Maria', 'Taylor', 'maria.taylor@email.com', '$2a$10$hashedpassword10', NOW()),
('Chris', 'Anderson', 'chris.anderson@email.com', '$2a$10$hashedpassword11', NOW()),
('Ashley', 'Thomas', 'ashley.thomas@email.com', '$2a$10$hashedpassword12', NOW()),
('Daniel', 'Jackson', 'daniel.jackson@email.com', '$2a$10$hashedpassword13', NOW()),
('Jessica', 'White', 'jessica.white@email.com', '$2a$10$hashedpassword14', NOW()),
('Michael', 'Harris', 'michael.harris@email.com', '$2a$10$hashedpassword15', NOW());

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
('Uma', 'Thurman', '1970-04-29', 'Female', NOW()),
('Tom', 'Hanks', '1956-07-09', 'Male', NOW()),
('Meryl', 'Streep', '1949-06-22', 'Female', NOW()),
('Robert', 'De Niro', '1943-08-17', 'Male', NOW()),
('Natalie', 'Portman', '1981-06-09', 'Female', NOW()),
('Brad', 'Pitt', '1963-12-18', 'Male', NOW()),
('Jennifer', 'Lawrence', '1990-08-15', 'Female', NOW()),
('Will', 'Smith', '1968-09-25', 'Male', NOW()),
('Emma', 'Stone', '1988-11-06', 'Female', NOW()),
('Denzel', 'Washington', '1954-12-28', 'Male', NOW()),
('Charlize', 'Theron', '1975-08-07', 'Female', NOW()),
('Morgan', 'Freeman', '1937-06-01', 'Male', NOW()),
('Cate', 'Blanchett', '1969-05-14', 'Female', NOW()),
('Christian', 'Bale', '1974-01-30', 'Male', NOW()),
('Amy', 'Adams', '1974-08-20', 'Female', NOW()),
('Hugh', 'Jackman', '1968-10-12', 'Male', NOW()),
('Sandra', 'Bullock', '1964-07-26', 'Female', NOW()),
('Jake', 'Gyllenhaal', '1980-12-19', 'Male', NOW()),
('Reese', 'Witherspoon', '1976-03-22', 'Female', NOW()),
('Mark', 'Wahlberg', '1971-06-05', 'Male', NOW()),
('Angelina', 'Jolie', '1975-06-04', 'Female', NOW());

-- Insert Medias
INSERT INTO medias (name, type, runtime, description, cover, age_limit, release, created_at) VALUES 
('Inception', 'Movies', 148, 'A thief who steals corporate secrets through dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.', 'https://example.com/inception.jpg', 13, '2010-07-16', NOW()),
('The Dark Knight', 'Movies', 152, 'When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.', 'https://example.com/darknight.jpg', 13, '2008-07-18', NOW()),
('Barbie', 'Movies', 114, 'Barbie and Ken are having the time of their lives in the colorful and seemingly perfect world of Barbie Land.', 'https://example.com/barbie.jpg', 7, '2023-07-21', NOW()),
('Interstellar', 'Movies', 169, 'A team of explorers travel through a wormhole in space in an attempt to ensure humanity''s survival.', 'https://example.com/interstellar.jpg', 13, '2014-11-07', NOW()),
('Stranger Things', 'Series', 45, 'When a young boy disappears, his mother, a police chief and his friends must confront terrifying supernatural forces in order to get him back.', 'https://example.com/strangerthings.jpg', 16, '2016-07-15', NOW()),
('Breaking Bad', 'Series', 47, 'A high school chemistry teacher diagnosed with inoperable lung cancer turns to manufacturing and selling methamphetamine in order to secure his family''s future.', 'https://example.com/breakingbad.jpg', 18, '2008-01-20', NOW()),
('The Office', 'Series', 22, 'A mockumentary on a group of typical office workers, where the workday consists of ego clashes, inappropriate behavior, and tedium.', 'https://example.com/theoffice.jpg', 13, '2005-03-24', NOW()),
('Avatar', 'Movies', 162, 'A paraplegic Marine dispatched to the moon Pandora on a unique mission becomes torn between following his orders and protecting the world he feels is his home.', 'https://example.com/avatar.jpg', 13, '2009-12-18', NOW()),
('The Shawshank Redemption', 'Movies', 142, 'Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.', 'https://example.com/shawshank.jpg', 15, '1994-09-23', NOW()),
('The Godfather', 'Movies', 175, 'An organized crime dynasty''s aging patriarch transfers control of his clandestine empire to his reluctant son.', 'https://example.com/godfather.jpg', 18, '1972-03-24', NOW()),
('Pulp Fiction', 'Movies', 154, 'The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.', 'https://example.com/pulpfiction.jpg', 18, '1994-10-14', NOW()),
('The Matrix', 'Movies', 136, 'A computer programmer is led to fight an underground war against powerful computers who have constructed his entire reality with a system called the Matrix.', 'https://example.com/matrix.jpg', 15, '1999-03-31', NOW()),
('Forrest Gump', 'Movies', 142, 'The presidencies of Kennedy and Johnson through the eyes of an Alabama man with an IQ of 75, whose only desire is to be reunited with his childhood sweetheart.', 'https://example.com/forrestgump.jpg', 13, '1994-07-06', NOW()),
('The Lion King', 'Movies', 88, 'A Lion cub crown prince is tricked by a treacherous uncle into thinking he caused his father''s death and flees into exile in despair, only to learn in adulthood his identity and his responsibilities.', 'https://example.com/lionking.jpg', 0, '1994-06-24', NOW()),
('Titanic', 'Movies', 194, 'A seventeen-year-old aristocrat falls in love with a kind but poor artist aboard the luxurious, ill-fated R.M.S. Titanic.', 'https://example.com/titanic.jpg', 13, '1997-12-19', NOW()),
('The Avengers', 'Movies', 143, 'Earth''s mightiest heroes must come together and learn to fight as a team if they are going to stop the mischievous Loki and his alien army from enslaving humanity.', 'https://example.com/avengers.jpg', 13, '2012-05-04', NOW()),
('Game of Thrones', 'Series', 57, 'Nine noble families fight for control over the lands of Westeros, while an ancient enemy returns after being dormant for millennia.', 'https://example.com/got.jpg', 18, '2011-04-17', NOW()),
('Friends', 'Series', 22, 'Follows the personal and professional lives of six twenty to thirty-something-year-old friends living in Manhattan.', 'https://example.com/friends.jpg', 13, '1994-09-22', NOW()),
('The Simpsons', 'Series', 22, 'The satiric adventures of a working-class family in the misfit city of Springfield.', 'https://example.com/simpsons.jpg', 7, '1989-12-17', NOW()),
('Seinfeld', 'Series', 22, 'A stand-up comedian and his three offbeat friends weather the pitfalls and payoffs of life in New York City in the ''90s.', 'https://example.com/seinfeld.jpg', 13, '1989-07-05', NOW()),
('The Walking Dead', 'Series', 44, 'Sheriff Deputy Rick Grimes wakes up from a coma to learn the world is in ruins and must lead a group of survivors to stay alive.', 'https://example.com/walkingdead.jpg', 18, '2010-10-31', NOW()),
('Lost', 'Series', 43, 'The survivors of a plane crash are forced to work together in order to survive on a seemingly deserted tropical island.', 'https://example.com/lost.jpg', 15, '2004-09-22', NOW()),
('House of Cards', 'Series', 51, 'A Congressman works with his equally conniving wife to exact revenge on the people who betrayed him.', 'https://example.com/houseofcards.jpg', 18, '2013-02-01', NOW()),
('Black Mirror', 'Series', 60, 'An anthology series exploring a twisted, high-tech multiverse where humanity''s greatest innovations and darkest instincts collide.', 'https://example.com/blackmirror.jpg', 18, '2011-12-04', NOW()),
('Narcos', 'Series', 49, 'A chronicled look at the criminal exploits of Colombian drug lord Pablo Escobar, as well as the many other drug kingpins who plagued the country through the years.', 'https://example.com/narcos.jpg', 18, '2015-08-28', NOW());

-- Insert Profiles
INSERT INTO profiles (user_id, name, is_child, created_at) VALUES 
(1, 'John Main', false, NOW()),
(1, 'Johnny Jr', true, NOW()),
(2, 'Jane Profile', false, NOW()),
(2, 'Kids Profile', true, NOW()),
(3, 'Mike Personal', false, NOW()),
(4, 'Sarah Main', false, NOW()),
(4, 'Family Kids', true, NOW()),
(5, 'David Profile', false, NOW()),
(6, 'Emma Main', false, NOW()),
(6, 'Emma Kids', true, NOW()),
(7, 'Robert Profile', false, NOW()),
(8, 'Lisa Main', false, NOW()),
(8, 'Lisa Family', true, NOW()),
(9, 'James Personal', false, NOW()),
(10, 'Maria Profile', false, NOW()),
(10, 'Maria Children', true, NOW()),
(11, 'Chris Main', false, NOW()),
(12, 'Ashley Profile', false, NOW()),
(12, 'Ashley Kids', true, NOW()),
(13, 'Daniel Personal', false, NOW()),
(14, 'Jessica Main', false, NOW()),
(14, 'Jessica Family', true, NOW()),
(15, 'Michael Profile', false, NOW()),
(15, 'Michael Kids', true, NOW());

-- Insert Episodes (for series)
INSERT INTO episodes (media_id, name, season_count, episode_count, runtime, description, release, created_at) VALUES 
-- Stranger Things Episodes
(5, 'The Vanishing of Will Byers', 1, 1, 47, 'On his way home from a friend''s house, young Will sees something terrifying. Nearby, a sinister secret lurks in the depths of a government lab.', '2016-07-15', NOW()),
(5, 'The Weirdo on Maple Street', 1, 2, 56, 'Lucas, Mike and Dustin try to talk to the girl they found in the woods. Hopper questions an anxious Joyce about an unsettling phone call.', '2016-07-15', NOW()),
(5, 'Holly, Jolly', 1, 3, 52, 'An increasingly concerned Nancy looks for Barb and finds out what Jonathan''s been up to. Joyce is convinced Will is trying to talk to her.', '2016-07-15', NOW()),
(5, 'The Body', 1, 4, 49, 'Hopper discovers the truth behind the lab experiments. The boys team up with Eleven to save their friend.', '2016-07-15', NOW()),
(5, 'MADMAX', 2, 1, 48, 'One year later, the boys are still reeling from their encounter with the Upside Down, and new mysteries begin to emerge.', '2017-10-27', NOW()),
(5, 'Trick or Treat, Freak', 2, 2, 56, 'After Will sees something terrible on Halloween night, Mike wonders whether Eleven is still out there.', '2017-10-27', NOW()),
-- Breaking Bad Episodes
(6, 'Pilot', 1, 1, 58, 'Walter White, a struggling high school chemistry teacher, is diagnosed with lung cancer. He turns to a life of crime, producing and selling methamphetamine.', '2008-01-20', NOW()),
(6, 'Cat''s in the Bag...', 1, 2, 48, 'Walt and Jesse attempt to tie up loose ends. The desperate situation gets more complicated with the flip of a coin.', '2008-01-27', NOW()),
(6, '...And the Bag''s in the River', 1, 3, 48, 'Walt and Jesse clean up after the bathtub incident before Walt decides what to do with their prisoner.', '2008-02-10', NOW()),
(6, 'Cancer Man', 1, 4, 48, 'Walt tells his family about his cancer diagnosis. Jesse tries to make a deal with Tuco.', '2008-02-17', NOW()),
-- The Office Episodes
(7, 'Pilot', 1, 1, 22, 'A documentary crew arrives at the offices of Dunder Mifflin to observe the employees and their daily interactions.', '2005-03-24', NOW()),
(7, 'Diversity Day', 1, 2, 22, 'Michael''s off color remark puts a sensitivity trainer in the office for a presentation, which prompts Michael to create his own.', '2005-03-29', NOW()),
(7, 'Health Care', 1, 3, 22, 'Michael leaves Dwight in charge of picking the new health care plan for the staff.', '2005-04-05', NOW()),
(7, 'The Alliance', 1, 4, 22, 'Just as Jim gets a big client to agree to a deal, Dwight tries to convince him to join an alliance.', '2005-04-12', NOW()),
-- Game of Thrones Episodes
(17, 'Winter Is Coming', 1, 1, 62, 'Eddard Stark is torn between his family and an old friend when asked to serve at the side of King Robert Baratheon.', '2011-04-17', NOW()),
(17, 'The Kingsroad', 1, 2, 56, 'While Bran recovers from his fall, Ned takes only his daughters to King''s Landing.', '2011-04-24', NOW()),
(17, 'Lord Snow', 1, 3, 58, 'Jon begins his training with the Night''s Watch; Ned confronts his past and future at King''s Landing.', '2011-05-01', NOW()),
-- Friends Episodes
(18, 'The Pilot', 1, 1, 22, 'Monica and the gang introduce Rachel to the "real world" after she leaves her fiancé at the altar.', '1994-09-22', NOW()),
(18, 'The One with the Sonogram at the End', 1, 2, 22, 'Ross finds out his ex-wife is pregnant. Rachel returns her engagement ring to Barry.', '1994-09-29', NOW()),
(18, 'The One with the Thumb', 1, 3, 22, 'Monica becomes irritated when everyone likes her new boyfriend more than she does.', '1994-10-06', NOW()),
-- Black Mirror Episodes
(24, 'The National Anthem', 1, 1, 44, 'Prime Minister Michael Callow faces a shocking dilemma when Princess Susannah, a much-loved member of the Royal Family, is kidnapped.', '2011-12-04', NOW()),
(24, 'Fifteen Million Merits', 1, 2, 62, 'In a world where people''s lives consist of riding exercise bikes to gain credits, Bing tries to help a girl achieve her dreams.', '2011-12-11', NOW()),
(24, 'The Entire History of You', 1, 3, 49, 'In the near future, everyone has access to a memory implant that records everything they do, see and hear.', '2011-12-18', NOW());

-- Insert Watch Lists
INSERT INTO watch_lists (profile_id, is_locked, created_at) VALUES 
(1, false, NOW()),
(2, true, NOW()),
(3, false, NOW()),
(4, true, NOW()),
(5, false, NOW()),
(6, false, NOW()),
(7, true, NOW()),
(8, false, NOW()),
(9, false, NOW()),
(10, true, NOW()),
(11, false, NOW()),
(12, false, NOW()),
(13, true, NOW()),
(14, false, NOW()),
(15, false, NOW()),
(16, true, NOW()),
(17, false, NOW()),
(18, false, NOW()),
(19, true, NOW()),
(20, false, NOW()),
(21, false, NOW()),
(22, true, NOW()),
(23, false, NOW()),
(24, true, NOW());

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
(8, 3, 'Visually stunning but the story was a bit predictable.', 3, NOW()),
(9, 9, 'A masterpiece of cinema. Morgan Freeman''s narration is perfect.', 5, NOW()),
(10, 11, 'Epic family saga. Marlon Brando delivers an unforgettable performance.', 5, NOW()),
(11, 12, 'Tarantino at his finest. Clever dialogue and great storytelling.', 5, NOW()),
(12, 14, 'Mind-blowing action and philosophical depth. Keanu Reeves is iconic.', 4, NOW()),
(13, 15, 'Tom Hanks is incredible. A heartwarming story about life.', 5, NOW()),
(14, 16, 'Beautiful animation and touching story. Perfect for kids.', 5, NOW()),
(15, 17, 'Epic romance and disaster. Leo and Kate have amazing chemistry.', 4, NOW()),
(16, 18, 'Great superhero ensemble. Action-packed and fun.', 4, NOW()),
(17, 20, 'Complex political drama with amazing characters. Winter is coming!', 5, NOW()),
(18, 21, 'Timeless comedy about friendship. Still funny after all these years.', 4, NOW()),
(19, 22, 'Hilarious satire that has influenced comedy for decades.', 4, NOW()),
(20, 23, 'About nothing and everything. Jerry and the gang are iconic.', 4, NOW()),
(21, 24, 'Thrilling zombie apocalypse series. Rick Grimes is a great leader.', 4, NOW()),
(22, 1, 'Mystery island adventure. So many twists and turns!', 4, NOW()),
(23, 3, 'Political thriller at its finest. Kevin Spacey is brilliant.', 5, NOW()),
(24, 5, 'Dark and thought-provoking. Makes you question technology.', 5, NOW()),
(1, 9, 'Christopher Nolan''s best work. The dream sequences are incredible.', 5, NOW()),
(2, 11, 'Dark and gritty superhero film. Christian Bale is the perfect Batman.', 5, NOW()),
(4, 12, 'Space epic that makes you think about humanity''s future.', 4, NOW()),
(5, 14, 'Great 80s nostalgia with supernatural elements.', 4, NOW()),
(6, 15, 'Addictive crime drama. Walter White''s transformation is amazing.', 5, NOW());

-- Insert Many-to-Many relationships

-- Users and Privileges
INSERT INTO users_privileges ("UsersId", "PrivilegesId") VALUES 
(1, 2), -- John: USER
(2, 2), -- Jane: USER
(3, 2), -- Mike: USER
(4, 1), -- Sarah: ADMIN
(5, 2), -- David: USER
(6, 2), -- Emma: USER
(7, 1), -- Robert: ADMIN
(8, 2), -- Lisa: USER
(9, 2), -- James: USER
(10, 2), -- Maria: USER
(11, 2), -- Chris: USER
(12, 1), -- Ashley: ADMIN
(13, 2), -- Daniel: USER
(14, 2), -- Jessica: USER
(15, 2); -- Michael: USER

-- Users and Subscriptions
INSERT INTO users_subscriptions ("UsersId", "SubscriptionsId") VALUES 
(1, 1), -- John: Basic Entertainment
(2, 3), -- Jane: Premium All Access
(3, 2), -- Mike: Action & Thriller
(4, 4), -- Sarah: Family & Animation
(5, 5), -- David: Horror & Sci-Fi
(6, 1), -- Emma: Basic Entertainment
(7, 3), -- Robert: Premium All Access
(8, 2), -- Lisa: Action & Thriller
(9, 6), -- James: Documentary
(10, 4), -- Maria: Family & Animation
(11, 5), -- Chris: Horror & Sci-Fi
(12, 3), -- Ashley: Premium All Access
(13, 1), -- Daniel: Basic Entertainment
(14, 2), -- Jessica: Action & Thriller
(15, 4); -- Michael: Family & Animation

-- Subscriptions and Genres (which genres each subscription unlocks)
INSERT INTO genres_subscriptions ("GenresId", "SubscriptionsId") VALUES 
-- Basic Entertainment: Comedy, Drama
(2, 1), (3, 1),
-- Action & Thriller: Action, Thriller, Crime
(1, 2), (7, 2), (10, 2),
-- Premium All Access: All genres
(1, 3), (2, 3), (3, 3), (4, 3), (5, 3), (6, 3), (7, 3), (8, 3), (9, 3), (10, 3),
-- Family & Animation: Animation, Comedy, Romance
(9, 4), (2, 4), (6, 4),
-- Horror & Sci-Fi: Horror, Sci-Fi
(4, 5), (5, 5),
-- Documentary: Documentary
(8, 6);

-- Medias and Genres
INSERT INTO medias_genres ("GenresId", "MediasId") VALUES 
(1, 1), (5, 1), (7, 1), -- Inception: Action, Sci-Fi, Thriller
(1, 2), (10, 2), (7, 2), -- The Dark Knight: Action, Crime, Thriller
(2, 3), (6, 3), -- Barbie: Comedy, Romance
(3, 4), (5, 4), -- Interstellar: Drama, Sci-Fi
(3, 5), (4, 5), (5, 5), -- Stranger Things: Drama, Horror, Sci-Fi
(3, 6), (10, 6), (7, 6), -- Breaking Bad: Drama, Crime, Thriller
(2, 7), (3, 7), -- The Office: Comedy, Drama
(1, 8), (5, 8), (3, 8), -- Avatar: Action, Sci-Fi, Drama
(3, 9), (10, 9), -- The Shawshank Redemption: Drama, Crime
(3, 10), (10, 10), -- The Godfather: Drama, Crime
(10, 11), (7, 11), -- Pulp Fiction: Crime, Thriller
(1, 12), (5, 12), -- The Matrix: Action, Sci-Fi
(3, 13), (6, 13), -- Forrest Gump: Drama, Romance
(9, 14), (3, 14), -- The Lion King: Animation, Drama
(3, 15), (6, 15), -- Titanic: Drama, Romance
(1, 16), (5, 16), -- The Avengers: Action, Sci-Fi
(3, 17), (1, 17), -- Game of Thrones: Drama, Action
(2, 18), (3, 18), -- Friends: Comedy, Drama
(2, 19), (9, 19), -- The Simpsons: Comedy, Animation
(2, 20), -- Seinfeld: Comedy
(3, 21), (4, 21), -- The Walking Dead: Drama, Horror
(3, 22), (5, 22), -- Lost: Drama, Sci-Fi
(3, 23), (7, 23), -- House of Cards: Drama, Thriller
(5, 24), (7, 24); -- Black Mirror: Sci-Fi, Thriller

-- Media Person Roles
INSERT INTO medias_persons_roles (media_id, person_id, role_id) VALUES 
-- Inception
(1, 1, 2), -- Leonardo DiCaprio as Actor
(1, 3, 1), -- Christopher Nolan as Director
(1, 3, 4), -- Christopher Nolan as Writer
-- The Dark Knight
(2, 23, 2), -- Christian Bale as Actor
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
(8, 2, 8), -- Scarlett Johansson as Voice Actor (fictional)
-- The Shawshank Redemption
(9, 21, 2), -- Morgan Freeman as Actor
(9, 11, 2), -- Tom Hanks as Actor (fictional casting)
-- The Godfather
(10, 13, 2), -- Robert De Niro as Actor
(10, 15, 2), -- Brad Pitt as Actor (fictional casting)
-- Pulp Fiction
(11, 9, 1), -- Quentin Tarantino as Director
(11, 9, 4), -- Quentin Tarantino as Writer
(11, 10, 2), -- Uma Thurman as Actor
-- The Matrix
(12, 17, 2), -- Will Smith as Actor (fictional casting)
(12, 20, 2), -- Charlize Theron as Actor (fictional casting)
-- Forrest Gump
(13, 11, 2), -- Tom Hanks as Actor
-- The Lion King
(14, 17, 8), -- Will Smith as Voice Actor (fictional)
-- Titanic
(15, 1, 2), -- Leonardo DiCaprio as Actor
(15, 18, 2), -- Emma Stone as Actor (fictional casting)
-- The Avengers
(16, 2, 2), -- Scarlett Johansson as Actor
(16, 19, 2), -- Denzel Washington as Actor (fictional casting)
-- Game of Thrones
(17, 15, 2), -- Brad Pitt as Actor (fictional casting)
(17, 22, 2), -- Cate Blanchett as Actor (fictional casting)
-- Friends
(18, 16, 2), -- Jennifer Lawrence as Actor (fictional casting)
(18, 24, 2), -- Amy Adams as Actor (fictional casting)
-- The Simpsons
(19, 25, 8), -- Hugh Jackman as Voice Actor (fictional)
-- The Walking Dead
(21, 19, 2), -- Denzel Washington as Actor (fictional casting)
-- Lost
(22, 27, 2), -- Jake Gyllenhaal as Actor (fictional casting)
-- House of Cards
(23, 29, 2), -- Mark Wahlberg as Actor (fictional casting)
-- Black Mirror
(24, 22, 2), -- Cate Blanchett as Actor (fictional casting)
-- Additional roles for existing media
(1, 8, 2), -- Anne Hathaway in Inception (fictional)
(2, 21, 2), -- Morgan Freeman in The Dark Knight (fictional)
(4, 14, 2), -- Natalie Portman in Interstellar (fictional)
(5, 25, 2), -- Hugh Jackman in Stranger Things (fictional)
(6, 30, 2), -- Angelina Jolie in Breaking Bad (fictional)
(7, 26, 2), -- Sandra Bullock in The Office (fictional)
(8, 12, 2); -- Meryl Streep in Avatar (fictional)

-- Watch Lists and Medias
INSERT INTO watch_lists_medias ("WatchListsProfileId", "MediasId") VALUES 
-- Original watchlists (expanded)
(1, 1), (1, 2), (1, 4), (1, 9), (1, 12), -- John Main's watchlist
(2, 3), (2, 7), (2, 14), (2, 19), -- Johnny Jr's watchlist (child-friendly)
(3, 1), (3, 5), (3, 6), (3, 17), (3, 23), -- Jane's watchlist
(4, 3), (4, 7), (4, 14), (4, 18), -- Kids Profile watchlist (child-friendly)
(5, 2), (5, 4), (5, 8), (5, 11), (5, 16), -- Mike's watchlist
(6, 1), (6, 3), (6, 5), (6, 15), (6, 22), -- Sarah's watchlist
(7, 3), (7, 7), (7, 14), (7, 19), -- Family Kids watchlist (child-friendly)
(8, 6), (8, 8), (8, 21), (8, 24), -- David's watchlist
-- New users' watchlists
(9, 10), (9, 11), (9, 13), (9, 15), (9, 20), -- Emma Main's watchlist
(10, 3), (10, 14), (10, 18), (10, 19), -- Emma Kids watchlist (child-friendly)
(11, 2), (11, 4), (11, 16), (11, 22), (11, 24), -- Robert's watchlist
(12, 1), (12, 9), (12, 12), (12, 13), (12, 15), -- Lisa Main's watchlist
(13, 3), (13, 7), (13, 14), (13, 18), -- Lisa Family watchlist (child-friendly)
(14, 5), (14, 6), (14, 17), (14, 21), (14, 23), -- James Personal's watchlist
(15, 8), (15, 10), (15, 11), (15, 16), (15, 20), -- Maria's watchlist
(16, 3), (16, 14), (16, 18), (16, 19), -- Maria Children watchlist (child-friendly)
(17, 2), (17, 4), (17, 12), (17, 22), (17, 24), -- Chris Main's watchlist
(18, 1), (18, 9), (18, 13), (18, 15), (18, 17), -- Ashley's watchlist
(19, 3), (19, 7), (19, 14), (19, 18), -- Ashley Kids watchlist (child-friendly)
(20, 5), (20, 6), (20, 21), (20, 23), (20, 24), -- Daniel Personal's watchlist
(21, 8), (21, 10), (21, 11), (21, 16), (21, 20), -- Jessica Main's watchlist
(22, 3), (22, 14), (22, 18), (22, 19), -- Jessica Family watchlist (child-friendly)
(23, 1), (23, 2), (23, 4), (23, 12), (23, 22), -- Michael's watchlist
(24, 3), (24, 7), (24, 14), (24, 18); -- Michael Kids watchlist (child-friendly)
