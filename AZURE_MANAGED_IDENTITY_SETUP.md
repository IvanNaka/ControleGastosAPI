# Guia Completo: Azure Managed Identity para ControleGastos API

## ?? O que é Managed Identity?

**Managed Identity** é um recurso do Azure que permite que seus aplicativos se autentiquem em serviços Azure (como SQL Database) **sem precisar de senhas ou connection strings** no código.

### Benefícios
- ? **Sem senhas** no código ou arquivos de configuração
- ? **Mais seguro** - credenciais gerenciadas pelo Azure
- ? **Rotação automática** - Azure gerencia tokens
- ? **Auditoria completa** - logs de acesso no Azure AD

---

## ?? Visão Geral do Processo

```
1. Criar App Service no Azure
2. Habilitar Managed Identity no App Service
3. Dar permissão à Managed Identity no SQL Database
4. Atualizar Connection String para usar Managed Identity
5. Deploy da API
```

---

## ?? Pré-requisitos

- Azure Subscription ativa
- Azure SQL Database já criado (`controle-gastos`)
- Azure CLI instalado (opcional, mas recomendado)

---

## ?? Passo a Passo Completo

### **Passo 1: Criar App Service no Azure Portal**

#### Via Portal Azure (GUI)

1. Acesse https://portal.azure.com
2. Clique em **"Create a resource"**
3. Procure por **"App Service"** e clique em **"Create"**
4. Preencha:
   - **Subscription**: Sua subscription
   - **Resource Group**: `controle-gastos-rg` (ou crie novo)
   - **Name**: `controle-gastos-api` (deve ser único globalmente)
   - **Publish**: `Code`
   - **Runtime stack**: `.NET 8 (LTS)`
   - **Operating System**: `Linux` (mais barato) ou `Windows`
   - **Region**: `Brazil South` ou região próxima
   - **Pricing Plan**: 
     - **Development/Test**: `F1` (Free) ou `B1` (Basic)
     - **Production**: `P1V2` ou superior
5. Clique em **"Review + create"**
6. Clique em **"Create"**
7. Aguarde a criação (2-3 minutos)

#### Via Azure CLI (Linha de Comando)

```bash
# Login no Azure
az login

# Criar Resource Group (se não existir)
az group create \
  --name controle-gastos-rg \
  --location brazilsouth

# Criar App Service Plan
az appservice plan create \
  --name controle-gastos-plan \
  --resource-group controle-gastos-rg \
  --sku B1 \
  --is-linux

# Criar App Service
az webapp create \
  --name controle-gastos-api \
  --resource-group controle-gastos-rg \
  --plan controle-gastos-plan \
  --runtime "DOTNET|8.0"
```

---

### **Passo 2: Habilitar Managed Identity**

#### Via Portal Azure

1. No Portal Azure, vá para o **App Service** que você criou
2. No menu lateral, procure por **"Identity"** (seção **Settings**)
3. Na aba **"System assigned"**:
   - **Status**: Mude para **"On"**
   - Clique em **"Save"**
   - Confirme clicando em **"Yes"**
4. Após salvar, será gerado um **Object (principal) ID**
5. **Copie esse Object ID** - você vai precisar dele!

#### Via Azure CLI

```bash
# Habilitar System-assigned Managed Identity
az webapp identity assign \
  --name controle-gastos-api \
  --resource-group controle-gastos-rg

# O comando retornará algo como:
# {
#   "principalId": "12345678-1234-1234-1234-123456789abc",
#   "tenantId": "5f3d3de5-5d5f-45b9-bccf-6b8d5aea1a03",
#   "type": "SystemAssigned"
# }

# Copie o principalId para o próximo passo
```

---

### **Passo 3: Dar Permissão à Managed Identity no SQL Database**

Agora você precisa dar permissão para a Managed Identity acessar o SQL Database.

#### Opção A: Via Portal Azure (SQL Query Editor)

1. No Portal Azure, vá para **SQL databases** > **controle-gastos**
2. No menu lateral, clique em **"Query editor (preview)"**
3. Faça login com suas credenciais de admin do SQL Server
4. Execute o seguinte SQL (substitua `controle-gastos-api` pelo nome do seu App Service):

```sql
-- Criar usuário para a Managed Identity
CREATE USER [controle-gastos-api] FROM EXTERNAL PROVIDER;

-- Dar permissões necessárias
ALTER ROLE db_datareader ADD MEMBER [controle-gastos-api];
ALTER ROLE db_datawriter ADD MEMBER [controle-gastos-api];
ALTER ROLE db_ddladmin ADD MEMBER [controle-gastos-api];

-- Verificar se foi criado
SELECT name, type_desc, authentication_type_desc 
FROM sys.database_principals 
WHERE name = 'controle-gastos-api';
```

