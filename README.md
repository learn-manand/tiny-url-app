# Tiny URL App

A full-stack Tiny URL application with backend, frontend, and a cleanup Azure Function.

## Project Structure

tiny-url-app/
│
├── backend/
│ ├── TinyUrl.Api/ # .NET 6/8 backend API project
│ ├── TinyUrl.Api.Tests/ # Unit tests for backend
│ └── Dockerfile
│
├── frontend/ # Angular frontend
│ ├── src/
│ └── Dockerfile
│
├── infra/ # Infrastructure projects
│ └── azure-function/
│ └── CleanupJob/ # Azure Function to clean URLs periodically
│
├── .github/workflows/ # GitHub Actions CI/CD
├── docker-compose.yml
└── README.md


## Environment Variables

| Variable                         | Description                                           |
|----------------------------------|-------------------------------------------------------|
| ConnectionStrings__DefaultConnection | SQL/PostgreSQL connection string for backend       |
| ASPNETCORE_ENVIRONMENT            | Environment (Development/Production)               |
| SecretToken                       | Secret token for backend authentication            |
| BaseApiUrl                        | Backend API URL (used by frontend and cleanup function) |
| AzureStorage__BlobContainerName   | Azure Storage Blob container name                  |
| AzureStorage__ConnectionString    | Azure Storage connection string                     |

## Running Locally

### Backend
```bash
cd backend/TinyUrl.Api
dotnet run

Frontend
cd frontend
npm install
ng serve

Cleanup Function
cd infra/azure-function/CleanupJob
func start
Note: Ensure Azurite (local Azure Storage emulator) is running if testing the function locally.

Docker & Containers

All projects are containerized. You can build and run all containers using docker-compose:
docker-compose up --build

Services:

tinyurl-api: backend API

frontend: Angular frontend served via Nginx

cleanup-job: cleanup Azure Function (optional for local testing)

Deployment

Backend and frontend are deployed to Azure App Service (Container-based).

Cleanup function is deployed to Azure Function App.

CI/CD is configured via GitHub Actions for automatic builds and deployments.

Docker images are pushed to Docker Hub.

CI/CD

Triggered on main branch push.

Builds and pushes images for backend, frontend, and cleanup function.

Deploys containers to their respective Azure services.

Secrets required in GitHub:
DOCKERHUB_USERNAME
DOCKER_HUB_PASSWORD

Backend, frontend, and cleanup function publish profiles are not required as we deploy container images.