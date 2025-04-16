# TodoApp

A simple Todo application with task progress tracking functionality.

## Features

- Create, read, update, and delete Todo items
- Track progress on tasks with percentage-based progression
- Items with more than 50% progress cannot be updated or deleted
- Progress visualization with progress bars
- Categories for organizing tasks

## Tech Stack

- **Backend**: .NET 8 Web API, C#, Clean Architecture
- **Frontend**: Vue.js 3, Axios, Vue Router

## Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Node.js](https://nodejs.org/) (v18 or later recommended)
- npm (comes with Node.js)

## How to Run

### Backend (API)

1. Navigate to the backend project directory:

```bash
cd TodoApp.API
```

2. Run the API:

```bash
dotnet run
```

The API will be available at https://localhost:44359 with Swagger UI at https://localhost:44359/swagger.

### Frontend (Vue.js)

1. Navigate to the frontend project directory:

```bash
cd TodoApp.Web
```

2. Install dependencies:

```bash
npm install
```

3. Run the development server:

```bash
npm run dev
```

The web application will be accessible at http://localhost:8080 and will automatically connect to the backend API.

## Project Structure

- **TodoApp.API**: Web API controllers and startup configuration
- **TodoApp.Application**: Application services and DTOs
- **TodoApp.Domain**: Domain entities, interfaces, and validators
- **TodoApp.Infrastructure**: Repository implementations and data access
- **TodoApp.Tests**: Unit tests for the application
- **TodoApp.Web**: Vue.js frontend application

## Usage

1. Create new Todo items with title, description, and category
2. Register progress updates by percentage
3. Track overall item completion
4. View individual item details and progress history
5. Items are considered complete when they reach 100% progress
