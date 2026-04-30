#  MijnGameKast API

Een ASP.NET Core Web API voor het beheren van een persoonlijke gamecollectie.  
Gebruikers kunnen games opslaan, collecties maken en hun eigen catalogus beheren.

---

##  Functionaliteiten

-  Gebruikersregistratie en login
-  Authenticatie via Bearer tokens
-  Centrale gamecatalogus
-  Persoonlijke collecties
-  Games toevoegen/verwijderen uit collecties
- 🗄 PostgreSQL database via Entity Framework Core

---

## Architectuur

Het project is opgebouwd met een duidelijke scheiding van verantwoordelijkheden:

- **Controllers** → API endpoints
- **Services** → business logic
- **Repositories** → database interactie
- **Data (DbContext + Models)** → database structuur

---

## ️ Setup & installatie

### 1. Repository clonen

```bash
git clone <repo-url>
cd MijnGameKastApi
```

---

### 2. `.env` bestand aanmaken

Maak een `.env` bestand in de root van het API project:

```env
CONNECTION_STRING=Host=localhost;Port=5432;Database=mijngamekast;Username=postgres;Password=yourpassword
```

---

### 3. Database migraties uitvoeren

```bash
dotnet ef database update --project MijnGameKast.API.Data --startup-project MijnGameKast.API
```

---

### 4. Applicatie starten

```bash
dotnet run --project MijnGameKast.API
```

---

## Mitigaties
- ThreatID 15: bij het inloggen wordt niet verteld dat er al een account bestaat met dezelfde e-mailadres of gebruikersnaam.

---

##  API documentatie (Scalar)

Na het starten van de applicatie kun je alle endpoints bekijken via:

```
https://localhost:7199/scalar
```

Hier kun je:
- alle endpoints zien
- requests uitvoeren
- headers en body bekijken
- Bearer token invullen

---

##  Authenticatie

Voor beveiligde endpoints moet je een Bearer token meesturen:

```
Authorization: Bearer <token>
```

Token krijg je via:

```
POST /api/auth/login
```

---

##  Belangrijkste endpoints

### Auth

- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/logout
- GET /api/auth/me

---

### Catalog

- GET /api/catalog
- POST /api/catalog
- GET /api/catalog/{id}
- PUT /api/catalog/{id}
- DELETE /api/catalog/{id}

---

### Collections

- GET /api/collection
- POST /api/collection
- GET /api/collection/{id}
- PUT /api/collection/{id}
- DELETE /api/collection/{id}

---

### Collection Games

- GET /api/collection/{id}/games
- POST /api/collection/{id}/games/{gameId}
- DELETE /api/collection/{id}/games/{gameId}

---

##  Database seeding

Bij het opstarten wordt de database automatisch gevuld met testdata via de `DbSeeder`.

---

##  GitFlow workflow

Dit project gebruikt een eenvoudige GitFlow-structuur:

### Branches

- `main` → stabiele productieversie
- `develop` → actieve development branch

---

### Feature branches

Nieuwe features worden ontwikkeld vanuit `develop`:

```bash
git checkout develop
git checkout -b feature/naam-van-feature
```

Na afronding:
- Pull Request → develop

---

### Hotfix branches

Voor urgente fixes in productie:

```bash
git checkout main
git checkout -b hotfix/bug-fix
```

Na afronding:
- Merge naar main
- Merge terug naar develop

---

##  Toekomstige uitbreidingen

- JWT authenticatie i.p.v. simpele tokens
- Role-based authorization
- Public collections bekijken zonder login
- Pagination & filtering

---

## Licentie

Dit project is bedoeld voor educatieve doeleinden.

---

##  Gebruikte technologieën

- ASP.NET Core (.NET 10)
- Entity Framework Core
- PostgreSQL
- Scalar (OpenAPI UI)  