#### Opção B: Via Azure CLI + sqlcmd

```bash
# Instalar extensão do Azure SQL (se necessário)
az extension add --name db-up

# Obter o nome do App Service
APP_NAME="controle-gastos-api"

# Conectar ao SQL e executar comandos
# Você precisará do SQL Server admin user e password

az sql db execute \
  --resource-group controle-gastos-rg \
  --server controle-gastos \
  --name controle-gastos \
  --admin-user SEU_ADMIN_USER \
  --admin-password 'SUA_SENHA' \
  --query-text "
    CREATE USER [${APP_NAME}] FROM EXTERNAL PROVIDER;
    ALTER ROLE db_datareader ADD MEMBER [${APP_NAME}];
    ALTER ROLE db_datawriter ADD MEMBER [${APP_NAME}];
    ALTER ROLE db_ddladmin ADD MEMBER [${APP_NAME}];
  "
```

#### Opção C: Via SQL Server Management Studio (SSMS)

1. Conecte-se ao Azure SQL Server usando SSMS
2. Expanda **Databases** > **controle-gastos**
3. Clique com botão direito em **Security** > **Users** > **New User**
4. Em **User name**: `controle-gastos-api`
5. Em **User type**: Selecione **Azure Active Directory user**
6. Em **Database role membership**: Marque:
   - ?? `db_datareader`
   - ?? `db_datawriter`
   - ?? `db_ddladmin`
7. Clique em **OK**

---

### **Passo 4: Atualizar Connection String**

Agora você precisa atualizar a connection string para usar Managed Identity.

#### No appsettings.json (Local - Para referência)

Mantenha a connection string com credenciais para desenvolvimento local:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=controle-gastos.database.windows.net;Initial Catalog=controle-gastos;User ID=admin;Password=senha;Encrypt=True;"
  }
}
```

#### No Azure App Service (Produção)

1. Portal Azure > App Service > **controle-gastos-api**
2. Menu lateral: **Configuration** (em **Settings**)
3. Em **Application settings**, clique em **New connection string**
4. Preencha:
   - **Name**: `DefaultConnection`
   - **Value**: 
   ```
   Server=controle-gastos.database.windows.net;Initial Catalog=controle-gastos;Authentication=Active Directory Managed Identity;Encrypt=True;
   ```
   - **Type**: `SQLAzure`
5. Clique em **OK**
6. Clique em **Save** no topo da página
7. Confirme clicando em **Continue**

#### Via Azure CLI

```bash
# Definir connection string no App Service
az webapp config connection-string set \
  --name controle-gastos-api \
  --resource-group controle-gastos-rg \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=controle-gastos.database.windows.net;Initial Catalog=controle-gastos;Authentication=Active Directory Managed Identity;Encrypt=True;"
```

---

### **Passo 5: Instalar Pacote NuGet (Se necessário)**

Para usar Managed Identity com SQL Server, você precisa do pacote `Microsoft.Data.SqlClient` versão 3.0 ou superior.

```bash
cd ControleGastos.Infrastructure
dotnet add package Microsoft.Data.SqlClient --version 5.2.0
```

Verifique se o `ControleGastos.Infrastructure.csproj` tem:

```xml
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.2.0" />
```

---

### **Passo 6: Deploy da API**

#### Opção A: Publicar via Visual Studio

1. No Visual Studio, clique com botão direito no projeto **ControleGastos.Api**
2. Selecione **Publish**
3. Escolha **Azure** > **Azure App Service (Linux)** ou **(Windows)**
4. Selecione sua subscription e o App Service **controle-gastos-api**
5. Clique em **Publish**
6. Aguarde o deploy completar

#### Opção B: Deploy via Azure CLI

```bash
# Navegar para a pasta da API
cd ControleGastos.Api

# Publicar projeto
dotnet publish -c Release -o ./publish

# Zipar arquivos
cd publish
zip -r ../app.zip .
cd ..

# Deploy para App Service
az webapp deployment source config-zip \
  --name controle-gastos-api \
  --resource-group controle-gastos-rg \
  --src app.zip

# Reiniciar App Service
az webapp restart \
  --name controle-gastos-api \
  --resource-group controle-gastos-rg
