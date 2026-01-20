---

# 🚀 RetailPro - Enterprise Sales & Inventory

<div align="center">

**Plataforma Híbrida de Gestão de Inventário e Assistência de Vendas**

</div>

---

## 💡 Visão de Engenharia e Contexto

> **"Do Controle de Estados ao Controle de Processos"**
> Este projeto foi concebido como um laboratório de engenharia de software para demonstrar maturidade técnica e lógica de sistemas. Embora o domínio seja o varejo, o RetailPro é uma prova de conceito sobre **precisão, resiliência e gestão de estados**.
> Diante da ausência de ferramentas de hardware para prototipagem de microcontroladores no momento, apliquei a mesma disciplina exigida na automação industrial para resolver o problema crítico de inventário dinâmico. Se um sistema pode gerenciar com integridade total a complexidade de múltiplos SKUs e grades em tempo real, ele compartilha a mesma base lógica necessária para o controle de fluxos e processos industriais.
> **O RetailPro prova que a capacidade de estruturar soluções complexas e entregar um produto finalizado é independente da stack, mas totalmente dependente da lógica de engenharia.**

---

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

## 🐧 Infraestrutura e Stack Linux

O desenvolvimento e deploy do RetailPro foram validados em ambiente **Linux (AlmaLinux e Arch)**, demonstrando domínio em:

* Configuração de ambientes estáveis para missão crítica.
* Gerenciamento de containers e performance de sistema.
* Automação de processos via CLI.

---

## 📡 API Endpoints (Core)

| Método | Endpoint | Descrição |
| --- | --- | --- |
| **POST** | `/api/auth/login` | Autenticação e emissão de Cookie |
| **GET** | `/api/inventory/store/{id}` | Busca estoque específico da loja |
| **PATCH** | `/api/inventory/{id}/stock` | Incremento/Decremento de unidades |
| **GET** | `/api/stores` | Lista de unidades (Apenas Coordenadores) |

---

## 📁 Estrutura do Projeto

```
📦 RetailPro
├── 📁 Inventory.API
│   ├── 📁 Controllers       # APIs de estoque e autenticação
│   ├── 📁 Pages             # Razor Pages (Index.cshtml + Login Logic)
│   ├── 📁 Services          # Regras de negócio e IAuthService
│   ├── 📁 Dtos              # Objetos de transferência (LoginRequest, etc)
│   └── 📄 Program.cs        # Configuração de Auth, Cookies e DI
├── 📁 wwwroot
│   ├── 📁 js
│   │   └── 📄 app.js        # O "Músculo" do Front-end
│   └── 📁 css
│       └── 📄 styles.css    # Tailwind e custom styles

```

---

**Desenvolvido para máxima segurança e performance sob uma ótica de engenharia robusta.**

---
