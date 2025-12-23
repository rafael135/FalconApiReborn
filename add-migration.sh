#!/bin/bash

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${GREEN}=== Criar Nova Migration (EF Core) ===${NC}"
echo "Certifique-se de estar na raiz da Solution."
echo ""

# 1. Pede o nome da migration
read -p "Digite o nome da Migration (ex: CreateUsersTable): " migrationName

# 2. Valida se o usuário digitou algo
if [ -z "$migrationName" ]; then
    echo -e "${RED}Erro: O nome da migration não pode estar vazio.${NC}"
    exit 1
fi

echo ""
echo -e "Criando migration: ${GREEN}$migrationName${NC}..."

# 3. Executa o comando do EF Core
dotnet ef migrations add "$migrationName" \
    --project src/Falcon.Infrastructure \
    --startup-project src/Falcon.Api

if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}✅ Migration criada com sucesso!${NC}"
    echo "Não esqueça de rodar o ./update-db.sh para aplicar no banco."
else
    echo ""
    echo -e "${RED}❌ Falha ao criar migration.${NC}"
fi