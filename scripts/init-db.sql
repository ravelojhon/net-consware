-- Initialize TravelRequests Database
-- This script creates the database if it doesn't exist

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TravelRequestsDb')
BEGIN
    CREATE DATABASE [TravelRequestsDb];
    PRINT 'Database TravelRequestsDb created successfully.';
END
ELSE
BEGIN
    PRINT 'Database TravelRequestsDb already exists.';
END

-- Set the database to use
USE [TravelRequestsDb];

-- Create a user for the application (optional, can use sa for development)
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'TravelRequestsUser')
BEGIN
    CREATE LOGIN [TravelRequestsUser] WITH PASSWORD = 'YourStrong@Passw0rd';
    CREATE USER [TravelRequestsUser] FOR LOGIN [TravelRequestsUser];
    ALTER ROLE [db_owner] ADD MEMBER [TravelRequestsUser];
    PRINT 'User TravelRequestsUser created successfully.';
END
ELSE
BEGIN
    PRINT 'User TravelRequestsUser already exists.';
END

PRINT 'Database initialization completed.';
