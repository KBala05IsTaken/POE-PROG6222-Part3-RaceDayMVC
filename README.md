# RaceDay MVC Application

## Description

RaceDay MVC is an ASP.NET Core MVC application
that communicates with the RaceDay Part 2 API.

The application provides separate interfaces for
Organisers and Participants.

All database operations and image uploads are
handled through the Part 2 API.

Azure Blob Storage is accessed by the API only.
The MVC application does not connect directly
to Azure Blob Storage.

## Requirements

- Docker Desktop
- RaceDay Part 2 API
- Accessible API HTTP endpoint

## Build Docker Image

Open a terminal in the folder containing the Dockerfile.

Run:

```bash
docker build -t raceday-mvc .
```

## Run the Application

Start the RaceDay API first.

Then run the MVC container:

```bash
docker run -d --name raceday-mvc -p 8080:8080 -e ApiSettings__BaseUrl=http://host.docker.internal:5000/ raceday-mvc
```

Replace port 5000 with the actual HTTP port
of your RaceDay API.

## Access the Application

Open:

http://localhost:8080

## View Container Logs

```bash
docker logs raceday-mvc
```

## Stop the Application

```bash
docker stop raceday-mvc
```

## Remove the Container

```bash
docker rm -f raceday-mvc
```

## Azure Blob Storage

Event banner images are uploaded through the
RaceDay Part 2 API.

The API validates uploaded images and stores
them in Azure Blob Storage.

The MVC application does not access Azure
Blob Storage directly.
