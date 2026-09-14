@echo off

cd ..

set /p migrationName="Name of the migration: "

dotnet ef migrations add %migrationName% --output-dir Data\Migrations
dotnet ef database update

echo Done.
pause