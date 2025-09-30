# 📚 Conceitos Básicos para Entender a TodoAPI

Este documento explica os conceitos fundamentais que você precisa saber antes de seguir o tutorial da TodoAPI.

## 🎯 Índice

- [O que é uma API?](#-o-que-é-uma-api)
- [O que é um Backend?](#-o-que-é-um-backend)
- [O que é um Banco de Dados?](#-o-que-é-um-banco-de-dados)
- [O que é REST?](#-o-que-é-rest)
- [O que é HTTP?](#-o-que-é-http)
- [Arquitetura em Camadas](#-arquitetura-em-camadas)
- [Por que separar em camadas?](#-por-que-separar-em-camadas)
- [O que é Entity Framework?](#-o-que-é-entity-framework)
- [O que são Migrações?](#-o-que-são-migrações)
- [O que é Dependency Injection?](#-o-que-é-dependency-injection)

---

## 🌐 O que é uma API?

### Analogia: O Garçom do Restaurante

Imagine que você está em um restaurante:

- **Você (Cliente)** = Aplicação que quer dados
- **Garçom (API)** = Intermediário que leva pedidos
- **Cozinha (Backend)** = Onde os dados são processados
- **Cardápio (Documentação)** = Lista do que você pode pedir

### Definição Técnica

**API** = Application Programming Interface (Interface de Programação de Aplicações)

- É um **"intermediário"** entre diferentes programas
- Permite que aplicações **"conversem"** entre si
- Define **regras** de como solicitar e receber dados

### Exemplo Prático

```bash
# Quando você faz uma requisição:
GET /api/todo

# A API responde com dados:
[
  {"id": "123", "descricao": "Estudar C#", "completo": false},
  {"id": "456", "descricao": "Fazer exercícios", "completo": true}
]
```

---

## 🖥️ O que é um Backend?

### Analogia: A Cozinha do Restaurante

- **Frontend** = Sala do restaurante (onde você está)
- **Backend** = Cozinha (onde a comida é preparada)
- **API** = Garçom (leva pedidos da sala para a cozinha)

### Definição Técnica

**Backend** é a parte "invisível" de uma aplicação:

- **Processa** as requisições
- **Acessa** o banco de dados
- **Aplica** regras de negócio
- **Retorna** os dados processados

### O que o Backend faz na TodoAPI?

1. **Recebe** pedido: "Criar nova tarefa"
2. **Valida** os dados: "Descrição não pode estar vazia"
3. **Salva** no banco de dados
4. **Retorna** confirmação: "Tarefa criada com sucesso"

---

## 🗄️ O que é um Banco de Dados?

### Analogia: Um Armário Organizado

Imagine um armário com gavetas bem organizadas:

- **Armário** = Banco de dados
- **Gavetas** = Tabelas (ex: tabela de tarefas)
- **Itens nas gavetas** = Registros (ex: tarefa individual)

### Definição Técnica

**Banco de Dados** é um sistema que:

- **Armazena** dados de forma organizada
- **Permite** buscar, adicionar, modificar e remover dados
- **Garante** que os dados não se percam
- **Mantém** a integridade dos dados

### Exemplo: Tabela de Tarefas

| ID | Descrição | Completo |
|----|-----------|----------|
| 1 | Estudar C# | false |
| 2 | Fazer exercícios | true |
| 3 | Ler documentação | false |

### Por que usar PostgreSQL?

- **Confiavel**: Dados não se perdem
- **Rápido**: Busca dados rapidamente
- **Escalável**: Suporta muitos usuários
- **Padrão**: Usado em produção

---

## 🔗 O que é REST?

### Analogia: Métodos de Comunicação

Imagine diferentes formas de se comunicar:

- **GET** = "Me mostre..." (como perguntar algo)
- **POST** = "Crie para mim..." (como fazer um pedido)
- **PUT** = "Atualize..." (como pedir uma mudança)
- **DELETE** = "Remova..." (como pedir para apagar)

### Definição Técnica

**REST** = Representational State Transfer

- É um **padrão** para criar APIs
- Usa **verbos HTTP** para diferentes ações
- **Padroniza** como fazer requisições
- **Facilita** a comunicação entre sistemas

### Exemplos na TodoAPI

```bash
GET    /api/todo          # Buscar todas as tarefas
GET    /api/todo/123      # Buscar tarefa específica
POST   /api/todo          # Criar nova tarefa
PUT    /api/todo/123      # Atualizar tarefa
DELETE /api/todo/123      # Remover tarefa
```

---

## 🌍 O que é HTTP?

### Analogia: Carta Postal

- **Remetente** = Cliente (aplicação que faz pedido)
- **Destinatário** = Servidor (onde está a API)
- **Carta** = Requisição HTTP
- **Resposta** = Carta de volta

### Definição Técnica

**HTTP** = HyperText Transfer Protocol

- É o **"idioma"** da internet
- Define como **requisições** e **respostas** funcionam
- Usa **códigos de status** para indicar resultados

### Códigos de Status HTTP

| Código | Significado | Quando acontece |
|--------|-------------|-----------------|
| 200 | OK | Sucesso |
| 201 | Created | Item criado |
| 400 | Bad Request | Dados inválidos |
| 404 | Not Found | Item não encontrado |
| 500 | Server Error | Erro no servidor |

---

## 🏗️ Arquitetura em Camadas

### Analogia: Prédio com Andares

Imagine um prédio de 3 andares:

- **3º Andar (Presentation)** = Recepção (onde as pessoas chegam)
- **2º Andar (Application)** = Escritórios (onde o trabalho é feito)
- **1º Andar (Domain)** = Fundamentos (regras básicas)

### As 3 Camadas da TodoAPI

#### 🎯 **Domain** (Fundação)
- **O que faz**: Define "o que é uma tarefa"
- **Contém**: Classe `Todo` com propriedades
- **Não depende** de nada externo

#### 🔧 **Application** (Lógica de Negócio)
- **O que faz**: Implementa as regras de negócio
- **Contém**: Services e Repositories
- **Depende** apenas do Domain

#### 🌐 **Presentation** (Interface)
- **O que faz**: Expõe a API para o mundo
- **Contém**: Controllers e DTOs
- **Depende** do Application

### Fluxo de Dados

```
Cliente → Presentation → Application → Domain
                   ↓
Cliente ← Presentation ← Application ← Domain
```

---

## ❓ Por que separar em camadas?

### Analogia: Organização de uma Empresa

Imagine uma empresa bem organizada:

- **Departamento Comercial** = Presentation (atende clientes)
- **Departamento de Produção** = Application (faz o trabalho)
- **Regulamentações** = Domain (regras que todos seguem)

### Benefícios da Separação

#### 🎯 **Manutenibilidade**
- Cada camada tem **responsabilidade específica**
- Mudanças em uma camada **não afetam** as outras
- **Fácil** de encontrar e corrigir problemas

#### 🔄 **Reutilização**
- **Domain** pode ser usado em outras aplicações
- **Application** pode ter diferentes interfaces (API, Console, etc.)
- **Código** não é duplicado

#### 🧪 **Testabilidade**
- Cada camada pode ser **testada isoladamente**
- **Mocks** podem substituir dependências
- **Testes** são mais rápidos e confiáveis

#### 👥 **Trabalho em Equipe**
- Diferentes pessoas podem trabalhar em **camadas diferentes**
- **Conflitos** de código são reduzidos
- **Especialização** por área

---

## 🔧 O que é Entity Framework?

### Analogia: Tradutor Automático

Imagine um tradutor que:

- **Entende** tanto português quanto inglês
- **Converte** automaticamente entre os dois idiomas
- **Não precisa** que você saiba inglês

### Definição Técnica

**Entity Framework** é um **ORM** (Object-Relational Mapping):

- **Converte** objetos C# em tabelas de banco
- **Converte** dados do banco em objetos C#
- **Automatiza** operações de banco de dados
- **Elimina** a necessidade de escrever SQL

### Exemplo Prático

#### Sem Entity Framework (SQL puro):
```sql
INSERT INTO Todos (Id, Descricao, Completo) 
VALUES ('123', 'Estudar C#', false);
```

#### Com Entity Framework:
```csharp
var todo = new Todo { Descricao = "Estudar C#", Completo = false };
await _context.Todos.AddAsync(todo);
await _context.SaveChangesAsync();
```

### Vantagens do Entity Framework

- ✅ **Menos código**: Não precisa escrever SQL
- ✅ **Tipagem forte**: Erros detectados em tempo de compilação
- ✅ **Portabilidade**: Funciona com diferentes bancos
- ✅ **Produtividade**: Desenvolvimento mais rápido

---

## 🚀 O que são Migrações?

### Analogia: Planta de uma Casa

Imagine que você quer **modificar sua casa**:

1. **Planta atual** = Estrutura atual do banco
2. **Planta nova** = Estrutura desejada do banco
3. **Reforma** = Migração (mudanças necessárias)
4. **Resultado** = Casa reformada

### Definição Técnica

**Migração** é um arquivo que:

- **Descreve** mudanças na estrutura do banco
- **Permite** aplicar mudanças de forma controlada
- **Mantém** histórico de todas as alterações
- **Facilita** atualizações em diferentes ambientes

### Exemplo: Migração para adicionar campo

#### Antes (estrutura atual):
```sql
CREATE TABLE Todos (
    Id uuid PRIMARY KEY,
    Descricao varchar(500)
);
```

#### Depois (nova estrutura):
```sql
CREATE TABLE Todos (
    Id uuid PRIMARY KEY,
    Descricao varchar(500),
    Completo boolean DEFAULT false  -- NOVO CAMPO
);
```

#### Migração gerada:
```csharp
public partial class AddCompletoField : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "Completo",
            table: "Todos",
            defaultValue: false);
    }
}
```

### Comandos de Migração

```bash
# Criar nova migração
dotnet ef migrations add NomeDaMigracao

# Aplicar migrações
dotnet ef database update

# Ver histórico
dotnet ef migrations list
```

---

## 💉 O que é Dependency Injection?

### Analogia: Loja de Ferramentas

Imagine que você é um mecânico:

- **Você** não fabrica suas próprias ferramentas
- **Loja** fornece as ferramentas que você precisa
- **Você** só precisa saber como usar, não como fazer

### Definição Técnica

**Dependency Injection** (DI) é um padrão que:

- **Fornece** dependências automaticamente
- **Evita** criar objetos manualmente
- **Facilita** testes e manutenção
- **Desacopla** classes

### Exemplo Prático

#### Sem DI (problema):
```csharp
public class TodoService
{
    public TodoService()
    {
        // Cria dependência diretamente (ruim!)
        var repository = new TodoDbRepository();
    }
}
```

#### Com DI (solução):
```csharp
public class TodoService
{
    private readonly IBaseRepository<Todo> _repository;
    
    // DI injeta a dependência automaticamente
    public TodoService(IBaseRepository<Todo> repository)
    {
        _repository = repository;
    }
}
```

#### Configuração no Program.cs:
```csharp
// Registra as dependências
builder.Services.AddScoped<IBaseRepository<Todo>, TodoDbRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();
```

### Benefícios do DI

- ✅ **Testabilidade**: Fácil criar mocks
- ✅ **Flexibilidade**: Trocar implementações facilmente
- ✅ **Manutenibilidade**: Código mais limpo
- ✅ **Reutilização**: Dependências compartilhadas

---

## 🎓 Resumo dos Conceitos

### O que você precisa saber:

1. **API** = Intermediário entre aplicações
2. **Backend** = Parte que processa dados
3. **Banco de Dados** = Armazena dados organizadamente
4. **REST** = Padrão para criar APIs
5. **HTTP** = Protocolo de comunicação da internet
6. **Camadas** = Organização do código por responsabilidade
7. **Entity Framework** = Ferramenta que facilita acesso ao banco
8. **Migrações** = Controle de mudanças no banco
9. **Dependency Injection** = Fornece dependências automaticamente

### Por que esses conceitos são importantes?

- **Entender o contexto** ajuda a não apenas copiar código
- **Saber o "porquê"** facilita a resolução de problemas
- **Conceitos claros** permitem evoluir a aplicação
- **Base sólida** prepara para conceitos mais avançados

---

## 🚀 Próximo Passo

Agora que você entende os conceitos básicos, está pronto para seguir o [tutorial principal](README.md)!

**Lembre-se**: Se algo não ficar claro durante o tutorial, volte aqui para relembrar os conceitos fundamentais.

---

**💡 Dica**: Mantenha este arquivo aberto enquanto segue o tutorial. Ele servirá como seu "dicionário" de conceitos!
