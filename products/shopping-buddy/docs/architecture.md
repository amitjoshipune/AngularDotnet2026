# Architecture

Last Updated : 21-Jun-2026


# System Overview

The application is a web-first Progressive Web Application built using
Angular 15.

Later it can be packaged as an Android application using Capacitor.


--------------------------------------------------

Presentation Layer


Angular 15

Bootstrap 5

TypeScript

RxJS


Features


Login


Registration


Profile


Search


Availability


Booking


Messaging


Notifications


Ratings


Reports



--------------------------------------------------

Backend Layer


ASP.NET Core 8 Web API


Modules


Authentication API


User API


Availability API


Booking API


Chat API


Notification API


Moderation API




--------------------------------------------------

Database Layer


SQL Server 2022



Tables


Users


Profiles


AvailabilitySlots


Bookings


Messages


Reports


Ratings


Notifications





--------------------------------------------------

Authentication


JWT


Refresh Token


Email Verification


Mobile OTP




--------------------------------------------------

Notification Layer


Firebase Cloud Messaging




--------------------------------------------------

AI Moderation


Keyword Detection


Spam Detection


Profanity Detection


Prompt Injection Detection




--------------------------------------------------

Storage


Azure Blob Storage


or


Local File Storage




--------------------------------------------------

Deployment


Frontend


Azure Static Web Apps




Backend


Azure App Service




Database


Azure SQL Database




--------------------------------------------------


Android Packaging


Capacitor


Android Studio




--------------------------------------------------


Future


Angular 20


Signal Based Components


PWA


iOS Support