```

#### Opção C: Deploy via GitHub Actions (CI/CD)

Crie `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Azure App Service

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Publish
      run: dotnet publish ControleGastos.Api/ControleGastos.Api.csproj -c Release -o ./publish
    
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'controle-gastos-api'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: './publish'
```

Para obter o **Publish Profile**:
1. Portal Azure > App Service > **controle-gastos-api**
2. Clique em **Get publish profile** (topo da página)
3. Copie o conteúdo do arquivo baixado
4. No GitHub: **Settings** > **Secrets and variables** > **Actions** > **New repository secret**
5. Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
6. Value: Cole o conteúdo do publish profile
7. Clique em **Add secret**

---

### **Passo 7: Verificar e Testar**

#### Verificar Logs do App Service

1. Portal Azure > App Service > **controle-gastos-api**
2. Menu lateral: **Log stream**
3. Observe os logs para ver se há erros de conexão

#### Testar Endpoint

```bash
# Obter URL do App Service
APP_URL="https://controle-gastos-api.azurewebsites.net"

# Testar health check
curl $APP_URL/health

# Testar com autenticação (use seu token JWT)
curl -X GET "$APP_URL/api/usuarios/me" \
  -H "Authorization: Bearer SEU_TOKEN_JWT"
```

---

## ?? Troubleshooting

### Erro: "Login failed for user 'NT AUTHORITY\\ANONYMOUS LOGON'"

**Causa**: Managed Identity não foi configurada corretamente.

**Solução**:
1. Verifique se Managed Identity está habilitada (Status = On)
2. Verifique se o usuário foi criado no SQL Database
3. Reinicie o App Service

```bash
az webapp restart --name controle-gastos-api --resource-group controle-gastos-rg
```

### Erro: "The user 'controle-gastos-api' does not exist"

**Causa**: O usuário Managed Identity não foi criado no SQL Database.

**Solução**: Execute novamente o SQL no Passo 3:

```sql
CREATE USER [controle-gastos-api] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [controle-gastos-api];
ALTER ROLE db_datawriter ADD MEMBER [controle-gastos-api];
ALTER ROLE db_ddladmin ADD MEMBER [controle-gastos-api];
```

### Erro: "Connection timeout"

**Causa**: Firewall do SQL Server não permite conexões do App Service.

**Solução**:
1. Portal Azure > SQL Server > **Firewalls and virtual networks**
2. Marque: ?? **"Allow Azure services and resources to access this server"**
3. Clique em **Save**

### Erro: "Package Microsoft.Data.SqlClient not found"

**Solução**:

```bash
cd ControleGastos.Infrastructure
dotnet add package Microsoft.Data.SqlClient --version 5.2.0
dotnet restore
```

---

## ?? Configuração Completa (Resumo)

### 1. Connection String com Managed Identity

```
Server=controle-gastos.database.windows.net;Initial Catalog=controle-gastos;Authentication=Active Directory Managed Identity;Encrypt=True;
```

### 2. SQL para Criar Usuário

```sql
CREATE USER [controle-gastos-api] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [controle-gastos-api];
ALTER ROLE db_datawriter ADD MEMBER [controle-gastos-api];
ALTER ROLE db_ddladmin ADD MEMBER [controle-gastos-api];
```

### 3. Pacote NuGet Necessário

```xml
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.2.0" />
```

---

## ?? Desenvolvimento Local vs Produção

### Desenvolvimento Local

Use **User Secrets** com credenciais:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=controle-gastos.database.windows.net;Initial Catalog=controle-gastos;User ID=admin;Password=senha;Encrypt=True;"
```

### Produção (Azure)

Use **Managed Identity** (sem senha):

```
Server=controle-gastos.database.windows.net;Initial Catalog=controle-gastos;Authentication=Active Directory Managed Identity;Encrypt=True;
```

### Código que Funciona em Ambos

O Entity Framework Core detecta automaticamente o tipo de autenticação pela connection string. Nenhuma alteração de código é necessária!

---

## ?? Checklist Final

- [ ] App Service criado no Azure
- [ ] Managed Identity habilitada (Status = On)
- [ ] Usuário criado no SQL Database via SQL
- [ ] Permissões concedidas (db_datareader, db_datawriter, db_ddladmin)
- [ ] Connection String configurada no App Service (Configuration)
- [ ] Firewall do SQL Server permite Azure services
- [ ] Pacote Microsoft.Data.SqlClient versão 5.0+ instalado
- [ ] Deploy da API feito para o App Service
- [ ] Testado endpoint com sucesso

---

## ?? Recursos Adicionais

- [Azure Managed Identity Documentation](https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/)
- [SQL Database with Managed Identity](https://learn.microsoft.com/en-us/azure/azure-sql/database/authentication-aad-configure)
- [App Service Managed Identity](https://learn.microsoft.com/en-us/azure/app-service/overview-managed-identity)

---

## ?? Resultado Final

Após seguir todos os passos:
- ? API rodando no Azure App Service
- ? Conexão com SQL Database **sem senhas** no código
- ? Segurança aprimorada com Managed Identity
- ? Autenticação Azure AD funcionando
- ? Endpoints protegidos com JWT

**Sua API está pronta para produção!** ??
