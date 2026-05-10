# Cosmic Champs — Web3 Platform

Real-time 1v1 card battle game on **Algorand** and Immutable. Players collect **Algorand** NFT units, build decks, and fight live matches hosted on dedicated AWS GameLift servers. NFT ownership and token economy powered by **Algorand**. Five open-source repositories, one platform.

---

## Repositories

### [CosmicChampsGame_oss](./CosmicChampsGame_oss) — Unity Game Client & Server
The full game — client, headless server, and simulator — in a single Unity project.

- **Engine:** Unity 6000.0.29f1 · URP · Mirror v70 (KCP + WebSocket)
- **Platforms:** iOS · Android · WebGL · Linux Dedicated Server
- **Infra:** AWS GameLift fleet (Linux x86_64) · Unity Addressables CDN
- **Architecture:** Server-authoritative battle simulation; client renders and sends inputs. Three deploy modes — Client, Server, Simulator — from one codebase.

→ [Full README](./CosmicChampsGame_oss/README.md) — scene architecture, networking setup, build instructions, App Profiles, multiplayer testing

### [CosmicChampsBackend_oss](./CosmicChampsBackend_oss) — Cloud Backend
Two independently deployable gRPC microservices in C# .NET 6 on AWS ECS Fargate.

- **Auth service** (10 RPCs): email/password, guest (device ID), Immutable Passport sign-in, token refresh
- **Game API** (19 RPCs): player data, matchmaking, ELO rating, shard rewards, missions, tournaments, bot system
- **Infra:** DynamoDB · GameLift FlexMatch · Cognito · AppConfig (live game data, 60s TTL) · Secrets Manager
- **Auth schemes:** Cognito JWT (players) · HMAC-SHA256 (game server → API)

→ [Full README](./CosmicChampsBackend_oss/README.md) — local dev setup, DynamoDB table creation, all config keys, deployment via Copilot, proto contracts

### [CosmicChampsWebpage_oss](./CosmicChampsWebpage_oss) — Web Portal
SvelteKit web app at [play.cosmicchamps.com](https://play.cosmicchamps.com) — wallet linking, leaderboards, allowlist, and tournament registration.

- **Stack:** SvelteKit · Vercel · AWS Cognito + DynamoDB
- **Wallets:** Pera · Defly · Lute (**Algorand** WalletConnect)
- Links a player's **Algorand** wallet to their game account stored in DynamoDB

→ [Full README](./CosmicChampsWebpage_oss/README.md) — quickstart, auth flow, environment variables, wallet integration

### [CosmicChampsApi_oss](./CosmicChampsApi_oss) — NFT Inventory & Match History API
Standalone Node.js/Express service for **Algorand** NFT tracking and public match history — fully separate from the game backend to keep indexer queries out of the core game loop.

- **NFT inventory cache** — cron jobs periodically sync **Algorand** NFT holder state (by rarity) from the Algonode public indexer into MongoDB; all inventory lookups are served from cache
- **Wallet eligibility** — `/check/:walletId` validates NFT ownership before a match starts; supports bypass lists (App Store review, dev, prize bot accounts) and blacklists
- **Public match history** — independent database and endpoints for leaderboard/match data, zero impact on game session flow
- **Inventory overrides** — cached NFT state can be seeded or overridden for special wallets without touching the indexer

→ [Full README](./CosmicChampsApi_oss/README.md) — endpoints, cron schedule, bypass/blacklist system, inventory manipulation, setup

### [CosmicChampsLambda_oss](./CosmicChampsLambda_oss) — AWS Lambda Functions
Serverless **Algorand** blockchain layer — custodial wallet creation, on-chain match logging, and prize token payouts.

- **API_CreateAssignWalletProcessor** — creates a custodial **Algorand** wallet for new players on registration; called by the Game API on first player creation
- **DBDTOSQSProcessor** — triggered by DynamoDB stream on `MatchReport`; routes prize bot match results to the payout queue and all results to the blockchain logger
- **LoggerProcessor** — writes every match outcome onto the **Algorand** blockchain as a transaction note — immutable on-chain audit log
- **PrizeBotPayoutsProcessor** — executes on-chain token payouts (ALGO or any ASA) to prize bot winners; applies multipliers based on NFT tier holdings and MMR rating

→ [Full README](./CosmicChampsLambda_oss/README.md) — lambda descriptions, env vars, payout logic, queue architecture

---

## appconfig.json — The Central Configuration

[`CosmicChampsBackend_oss/Api/appconfig.json`](./CosmicChampsBackend_oss/Api/appconfig.json) is the single file that ties the entire platform together. It is served via AWS AppConfig with a 60-second TTL and hot-reloaded across all backend services with no downtime.

It defines everything that makes the game what it is:

- **Cards & Units** — full roster, stats, energy costs, rarity, upgrade shard IDs
- **Bots** — all bot types (tutor, default, prize), difficulty levels, matchmaking weights, deck loadouts, wallet and player ID assignments
- **Economy** — card progression costs, player XP, universal shard rates, prize bot payout rates
- **Missions** — milestone definitions and rewards
- **Matchmaking** — ELO thresholds, prize bot trigger conditions, deck presets
- **Client gating** — minimum build version, maintenance mode toggle, soft-update thresholds
- **Social & partner URLs** — WalletConnect bridge, game partner links

If you are forking or rebranding this platform, `appconfig.json` is your primary customization point — change the bots, rebalance the economy, add cards, configure missions, and gate client versions all from this one file.

---

## Architecture at a Glance

```
Player Device
  └── Unity Client (iOS / Android / WebGL)
        ├── gRPC-Web → Auth Service  (ECS)  → Cognito
        └── gRPC-Web → Game API     (ECS)  → DynamoDB / AppConfig / GameLift
                                               ↑
                                    Game Server (GameLift EC2, HMAC auth)
                                               ↓
                               Lambda: wallet creation (on player registration)
                               Lambda: match logging → Algorand blockchain
                               Lambda: prize payouts → Algorand on-chain transfer

Algorand NFT Layer (CosmicChampsApi_oss — Linux/pm2)
  └── Cron jobs → Algonode indexer → MongoDB NFT cache
  └── /check/:walletId   ← game backend killswitch calls
  └── /getMatchdata      ← public leaderboard (separate DB)

Web Browser
  └── SvelteKit (Vercel) → Cognito + DynamoDB
```

## License

See [LICENSE](./LICENSE).
