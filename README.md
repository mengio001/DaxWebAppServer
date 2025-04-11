# DAX WebAppServer

**DAX WebAppServer** is the main entry point of the **DAX (Distributed Architecture eXamples)** ecosystem — a full-stack web application server that integrates an Angular SPA, a .NET Backend API, and a BFF (Backend for Frontend) secured by YARP reverse proxy.

This application interacts with other core services in the DAX architecture:

- **DAX IdentityServer** — Provides secure authentication and token-based authorization
- **DAX User Management** — Manages registered users, roles, and access control (e.g. blocking users)

---

## Project Structure

| Component                  | Description                                |
|---------------------------|--------------------------------------------|
| Angular SPA               | Frontend user interface (served via proxy) |
| Backend API               | RESTful API for quiz and user features     |
| BFF (YARP Reverse Proxy)  | Bridges frontend to backend + auth flows   |

---

## External DAX Services Used

| Service               | Role in Platform                                      | URL                         |
|-----------------------|--------------------------------------------------------|------------------------------|
| DAX IdentityServer    | Authenticates users and issues access tokens          | https://localhost:44300/     |
| DAX User Management   | Handles role assignments, user blocking, user metadata| https://localhost:44324/     |

---

## Local Development URLs

| Component       | Port/Cluster         | URL                          |
|------------------|----------------------|-------------------------------|
| Angular SPA      | `angularCluster`     | https://localhost:44480/      |
| Backend API      | `apiCluster`         | https://localhost:44355/      |
| IdentityServer   | `identityCluster`    | https://localhost:44300/      |
| WebAppServer BFF | — (Gateway entry)    | https://localhost:44365/      |

> All traffic flows securely through the **DAX WebAppServer** using YARP reverse proxy.

---

## Security & Proxying with YARP

DAX WebAppServer uses YARP (Yet Another Reverse Proxy) to:

- Centralize routing and secure access to backend and external services
- Enforce cookie- and token-based security
- Prevent XSS, CSRF, and cross-origin misconfigurations

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/)
- [Node.js & npm](https://nodejs.org/)
- Angular CLI: `npm install -g @angular/cli`

---

### Start Services

#### 1. Backend API

```bash
cd QuizTowerPlatform.Api
dotnet restore
dotnet run --urls=https://localhost:44355
```

#### 2. BFF (WebAppServer)

```bash
cd QuizTowerPlatform.Bff
dotnet restore
dotnet run --urls=https://localhost:7184
```

#### 3. Angular SPA

```bash
cd QuizTowerPlatform.ClientApp
npm install
ng serve --port 44480
```

#### 4. IdentityServer (from separate repo)

```bash
cd ../DaxIdentityServer
dotnet run --urls=https://localhost:44300
```

---

## 📫 Contact

Ozkan Mengi – [LinkedIn](https://linkedin.com/in/mengio1990) – o.mengi@timelessmedia.nl – [timelessmedia.nl](https://timelessmedia.nl)

---

## 📄 License

This project is part of the DAX sandbox and intended for educational/demo use.

