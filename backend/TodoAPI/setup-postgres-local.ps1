# 🐘 Script de Configuração PostgreSQL Local
# Este script automatiza a configuração do PostgreSQL para desenvolvimento local

param(
    [switch]$UseDocker = $true,
    [switch]$SkipDockerCheck = $false
)

Write-Host "🚀 Configurando PostgreSQL Local para TodoAPI" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green

# Função para verificar se um comando existe
function Test-Command($cmdname) {
    return [bool](Get-Command -Name $cmdname -ErrorAction SilentlyContinue)
}

# Função para aguardar entrada do usuário
function Wait-UserInput($message) {
    Write-Host $message -ForegroundColor Yellow
    Read-Host "Pressione Enter para continuar"
}

# Verificar se estamos na pasta correta
if (-not (Test-Path "TodoAPI.csproj")) {
    Write-Host "❌ Erro: Execute este script na pasta raiz do projeto TodoAPI" -ForegroundColor Red
    exit 1
}

if ($UseDocker) {
    Write-Host "🐳 Configurando PostgreSQL via Docker..." -ForegroundColor Cyan
    
    # Verificar Docker
    if (-not $SkipDockerCheck) {
        if (-not (Test-Command "docker")) {
            Write-Host "❌ Docker não encontrado. Instale o Docker Desktop primeiro." -ForegroundColor Red
            Write-Host "📥 Download: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
            exit 1
        }
        
        if (-not (Test-Command "docker-compose")) {
            Write-Host "❌ Docker Compose não encontrado." -ForegroundColor Red
            exit 1
        }
    }
    
    # Verificar se docker-compose.yml existe
    if (-not (Test-Path "docker-compose.yml")) {
        Write-Host "❌ Arquivo docker-compose.yml não encontrado." -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✅ Docker encontrado" -ForegroundColor Green
    
    # Parar containers existentes
    Write-Host "🛑 Parando containers existentes..." -ForegroundColor Yellow
    docker-compose down 2>$null
    
    # Iniciar PostgreSQL
    Write-Host "🚀 Iniciando PostgreSQL..." -ForegroundColor Yellow
    docker-compose up -d postgres
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ PostgreSQL iniciado com sucesso!" -ForegroundColor Green
    } else {
        Write-Host "❌ Erro ao iniciar PostgreSQL" -ForegroundColor Red
        exit 1
    }
    
    # Aguardar PostgreSQL ficar pronto
    Write-Host "⏳ Aguardando PostgreSQL ficar pronto..." -ForegroundColor Yellow
    Start-Sleep -Seconds 10
    
    # Verificar se está rodando
    $containerStatus = docker ps --filter "name=todoapi-postgres" --format "{{.Status}}"
    if ($containerStatus -match "Up") {
        Write-Host "✅ PostgreSQL está rodando!" -ForegroundColor Green
    } else {
        Write-Host "❌ PostgreSQL não está rodando corretamente" -ForegroundColor Red
        Write-Host "📋 Logs do container:" -ForegroundColor Yellow
        docker logs todoapi-postgres
        exit 1
    }
    
} else {
    Write-Host "🖥️ Configurando PostgreSQL via instalação manual..." -ForegroundColor Cyan
    Write-Host "⚠️  Certifique-se de que o PostgreSQL está instalado e rodando" -ForegroundColor Yellow
    
    # Verificar se psql está disponível
    if (-not (Test-Command "psql")) {
        Write-Host "❌ psql não encontrado. Instale o PostgreSQL primeiro." -ForegroundColor Red
        Write-Host "📥 Download: https://www.postgresql.org/download/windows/" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "✅ PostgreSQL encontrado" -ForegroundColor Green
}

# Verificar .NET EF Tools
Write-Host "🔧 Verificando Entity Framework Tools..." -ForegroundColor Cyan
if (-not (Test-Command "dotnet-ef")) {
    Write-Host "📦 Instalando Entity Framework Tools..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Erro ao instalar Entity Framework Tools" -ForegroundColor Red
        exit 1
    }
}

# Restaurar pacotes
Write-Host "📦 Restaurando pacotes NuGet..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erro ao restaurar pacotes" -ForegroundColor Red
    exit 1
}

# Compilar projeto
Write-Host "🔨 Compilando projeto..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erro na compilação" -ForegroundColor Red
    exit 1
}

# Aplicar migrações
Write-Host "🗄️ Aplicando migrações..." -ForegroundColor Yellow
dotnet ef database update
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erro ao aplicar migrações" -ForegroundColor Red
    Write-Host "💡 Verifique se o PostgreSQL está rodando e as credenciais estão corretas" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Migrações aplicadas com sucesso!" -ForegroundColor Green

# Testar conexão criando uma tarefa de teste
Write-Host "🧪 Testando conexão..." -ForegroundColor Yellow

# Aguardar um pouco para garantir que a API está pronta
Start-Sleep -Seconds 5

Write-Host "🎉 Configuração concluída com sucesso!" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green
Write-Host ""
Write-Host "🚀 Para executar a aplicação:" -ForegroundColor Cyan
Write-Host "   dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "🌐 Acessos disponíveis:" -ForegroundColor Cyan
Write-Host "   • API: https://localhost:5001" -ForegroundColor White
Write-Host "   • Swagger: https://localhost:5001/swagger" -ForegroundColor White
Write-Host ""
Write-Host "🧪 Teste rápido:" -ForegroundColor Cyan
Write-Host '   curl -X POST "https://localhost:5001/api/Todo" -H "Content-Type: application/json" -d "{\"descricao\": \"Teste inicial\"}"' -ForegroundColor White
Write-Host ""
Write-Host "📚 Documentação completa:" -ForegroundColor Cyan
Write-Host "   • POSTGRES_LOCAL_SETUP.md" -ForegroundColor White
Write-Host "   • DATABASE_SETUP.md" -ForegroundColor White
Write-Host ""

if ($UseDocker) {
    Write-Host "🐳 Comandos Docker úteis:" -ForegroundColor Cyan
    Write-Host "   • Parar: docker-compose down" -ForegroundColor White
    Write-Host "   • Ver logs: docker logs todoapi-postgres" -ForegroundColor White
    Write-Host "   • Resetar dados: docker-compose down -v" -ForegroundColor White
    Write-Host ""
}

Wait-UserInput "Pressione Enter para finalizar"
