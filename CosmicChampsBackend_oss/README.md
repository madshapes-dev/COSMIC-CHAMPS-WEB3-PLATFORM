# Cosmic Champs — Backend

Two independently deployable gRPC microservices in **C# .NET 6** on **AWS ECS Fargate**. Handles auth, player data, matchmaking, ELO rating, rewards, and game session lifecycle.

---

## Services

### Auth (10 RPCs)
Wraps AWS Cognito. Handles all account flows:
- Email/password sign-up, confirmation, sign-in
- Guest accounts — device ID only, Cognito account auto-created behind the scenes
- Immutable Passport — Web3 sign-in via Immutable X API; on first login, creates a Cognito account and stores the player's Immutable wallet address (`passport_address`) in the `ImmutableCredentials` DynamoDB table
- Token refresh, sign-out, password reset
- `VersionCheck` — returns the active WalletConnect bridge URL to the client on startup

### Game API (19 RPCs)
All game logic. JWT-authenticated for clients, HMAC-authenticated for the game server:
- **Player creation** — on the first `GetPlayerData` call, if no player record exists, the API calls the `API_CreateAssignWalletProcessor` Lambda (`getWallet` operation) with `payload.auth` set to `WalletRequest__Auth` (from Secrets Manager) to create or retrieve the player's custodial **Algorand** wallet, then writes the full player record to DynamoDB. This is the only point where wallet ID is fetched and persisted for standard/guest accounts.
- **Algorand** NFT inventory validation — before each match the backend calls `CosmicChampsApi_oss` (`/check/:walletId` for killswitch eligibility, `/getinventory/:walletId` for NFT loadout). Inventory is served from the API's MongoDB cache — the backend never queries the **Algorand** indexer directly
- Deck management, card levelling
- Matchmaking via AWS GameLift FlexMatch (four queues: standard PVP, tutor bot, prize bot, tournament)
- Game session start/stop — called by the game server, not the client
- ELO rating updates, shard rewards, mission milestones after each match
- Promo codes, news, crash reporting

---

## Auth Schemes

| Scheme | Used by | How |
|--------|---------|-----|
| `AWSJwt` | Unity clients (production) | Cognito JWKS |
| `CustomJwt` | Dev/test clients | HMAC-SHA256 symmetric key |
| `HMACBearer` | Game server → API | `HMACAuthorizationHttpClientHandler` |

The game server signs every backend call with `AppProfile.APIServiceSecretKey` (Unity side) — same value as `HMAC__Key` in the backend config.

---

## Infrastructure

| Service | Role |
|---------|------|
| **ECS + ECR** | Container hosting via AWS Copilot (`linux/arm64`, 256 CPU / 512 MB each) |
| **DynamoDB** | Primary database — player, session, match, credential data (9 tables) |
| **GameLift + FlexMatch** | Dedicated game server hosting and skill-based matchmaking |
| **Cognito** | Identity provider for email/password and guest accounts |
| **AppConfig** | Live game config — cards, bots, missions, balance (~300 KB, 60s TTL) |
| **Secrets Manager** | All runtime secrets, injected by Copilot at container start |
| **CloudWatch** | Structured logs from both services |

---

## Project Structure

```
CosmicChampsBackend.sln
├── Api/                    Game API gRPC service (19 RPCs) + Dockerfile
├── Auth/                   Auth gRPC service (10 RPCs) + Dockerfile
├── Common/                 Shared DynamoDB models, repositories, ImmutableService
├── ClientFilterInterceptor/ gRPC interceptor — maintenance mode + client version gating
├── ApiClientDataSource/    Compiles game.proto as gRPC client → ApiClientDataSource.dll (shipped to Unity)
├── AuthClientDataSource/   Compiles auth.proto as gRPC client → AuthClientDataSource.dll (shipped to Unity)
├── DynamoDBLocal/          Local DynamoDB JAR for development
├── GameLiftLocal/          Local GameLift JAR for development
└── copilot/                AWS Copilot manifests (per service, per environment)
```

> **`ApiClientDataSource` and `AuthClientDataSource`** are not internal tooling — they produce the DLLs embedded in the Unity game. Rebuild and ship them to the Unity team whenever a `.proto` contract changes.

---

## Local Development

### Prerequisites
.NET 6 SDK · Docker Desktop · Java 11+ (for local DynamoDB and GameLift JARs) · AWS CLI

### Run locally

```bash
# Start DynamoDB Local
cd DynamoDBLocal && java -jar DynamoDBLocal.jar -sharedDb

# Start GameLift Local
cd GameLiftLocal && java -jar GameLiftLocal.jar -p 9080

# Set env vars
export ASPNETCORE_ENVIRONMENT=Development
export LOCAL_DYNAMODB=http://localhost:8000
export LOCAL_GAMELIFT=http://localhost:9080
export LOCAL_APPCONFIG=true   # reads local Api/appconfig.json

# Run services (different terminals, different ports — both default to :80)
cd Api  && dotnet run --urls "http://localhost:5001"
cd Auth && dotnet run --urls "http://localhost:5002"
```

Set secrets via .NET User Secrets (never commit them):
```bash
cd Api
dotnet user-secrets set "HMAC__Key" "<value>"
dotnet user-secrets set "CustomJWT__Secret" "<value>"
# ... see full list in copilot/api/manifest.yml
```

`appconfig.json` is included in `Api/` — no additional steps needed.

Health checks: `GET http://localhost:5001/Api/Healthcheck` · `GET http://localhost:5002/Auth/Healthcheck`

---

## Deployment

```bash
copilot svc deploy --name api  --env dev
copilot svc deploy --name auth --env dev
```

Environments: `dev` · `testa` · `testb` · `production` — each with its own DynamoDB table prefix and FlexMatch config.

All secrets must be provisioned in AWS Secrets Manager before first deploy. Paths follow the pattern `services/{service}/{env}/env:{KEY}::` as defined in the Copilot manifests.
