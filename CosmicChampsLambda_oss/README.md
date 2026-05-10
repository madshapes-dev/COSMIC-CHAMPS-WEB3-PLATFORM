# Cosmic Champs Lambda Functions

AWS Lambda functions powering portion of Cosmic Champs Web3 game backend on Algorand. The benefit of this approach is that its very modular and can be easily reused and/or ported.
---

## Lambdas

### API_CreateAssignWalletProcessor

Creates a new custodial Algorand wallet for a player, or returns the existing one if already assigned. Called when a new player is registered to give them a wallet they can use in-game without needing to bring their own.

Can be extended to perform on-chain actions on behalf of the player (e.g. opt-ins, transfers).

**Auth:** the request payload must include a `payload.auth` field matching either the `AuthDev` or `AuthMaster` env var. On the backend side this value is stored in AWS Secrets Manager as `WalletRequest__Auth` and injected into the Game API at runtime — the client never sees it.

---

### DBDTOSQSProcessor

Triggered by DynamoDB stream inserts on the `MatchReport` table. Evaluates each match result and, if a prize-paying bot was defeated, pushes the payout event to the `PrizeBotPayoutsQueue` SQS queue. Simultaneously pushes all match results to the `LoggerQueue` for blockchain logging.

Key benefits of the queue approach:
- Deduplication of identical match results
- Burst protection — queue processes at its own pace
- Automatic retries on failure

---

### LoggerProcessor

Consumes from the `LoggerQueue`. Writes match results (winner, loser, ratings, wallets) onto the Algorand blockchain as a transaction note. Acts as an immutable, on-chain audit log of all match outcomes.

---

### PrizeBotPayoutsProcessor

Consumes from the `PrizeBotPayoutsQueue`. Executes the actual on-chain token payouts to winners who defeated a prize bot. 

Features:
- Per-bot configurable payout token (ALGO or any ASA) and base amount
- Reward multipliers based on winner's Algorand Wallet NFT holding (holder tier NFT) and MMR rating
- Planet owners fee — a configurable percentage of each payout is sent to the planet wallet
- Handles edge cases: insufficient balance alerts, recipient not opted-in to the ASA

**Required env vars:** `SenderMnemonic` (Algorand account that signs and sends transactions).

---

## Setup & Deployment

Each Lambda is a self-contained Node.js function. Deploy them individually via the AWS Console or CLI.

**For each function:**

```bash
cd <FunctionFolder>
npm install
zip -r function.zip .
aws lambda create-function \
  --function-name <FunctionName> \
  --runtime nodejs18.x \
  --handler index.handler \
  --zip-file fileb://function.zip \
  --role arn:aws:iam::<account-id>:role/<lambda-execution-role>
```

To update an existing function:
```bash
aws lambda update-function-code \
  --function-name <FunctionName> \
  --zip-file fileb://function.zip
```

**Set environment variables** in the Lambda console or via CLI:
```bash
aws lambda update-function-configuration \
  --function-name <FunctionName> \
  --environment "Variables={AuthDev=<value>,AuthMaster=<value>}"
```

**Triggers to configure in AWS Console:**
- `API_CreateAssignWalletProcessor` — Function URL or API Gateway (HTTP)
- `DBDTOSQSProcessor` — DynamoDB Stream on the `MatchReport` table
- `LoggerProcessor` — SQS trigger on `LoggerQueue`
- `PrizeBotPayoutsProcessor` — SQS trigger on `PrizeBotPayoutsQueue`

---

## Dependencies

Uses the older single-package `algosdk`. It does the job, keeps the footprint minimal, and could be swapped for the latest SDK with little effort.

---

## Environment Variables

| Variable | Used In | Description |
|----------|---------|-------------|
| `AuthDev` | API_CreateAssignWalletProcessor | Dev-level auth key for API calls |
| `AuthMaster` | API_CreateAssignWalletProcessor | Master auth key for API calls |
| `SenderMnemonic` | PrizeBotPayoutsProcessor, LoggerProcessor | Mnemonic for the Algorand sender account |
