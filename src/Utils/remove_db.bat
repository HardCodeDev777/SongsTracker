@echo off

cd ..
cd bin\Debug\net10.0

del Tracker.db
del Tracker.db-shm
del Tracker.db-wal

echo Done.
pause