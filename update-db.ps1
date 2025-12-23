Write-Host "=== Atualizando Banco de Dados ===" -ForegroundColor Green

dotnet ef database update `
    --project src/Falcon.Infrastructure `
    --startup-project src/Falcon.Api

Write-Host "`n✅ Processo finalizado." -ForegroundColor Green