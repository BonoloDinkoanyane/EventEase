# EventEase – Cloud-Based Event Booking System

EventEase is a full-stack event management and venue booking platform built using ASP.NET Core MVC, Azure SQL Database, and Azure Blob Storage.
The system allows administrators to manage venues, events, and bookings while supporting advanced search, filtering, and image upload through Azure services.

## Features

### Venue Management
  - Add, edit, view, and delete venues
  - Upload venue images using Azure Blob Storage
  - Display venues in responsive Bootstrap cards
  - Venue availability tracking
  - Validation for preventing deletion of venues with linked bookings

### Event Management
  - Create events with a name, description, date, and an event type
  - Select an event type from a pre-populated dropdown
  - Display events in a structured Bootstrap layout
  - CRUD operations with validation

### Booking Management
  - Book venues for specific events and dates
  - Display bookings with joined Event + Venue information
  - Search bookings by:
    - Booking ID
    - Event Name
  - View bookings in a structured Bootstrap card gallery

### Advanced Booking Filtering
  - Filter bookings by:
    - Event Type
    - Date range
    - Venue availability
  - Dynamic result display using Bootstrap cards

### Cloud Integration
  - Azure SQL Database for persistent data storage
  - Azure Blob Storage for storing and displaying images
  - Azure Web App Service for deployment

## Technology Stack

### Backend
	- ASP.NET Core MVC
  - Entity Framework Core (Code-First + Database-First elements)
	- Azure SQL Database
	-	Azure Blob Storage

### Frontend
  - Razor Pages
  - Bootstrap 5
  - Responsive card-based UI

### DevOps & Deployment
  - Azure App Service (Web App Hosting)
  - Azure SQL Database holds application data
  - Azure Storage Account hold the uploaded images

## Architecture Overview

### Controllers
	-	VenueController: Handles venue CRUD operations and image uploads
	-	EventController: Manages events and event types
	-	BookingController: Manages bookings, search, and advanced filtering

### Models
	-	Venue
	-	Event
	-	Booking
	-	EventType
	-	BookingViewModel 

### Views
	- Organised into:
		- /Views/Venue
		- /Views/Event
    	- /Views/Booking

### Azure Components
  - BlobContainerClient used for:
    - Uploading images
    - Generating blob URLs
  - Azure SQL Server storing relational data

## Image Upload System
The application uses Azure Blob Storage to handle image uploads.
Process flow:
	1.	User uploads an image in the Venue form
	2.	The image is streamed to Azure Blob Storage
	3.	Azure returns a secure URL
	4.	URL is saved in the SQL database
	5.	Images are displayed dynamically in the UI






