Write-Host "=== Criar Nova Migration (EF Core) ===" -ForegroundColor Green

# 1. Pede o nome
$migrationName = Read-Host "Digite o nome da Migration (ex: CreateUsersTable)"

# 2. Valida
if ([string]::IsNullOrWhiteSpace($migrationName)) {
    Write-Host "Erro: O nome da migration não pode estar vazio." -ForegroundColor Red
    exit
}

Write-Host "Criando migration: $migrationName..." -ForegroundColor Cyan

# 3. Executa
dotnet ef migrations add $migrationName `
    --project src/Falcon.Infrastructure `
    --startup-project src/Falcon.Api

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ Migration criada com sucesso!" -ForegroundColor Green
    Write-Host "Execute .\update-db.ps1 para aplicar no banco." -ForegroundColor Yellow
} else {
    Write-Host "`n❌ Falha ao criar migration." -ForegroundColor Red
}