> Pet project
# SongsTracker

Telegram bot for monitoring new song releases of your favourite artists. Made with **ITunes API, ASP.NET Core** and `Telegram.Bot`. 

---

## Key Features
- Allows to add and remove tracked artists via commands like `/add_artist` and `/remove_artist`
- Keeps tracked artists in db (SQLite)
- Filters users so bot won't be spammed by a random person
- Can be run in Docker and after restart db won't override

---

## Running via Docker

Before running you need to configure enviroment variables. Template is [here](src/.env.template)

```cmd
docker compose up -d
```
