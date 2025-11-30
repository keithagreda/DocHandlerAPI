# DocHandlerAPI 📄

**DocHandlerAPI** is a simple, robust document-handler web service for storing, retrieving and managing documents. It aims to make saving and handling documents easy, flexible, and secure.

## Why DocHandlerAPI

Many applications need a backend service that handles document upload, storage, retrieval, and metadata management. DocHandlerAPI abstracts these concerns into a reusable service, giving you:

- Easy endpoints to upload documents  
- Consistent handling and storage of files  
- Flexibility for integration with front-end clients or other services  

## Features

- RESTful API endpoints for document upload, download, list, delete  
- Document metadata support (file name, upload date, etc.)  
- Unit tests (see `DocumentHandlerAPI.Tests`)  
- Clean and simple architecture  

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (match the version used by the project)
- (Optional) Docker for containerized deployment
- A storage location (local file system, cloud storage, or database — depending on your implementation)

## Getting Started — Running Locally

1. Clone the repository  
   ```bash
   git clone https://github.com/keithagreda/DocHandlerAPI.git
   cd DocHandlerAPI
2. Restore dependencies and build
  dotnet restore
  dotnet build
3. Run the API
   dotnet run --project DocumentHandlerAPI
4. Run tests
   dotnet test

The API will run on your configured port (e.g., https://localhost:5001). You can interact with it using Postman, curl, or any HTTP client.
API Endpoints (Example)
Method	Endpoint	Description
POST	/api/documents/upload	Upload a new document
GET	/api/documents/{id}	Retrieve/download a document by ID
GET	/api/documents	List all uploaded documents (metadata)
DELETE	/api/documents/{id}	Delete a document by ID

##Project Structure
DocumentHandlerAPI/         — main API project  
DocumentHandlerAPI.Tests/   — unit tests  
.gitignore  
README.md

##Usage / Integration
-Upload → send multipart/form-data request with the file.

-Download → GET request using document ID.

-List → GET /api/documents to display all uploaded files.

-Easily integrate with Angular, React, mobile apps, or other backend services.
