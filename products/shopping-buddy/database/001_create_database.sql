/*
  BookMyShoppingBuddy — create database
  Run in SSMS or: sqlcmd -S "(localdb)\MSSQLLocalDB" -i 001_create_database.sql
*/
IF DB_ID(N'ShoppingBuddy') IS NULL
BEGIN
    CREATE DATABASE ShoppingBuddy;
END
GO
