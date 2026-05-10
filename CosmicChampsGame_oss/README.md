# Cosmic Champs

Real-time 1v1 card battle game built in Unity. Players collect **Algorand** NFT units, build decks, and fight live matches on dedicated AWS GameLift servers. NFT ownership and in-game token economy run on **Algorand**.

**Engine:** Unity 6000.0.29f1 | **Platforms:** iOS · Android · WebGL · Linux Dedicated Server

---

## How it works

The server runs the full battle simulation — the client renders it and sends inputs. Three modes compile from one codebase: **Client** (player app), **Server** (headless GameLift process), and **Simulator** (headless bot client for load testing against a live server).

Multiplayer uses **Mirror v70** — KCP/UDP for native builds, WebSocket for WebGL (TLS-terminated by Nginx on EC2). Backend communication is **gRPC-Web** via pre-compiled client stubs from the backend repo's `.proto` files.

---

## Setup

**1. Fix the GameLift SDK path before opening in Unity.**
`Packages/manifest.json` line 20 has a hardcoded developer machine path — update it to point to your local copy of `com.amazonaws.gameliftserver.sdk-5.2.0.tgz`.

**2. Open in Unity Hub → Add → select the project folder.** First import takes 5–15 minutes.

**3. Assign an AppProfile.**
`Assets/CosmicChamps/Settings/AppProfiles/` contains per-environment ScriptableObjects. Assign the right one on `ProjectContext` in the bootstrap scene. Key fields: `AuthServiceUrl`, `APIServiceUrl`, `APIServiceSecretKey`, `BundlesLoadUrl`, `ImmutableConfig`.

**4. Run via the menu.** Use **CosmicChamps → Run → Editor → Client/Server** — no need to open the bootstrap scene manually.

---

## Scenes

The project uses a multi-scene additive setup. Every scene has its own `SceneContext` and an `EntryPoint` as the root of that scene's flow.

**Client path:** `ClientBootstrap → HomeScreen → ClientBattle → Level / Level2`
HomeScreen holds all lobby UI (auth, deck builder, matchmaking). ClientBattle loads a Level scene additively once a match is found.

**Server path:** `ServerBootstrap → ServerMatchmaking → ServerBattle → Level / Level2`
ServerMatchmaking waits for players, then transitions to ServerBattle. After the match it returns to ServerMatchmaking for the next game.

When creating new levels, make sure to configure `Contract Names` and `Parent Contract Names` on `SceneContext` to wire up Zenject's scene decorator pattern correctly.

---

## Code

All source lives in `Assets/CosmicChamps/Scripts/`, split by system:

- **Data/** — plain C# model layer (Player, GameSession, Tokens, repositories). Not MonoBehaviours.
- **Networking/** — gRPC handlers, HMAC/Bearer HTTP handlers, Mirror transports and messages
- **HomeScreen/UI/** — UI built with a MVP pattern; MonoBehaviours are presenters, data objects are the model
- **Services/** — Immutable Passport, AdMob, auth, and other third-party integrations
- **Settings/** — `AppProfile` and `NetworkingConfig` ScriptableObjects

Zenject Signals are used heavily to decouple systems from each other.

---

## Game Content

Units, levels, and UI assets are delivered via **Unity Addressables** — 27 groups, CDN-hosted per build version. At startup the client checks for catalog updates, downloads any changed bundles, then loads HomeScreen.

Inspect existing Addressables groups in **Window → Asset Management → Addressables** for the full layout.

---

## Build & Deploy

Build helpers live under **CosmicChamps → Build** in the Unity menu.

Shell scripts for deployment are in `Scripts/`:

| Script | Purpose |
|--------|---------|
| `upload-server-build` | Deploy Linux server binary to GameLift |
| `upload-webgl-build` | Deploy WebGL build |
| `upload-bundles` | Push Addressable bundles to CDN |
| `create-dev-fleet` | Create a development GameLift fleet |
| `get-instance-access` | SSH into a GameLift instance |
| `run-stress-test` | Run simulator bots against a fleet |

---

## Testing multiplayer

Build via **CosmicChamps → Build**, run one instance from the build and another in the Editor. Use **Primary** / **Secondary** options when running two standalone builds to avoid PlayerPrefs conflicts.

---

## Key libraries

[Mirror](https://mirror-networking.com) · [Zenject](https://github.com/modesttree/Zenject) · [UniRx](https://github.com/neuecc/UniRx) · [UniTask](https://github.com/Cysharp/UniTask) · [gRPC](https://grpc.io) · [Algorand Unity SDK](https://github.com/careboo/unity-algorand-sdk) · [Immutable Passport SDK](https://docs.immutable.com)
