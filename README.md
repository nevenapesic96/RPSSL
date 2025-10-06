Rock, Paper, Scissors, Lizard, Spock – .NET 8 API Game

This is a RESTful API game built with .NET 8, implementing the extended version of Rock, Paper, Scissors — including Lizard and Spock. The app is containerized using Docker, and uses PostgreSQL as its database, orchestrated via docker-compose.

---

## Tech Stack

- [.NET 8](https://dotnet.microsoft.com/)
- Docker & docker-compose
- PostgreSQL
- REST API

---

## Getting Started

### Prerequisites

Make sure you have installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/)
- [Git](https://git-scm.com/)

---

### Running the app with Docker Compose

1. **Clone the repository**

```bash
git clone git@github.com:your-username/your-repo-name.git
cd your-repo-name
```

2. **Run the app**

```bash
docker-compose up --build
```

This will build the .NET 8 API container and start both the API and a PostgreSQL container.

---
### API Endpoints


| Method | Endpoint     | Description                                          |
|--------|--------------|------------------------------------------------------|
| GET    | `/choices`   | Returns the list of all possible game choices        |
| GET    | `/random`    | Returns a random choice                              |
| POST   | `/play`      | Starts a game round                                  |
| DELETE | `/play`      | Resets scoreboard for selected user, or entire table |
| GET    | `/results`   | Returns latest results                               |


---

### Database connection settings

```env
POSTGRES_DB: RPSSL
POSTGRES_USER: postgres
POSTGRES_PASSWORD: postgres
```

---

### Running locally without Docker

#### Steps:

1. **Start PostgreSQL locally**

2. **Create the database manually**:

```sql
CREATE DATABASE RPSSL;
```
2. **Run the init script (from project root)**:

```bash
psql -U postgres -d RPSSL -f ./db/init/init.sql
```

>If your username/password is different, update accordingly.
### Running Tests

---

```bash
dotnet test
```