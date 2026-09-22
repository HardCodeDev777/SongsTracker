#!/bin/bash
echo "Building bot..."
docker compose build --no-cache
docker compose up -d --force-recreate

echo "Bot updated. Latest logs:"
docker compose logs -f --tail=10