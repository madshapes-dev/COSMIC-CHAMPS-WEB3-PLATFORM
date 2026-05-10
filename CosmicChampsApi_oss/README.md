# Cosmic Champs API

Standalone REST API service for **Algorand** NFT inventory tracking, wallet eligibility checks, and public match history. Intentionally separate from the main game backend — keeping NFT indexing and match logging out of the core game loop so they can never affect game logic or performance.

**Stack:** Node.js · Express · MongoDB (Mongoose)

---

## What it does

### NFT Inventory Tracking
Querying the **Algorand** indexer per-user on every request is expensive and slow. Instead, background cron jobs periodically pull the full NFT holder state from the [Algonode](https://algonode.io) public indexer and cache it in MongoDB. Game requests are then served from cache — fast, predictable, and with no indexer latency in the hot path.

Cron schedule (configurable):
- Uncommons — every 57 seconds
- Commons / Red Chroma / Space Rocks — every 3 minutes
- Rares — every 7 minutes
- Epics — every 4 minutes
- Legendaries — every 13 minutes

### Wallet Eligibility (Killswitch)
`/check/:walletId` validates whether a wallet holds the required NFT before a match is allowed to start. Includes:
- **Bypass list** — hardcoded wallet addresses that always return `valid: true` (used for iOS App Store review accounts, dev accounts, prize bots, and other special cases)
- **Blacklist** — wallets that always return `valid: false`
- **Cache-first lookup** — checks MongoDB before hitting the indexer; falls back to a live indexer query only if the wallet isn't cached yet

### Public Match History
`/matchstart` and `/matchend` record match events into a separate MongoDB database. `/getMatchdata` exposes this as a public leaderboard endpoint. This database is completely independent of the game backend's DynamoDB — match logging here has zero impact on the actual game session flow.

---

## Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/check/:walletId` | Validate wallet NFT eligibility (bypass/blacklist/cache/live) |
| GET | `/updatewallets` | Manually trigger wallet whitelist sync from **Algorand** indexer |
| GET | `/matchstart/:matchId/:pa/:pb` | Record a match start |
| GET | `/matchend/:matchId/:winner` | Record a match result |
| GET | `/getMatchdata/:from` | Public leaderboard data from a given date (`all` for all-time) |
| GET | `/getinventory/:walletId` | Return cached NFT inventory for a wallet |
| GET | `/updatespecials_all` | Manually trigger full NFT inventory sync across all rarities |

---

## Setup

```bash
npm install
cp .env.example .env
# edit .env with your values
npm start
```

| Variable | Description |
|----------|-------------|
| `MONGO_URI` | MongoDB connection string (default: `mongodb://localhost/ccapidb`) |
| `PORT` | Express port (default: `3000`) |

The **Algorand** indexer endpoint (`mainnet-idx.algonode.cloud`) is public and requires no API key.

Runs on a Linux server managed by **pm2**, with an automatic restart every 24 hours to keep the process healthy and cron jobs fresh.

---

## Special Cases & Inventory Manipulation

The bypass wallet list in `ccapiController.js` allows specific wallets to skip NFT validation entirely — used for:
- iOS / App Store review accounts
- Dev and QA accounts
- Prize bot wallets
- Any other wallet that needs guaranteed eligibility regardless of on-chain state

To assign custom NFT inventories to specific wallets (e.g. for review builds or prize bots), update the inventory controller directly — the cached MongoDB state can be seeded or overridden without touching the **Algorand** indexer.
