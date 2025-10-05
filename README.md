# Product Catalog Management System

A full-stack product catalog application built with ASP.NET Core Web API and Angular, demonstrating three-tier architecture, CLEAN architecture principles, and advanced C# features including fuzzy search with Levenshtein algorithm.

## Features

### Backend (C# / .NET 8)
- RESTful API with 7 endpoints
- Fuzzy search with Levenshtein distance algorithm (searches for "lptop" finds "laptop")
- Pattern matching validation using C# switch expressions
- Generic repository pattern with in-memory storage (List/Dictionary)
- Custom middleware for logging and error handling
- Dictionary-based caching layer
- Hierarchical category tree structure
- Record types for DTOs (C# 9+)
- Nullable reference types throughout

### Frontend (Angular 17+)
- Standalone components
- Reactive forms with validation
- RxJS for state management
- Three-tier architecture (Data Access, Business Logic, Presentation)
- Debounced search with real-time filtering
- Category filtering
- CRUD operations with confirmation dialogs

## Architecture

### Backend Structure
ProductCatalog/
├── ProductCatalog.API/              # Presentation Layer (Controllers, Middleware)
├── ProductCatalog.Application/      # Business Logic (Services, DTOs, Validators)
├── ProductCatalog.Domain/           # Core Entities and Interfaces
├── ProductCatalog.Infrastructure/   # Data Access (Repositories, Caching)
└── ProductCatalog.Tests/            # Unit Tests

## Prerequisites

### Backend
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 or VS Code with C# extension

### Frontend
- [Node.js](https://nodejs.org/) (v18 or newer)
- npm (comes with Node.js)
- Angular CLI: `npm install -g @angular/cli`

### Setup Instructions

### 1. Clone/Download the Repository

### 2. Backend Setup
Using Visual Studio

Open ProductCatalog.sln in Visual Studio 2022
Right-click on the solution → Restore NuGet Packages
Build the solution: Ctrl + Shift + B
Set ProductCatalog.API as startup project (right-click → Set as Startup Project)
Press F5 to run

# Navigate to frontend directory
Clone the front-end project: https://github.com/obinotwan/product-catalog-frontend.git

cd product-catalog-frontend

# Install dependencies
npm install

# Start development server
ng serve

The application will be available at http://localhost:4200
