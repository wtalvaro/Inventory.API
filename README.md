---

# 🚀 RetailPro - Enterprise Sales & Inventory

<div align="center">

**Plataforma Híbrida de Gestão de Inventário e Assistência de Vendas**

</div>

## ✨ A Nova Realidade Técnica

O RetailPro evoluiu de um script de interface para um sistema **Enterprise-Ready**. A arquitetura atual utiliza o **Razor Pages** como "Cérebro" (Estado e Segurança) e **Vanilla JS** como "Músculo" (Interatividade).

### 🛡️ Segurança por Design (Anti-F12)

Diferente de sistemas puramente front-end, o RetailPro implementa segurança no nível do servidor:

* **Role-Based Access Control (RBAC):** Seções restritas (como Telemetria) não são apenas "escondidas", elas **não são renderizadas** pelo servidor para usuários sem permissão.
* **Cookie Authentication:** Migramos do armazenamento inseguro de tokens no LocalStorage para Cookies protegidos pelo servidor.
* **SameStore Requirement:** Filtros automáticos garantem que gerentes vejam apenas os dados de sua unidade, enquanto coordenadores possuem visão global.

---

## 🏗️ Arquitetura Híbrida

| Camada | Responsabilidade | Tecnologia |
| --- | --- | --- |
| **Cérebro (Razor)** | Autenticação, Autorização, Estado da Sessão e Pré-carregamento de dados | ASP.NET Core 10 |
| **Executor (JS)** | Atualizações de estoque via API, Temporizadores em tempo real e Navegação SPA | Vanilla JavaScript |
| **Dados** | Relacionamentos complexos entre Lojas, Produtos e SKUs | PostgreSQL + EF Core |

---

## 🎯 Funcionalidades Atuais

### 📦 Gestão de Inventário Inteligente

* **Filtro Global Zero-Latency:** Coordenadores alternam entre lojas com preenchimento instantâneo via servidor.
* **Sincronização de Estoque:** Atualizações via chamadas `PATCH` que refletem imediatamente na UI sem recarregar a página.
* **Mapeamento SKU/LocalPrice:** Suporte a preços específicos por unidade física.

### ⏱️ Session Intelligence

* **Relógio Sincronizado:** O temporizador de sessão inicia baseado no tempo real restante do Cookie de autenticação, evitando reinícios no "F5".
* **Alerta de Expiração:** Feedback visual (pulse vermelho) configurado via servidor quando restam menos de 10 minutos.

---

## 📡 API Endpoints (Core)

| Método | Endpoint | Descrição |
| --- | --- | --- |
| **POST** | `/api/auth/login` | Autenticação e emissão de Cookie |
| **GET** | `/api/inventory/store/{id}` | Busca estoque específico da loja |
| **PATCH** | `/api/inventory/{id}/stock` | Incremento/Decremento de unidades |
| **GET** | `/api/stores` | Lista de unidades (Apenas Coordenadores) |

---

## 📁 Estrutura do Projeto Atualizada

```
📦 RetailPro
├── 📁 Inventory.API
│   ├── 📁 Controllers       # APIs de estoque e autenticação
│   ├── 📁 Pages             # Razor Pages (Index.cshtml + Login Logic)
│   ├── 📁 Services          # Regras de negócio e IAuthService
│   ├── 📁 Dtos              # Objetos de transferência (LoginRequest, etc)
│   └── 📄 Program.cs        # Configuração de Auth, Cookies e DI
├── 📁 wwwroot
│   ├── 📁 js
│   │   └── 📄 app.js        # O "Músculo" do Front-end
│   └── 📁 css
│       └── 📄 styles.css    # Tailwind e custom styles

```

---

## 🚀 Como Executar

1. **Configuração do Banco:** Certifique-se de que o PostgreSQL está rodando e a connection string no `appsettings.json` está correta.
2. **Migrações:** Execute `dotnet ef database update`.
3. **Execução:** `dotnet run`.
4. **Acesso:** O sistema identificará automaticamente o estado de login e servirá a interface correta baseada no cargo do usuário.

---

**Desenvolvido para máxima segurança e performance no varejo moderno.**

---