# Cosmic Champs Beta

## Overview

This app serves as the landing page for Cosmic Champs, a mobile blockchain game on **Algorand**. Its primary purpose is to allow players to connect their **Algorand** wallet (via Pera, Defly, or Lute) to their existing Cosmic Champs player account, linking the wallet address to their profile stored in DynamoDB.

Live page preview: [play.cosmicchamps.com](https://play.cosmicchamps.com)

---

## Tech Stack

- **SvelteKit** — frontend framework and server-side routing
- **Vercel** — hosting and deployment
- **AWS Cognito** — player authentication
- **AWS DynamoDB** — player account storage
- **Pera Connect** `1.5.0` — Pera Wallet integration
- **Defly Connect** `1.1.6` — Defly Wallet integration
- **Lute Connect** `1.7.0` — Lute Wallet integration

---

## Quickstart

Clone the repo and install dependencies:

```bash
git clone
npm install
```

Copy [`.env.example`](.env.example) to `.env` and fill in all required values (see [Variables](#variables) below):

```bash
cp .env.example .env
```

Start the dev server:

```bash
npm run dev
# opens at http://localhost:5173/
```


---

## Auth Flow

Authentication and wallet linking follow these steps on the backend:

1. **Player login** — the player submits their email and password via the frontend. The `/api` route calls `authenticateUser()` in [`src/lib/server/cognito.js`](src/lib/server/cognito.js), which issues a `USER_PASSWORD_AUTH` request to AWS Cognito. On success, Cognito returns an `AccessToken` and the player's user attributes.

2. **Player identity confirmed** — after a successful Cognito auth, `AdminGetUserCommand` is called to fetch the full user record, including the player's internal `Id`.

3. **Wallet linking** — once the player connects their **Algorand** wallet in the UI, the wallet address is written to DynamoDB via `updateItemInDynamoDB()` in [`src/lib/server/dynamodb.js`](src/lib/server/dynamodb.js). This sets the `LinkedWalletId` field on the player's record in the `Player` table, keyed by the player's `Id`.

4. **Service account operations** — calls to Google Sheets (allowlist checks, tiers, tournament data) are authenticated using an encrypted service account credential, decrypted at runtime via AES-128-CBC using `VITE_SERVICE_ENCRYPTION_KEY` and `VITE_SERVICE_ENCRYPTION_IV`.

> Note: Players are auto-assigned a `WalletId` on account creation (guest and email+password accounts). `LinkedWalletId` is the custom wallet the player explicitly connects through this app.

---

## Variables

All variables are listed in [`.env.example`](.env.example). Copy it to `.env` and populate the values. They are never exposed to the client — all are consumed server-side via SvelteKit's `$env/static/private`.

| Variable | Description |
|---|---|
| `VITE_AWS_ACCESS` | AWS IAM access key ID used to authenticate SDK calls to Cognito and DynamoDB |
| `VITE_AWS_SECRET` | AWS IAM secret access key paired with the above |
| `COGNITO_USER_POOL_ID` | AWS Cognito User Pool ID (format: `eu-central-1_XXXXXXXXX`) |
| `COGNITO_CLIENT_ID` | AWS Cognito App Client ID associated with the user pool |
| `VITE_SERVICE_ENCRYPTION_KEY` | 16-byte AES-128 key used to decrypt the Google service account credentials at runtime |
| `VITE_SERVICE_ENCRYPTION_IV` | 16-byte initialization vector paired with the encryption key above |

---

> **Note:** This repository includes additional features such as leaderboards, bug reporting, tier information, and tournament registration. These are not covered in this README.
