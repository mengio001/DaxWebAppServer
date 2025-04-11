# 🌐 DAX WebAppClient (Angular SPA)

This project is the **frontend** of the DAX WebAppServer — part of the **DAX (Distributed Architecture eXamples)** ecosystem. It is a modular single-page application built with Angular and served via the YARP-powered BFF layer.

The app communicates securely with:
- 🔐 **DAX IdentityServer** for authentication
- 🌍 **DAX Backend API** for protected resources

---

## ⚙️ Development Server

Start the development server with:

```bash
ng serve --port 44480
```

Then open [`https://localhost:44480`](https://localhost:44480) in your browser.  
The app will automatically reload when you edit source files.

---

## 🧱 Code Scaffolding

Generate new components, modules, or services using Angular CLI:

```bash
ng generate component my-component
```

You can also use:
```bash
ng generate directive|pipe|service|class|guard|interface|enum|module
```

---

## 🏗️ Build

```bash
ng build
```

Compiled output is stored in the `dist/` directory.

---

## ✅ Running Unit Tests

Run unit tests with:

```bash
ng test
```

Executed via [Karma](https://karma-runner.github.io).

---

## 🌐 E2E Testing

Add a testing platform (e.g., Cypress or Playwright), then run:

```bash
ng e2e
```

---

## 📚 Further Help

Use `ng help` or visit the [Angular CLI docs](https://angular.dev/tools/cli) for full command reference.

---

## 📦 Project Context

This app is part of the full-stack DAX WebAppServer, which also includes:
- A reverse proxy (BFF)
- A backend API
- Integration with DAX IdentityServer and DAX User Management

