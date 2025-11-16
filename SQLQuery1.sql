-- DATABASE CREATION SECTION
USE master
If EXISTS (SELECT * FROM sys.databases where name = 'EventEaseDB')
DROP DATABASE EventEaseDB
CREATE DATABASE EventEaseDB
USE EventEaseDB

-- TABLE CREATION SECTION
CREATE TABLE Venue (
VenueID INT IDENTITY(1,1) PRIMARY Key NOT NULL,          --the ID starts at 1 and increments by 1
VenueName VARCHAR (50) Unique NOT NULL,
[Location] VARCHAR (20) NOT NULL,
Capacity INT NOT NULL,
VenueImage VARCHAR (MAX) NOT NULL          --using a placholder url to the image, thus the varchar data type
);

CREATE TABLE [Event] (
EventID INT IDENTITY (1,1) PRIMARY KEY NOT NULL,     --the identity keyword tells the script that it's an ID, and auto increments
EventName VARCHAR (150) NOT NULL,
[Description] VARCHAR (MAX) NOT NULL,
EventDate DATE NOT NULL
);

CREATE TABLE Booking (
BookingID INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
VenueID INT FOREIGN KEY REFERENCES Venue (VenueID) NOT NULL,                             --
EventID INT FOREIGN KEY references [Event] (EventID) NOT NULL,
BookingDate DATE NOT NULL
);

CREATE TABLE EventType (
    EventTypeId INT IDENTITY(1,1) PRIMARY KEY,
    TypeName VARCHAR(50) NOT NULL
);

-- TABLE ALTERATIN SECTION
ALTER TABLE [Event]
ADD EventTypeId INT FOREIGN KEY REFERENCES EventType(EventTypeId);

ALTER TABLE Venue
ADD IsAvailable BIT NOT NULL DEFAULT 1;

-- INSERTION Of Sample data SECTION
INSERT INTO Venue (VenueName, [Location], Capacity, VenueImage) -- it autogenerates the ID, so no  need to add it because the system will add it because of the identity keyword 
values ('The Station', 'Newtown',1500, 'https://joburg.co.za/wp-content/uploads/2019/09/Beanstalk_CottonFest18_JonxPillemer-685.jpg'),
('FNB Stadium', 'Nasrec',95000, 'https://image-prod.iol.co.za/16x9/800/Chris-Brown-dazzled-fans-with-two-sold-out-performances-at-the-FNB-Stadium-in-Nasrec-Picture-Social-Media?source=https://iol-prod.appspot.com/image/dd2dd6426ff816f14aad3f83594e8b2b280f2e3a/1500&operation=CROP&offset=0x578&resize=1500x844')

INSERT INTO [Event] (EventName, [Description], EventDate)
values ('Cotton Fest', 'Cotton Fest showcases the diversities in music, it also celebrates various lifestyle elements within the "culture" including art, and fashion.', '2025-10-11'),
('Circus Maximus', 'CIRCUS MAXIMUS WORLD TOUR', '2025-10-11')

INSERT INTO Booking (VenueID, EventID, BookingDate)
values (1, 1, '2025-04-26'),
( 2, 2, '2025-11-10')

INSERT INTO EventType (TypeName)
VALUES ('Conference'), 
('Concert'), 
('Exhibition');

--TABLE MANIPULATION SECTION
SELECT * FROM Venue
SELECT * FROM [Event]
SELECT * FROM Booking
SELECT * FROM EventType

SELECT b.BookingID, b.BookingDate, e.EventID, e.EventName, e.EventDate, e.[Description] AS EventDescription, v.VenueID, v.VenueName, v.[Location],v.Capacity,
    v.VenueImage

FROM Booking b
JOIN [Event] e ON b.EventID = e.EventID
JOIN Venue v ON b.VenueID = v.VenueID;

GO

CREATE VIEW BookingView AS
SELECT b.BookingID, b.BookingDate, e.EventID, e.EventName, e.EventDate, e.[Description] AS EventDescription, v.VenueID, v.VenueName, v.[Location],v.Capacity,
    v.VenueImage

FROM Booking b
JOIN [Event] e ON b.EventID = e.EventID
JOIN Venue v ON b.VenueID = v.VenueID;

GO

SELECT * FROM BookingView


--STORED PROCEDURES
