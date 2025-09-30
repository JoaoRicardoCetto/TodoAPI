# 🐘 Tutorial: PostgreSQL Local - Instalação e Configuração

Este tutorial ensina como instalar e configurar o PostgreSQL localmente no Windows para usar com nossa TodoAPI.

## 📋 Índice

- [Métodos de Instalação](#-métodos-de-instalação)
- [Método 1: Instalação Manual](#-método-1-instalação-manual)
- [Método 2: Docker (Recomendado)](#-método-2-docker-recomendado)
- [Configuração da Aplicação](#-configuração-da-aplicação)
- [Testando a Conexão](#-testando-a-conexão)
- [Solução de Problemas](#-solução-de-problemas)

## 🛠️ Métodos de Instalação

### Método 1: Instalação Manual ⚙️
- Instalação completa do PostgreSQL no Windows
- Mais controle sobre configurações
- Requer mais configuração manual

### Método 2: Docker (Recomendado) 🐳
- Mais simples e rápido
- Isolado do sistema
- Fácil de remover completamente

---

## 🖥️ Método 1: Instalação Manual

### Passo 1: Download do PostgreSQL

1. Acesse o site oficial: https://www.postgresql.org/download/windows/
2. Clique em **"Download the installer"**
3. Baixe a versão mais recente (ex: PostgreSQL 15)

### Passo 2: Instalação

1. **Execute o instalador** como administrador
2. **Escolha o diretório** de instalação (padrão: `C:\Program Files\PostgreSQL\15`)
3. **Selecione componentes**:
   - ✅ PostgreSQL Server
   - ✅ pgAdmin 4 (interface gráfica)
   - ✅ Stack Builder
   - ✅ Command Line Tools

### Passo 3: Configuração Inicial

1. **Senha do usuário postgres**:
   ```
   Digite: postgres
   (ou escolha uma senha forte)
   ```

2. **Porta**:
   ```
   Mantenha: 5432
   ```

3. **Localização**:
   ```
   Selecione: Portuguese, Brazil
   ```

### Passo 4: Verificar Instalação

1. **Abra o Prompt de Comando** como administrador
2. **Navegue até a pasta do PostgreSQL**:
   ```cmd
   cd "C:\Program Files\PostgreSQL\15\bin"
   ```

3. **Teste a conexão**:
   ```cmd
   psql -U postgres -h localhost
   ```

4. **Digite a senha** quando solicitado
5. **Digite** para sair:
   ```sql
   \q
   ```

### Passo 5: Configurar Variáveis de Ambiente (Opcional)

1. **Abra as Variáveis de Ambiente**:
   - Windows + R → `sysdm.cpl` → Avançado → Variáveis de Ambiente

2. **Adicione ao PATH**:
   ```
   C:\Program Files\PostgreSQL\15\bin
   ```

3. **Reinicie o Prompt de Comando**

---

## 🐳 Método 2: Docker (Recomendado)

### Pré-requisitos

- Docker Desktop instalado
- Docker Compose (vem com Docker Desktop)

### Passo 1: Verificar Docker

```cmd
docker --version
docker-compose --version
```

### Passo 2: Usar o Docker Compose

O arquivo `docker-compose.yml` já está configurado no projeto:

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15
    container_name: todoapi-postgres
    environment:
      POSTGRES_DB: TodoDB
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    restart: unless-stopped

volumes:
  postgres_data:
```

### Passo 3: Iniciar o PostgreSQL

```cmd
# Na pasta do projeto TodoAPI
docker-compose up -d postgres
```

### Passo 4: Verificar se está rodando

```cmd
docker ps
```

Você deve ver algo como:
```
CONTAINER ID   IMAGE         COMMAND                  CREATED         STATUS         PORTS                    NAMES
abc123def456   postgres:15   "docker-entrypoint.s…"   2 minutes ago   Up 2 minutes   0.0.0.0:5432->5432/tcp   todoapi-postgres
```

---

## ⚙️ Configuração da Aplicação

### Passo 1: Atualizar String de Conexão

O arquivo `appsettings.json` já está configurado:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=TodoDB;Username=postgres;Password=postgres"
  }
}
```

### Passo 2: Aplicar Migrações

```cmd
# Na pasta do projeto TodoAPI
dotnet ef database update
```

### Passo 3: Executar a Aplicação

```cmd
dotnet run
```

---

## 🧪 Testando a Conexão

### Método 1: Via Aplicação

1. **Execute a aplicação**:
   ```cmd
   dotnet run
   ```

2. **Teste criar uma tarefa**:
   ```cmd
   curl -X POST "https://localhost:5001/api/Todo" ^
        -H "Content-Type: application/json" ^
        -d "{\"descricao\": \"Teste PostgreSQL Local\"}"
   ```

3. **Liste as tarefas**:
   ```cmd
   curl -X GET "https://localhost:5001/api/Todo"
   ```

### Método 2: Via pgAdmin (se instalado manualmente)

1. **Abra o pgAdmin 4**
2. **Conecte ao servidor**:
   - Host: `localhost`
   - Port: `5432`
   - Database: `postgres`
   - Username: `postgres`
   - Password: `postgres`

3. **Navegue até**: Servers → PostgreSQL 15 → Databases → TodoDB → Schemas → public → Tables → todos

### Método 3: Via psql (linha de comando)

```cmd
# Conectar ao banco
psql -h localhost -U postgres -d TodoDB

# Listar tabelas
\dt

# Ver dados da tabela todos
SELECT * FROM "Todos";

# Sair
\q
```

---

## 🚨 Solução de Problemas

### Problema: "Connection refused"

**Causa**: PostgreSQL não está rodando

**Solução**:
```cmd
# Verificar se está rodando (Docker)
docker ps

# Iniciar se parado (Docker)
docker-compose up -d postgres

# Verificar se está rodando (Instalação manual)
# Abrir Services (services.msc) e verificar "postgresql-x64-15"
```

### Problema: "Authentication failed"

**Causa**: Credenciais incorretas

**Solução**:
1. Verificar usuário e senha no `appsettings.json`
2. Para Docker, verificar variáveis no `docker-compose.yml`
3. Para instalação manual, redefinir senha do postgres

### Problema: "Database does not exist"

**Causa**: Banco TodoDB não foi criado

**Solução**:
```cmd
# Conectar como postgres
psql -h localhost -U postgres

# Criar o banco
CREATE DATABASE "TodoDB";

# Sair
\q

# Aplicar migrações
dotnet ef database update
```

### Problema: "Port 5432 is already in use"

**Causa**: Outro PostgreSQL ou serviço usando a porta

**Solução**:
```cmd
# Verificar o que está usando a porta
netstat -ano | findstr :5432

# Parar o serviço PostgreSQL (se necessário)
net stop postgresql-x64-15

# Ou mudar a porta no docker-compose.yml
```

### Problema: "dotnet ef command not found"

**Causa**: Ferramentas EF Core não instaladas

**Solução**:
```cmd
dotnet tool install --global dotnet-ef
```

---

## 📊 Comparação dos Métodos

| Aspecto | Instalação Manual | Docker |
|---------|------------------|---------|
| **Facilidade** | ⚠️ Moderada | ✅ Fácil |
| **Controle** | ✅ Total | ⚠️ Limitado |
| **Performance** | ✅ Nativa | 🐌 Virtualizada |
| **Isolamento** | ❌ Sistema | ✅ Container |
| **Remoção** | ⚠️ Complexa | ✅ Simples |
| **Configuração** | ⚠️ Manual | ✅ Automática |

---

## 🎯 Recomendação

**Para desenvolvimento e aprendizado**: Use **Docker**
- Mais simples de configurar
- Fácil de remover completamente
- Isolado do sistema

**Para produção**: Use **Instalação Manual**
- Melhor performance
- Mais controle sobre configurações
- Integração nativa com sistema

---

## 🚀 Próximos Passos

Após configurar o PostgreSQL:

1. **Teste a aplicação** com os comandos curl
2. **Explore o pgAdmin** (se instalado manualmente)
3. **Crie mais migrações** conforme necessário
4. **Configure backup** para produção

---

**🎉 Parabéns!** Seu PostgreSQL local está configurado e funcionando!
