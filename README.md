---

# 🚀 RetailPro - Enterprise Sales & Inventory

<div align="center">

**Plataforma Híbrida de Gestão de Inventário e Assistência de Vendas**

</div>

---

## 💡 Visão de Engenharia e Contexto

> **"Do Controle de Estados ao Controle de Processos"**
> Este projeto foi concebido como um laboratório de engenharia de software para demonstrar maturidade técnica e lógica de sistemas. Embora o domínio seja o varejo, o RetailPro é uma prova de conceito sobre **precisão, resiliência e gestão de estados**.
> Diante da ausência momentânea de ferramentas para prototipagem de hardware e microcontroladores, utilizei o desenvolvimento de software high-level para demonstrar que a **lógica de engenharia** permanece a mesma. Se um sistema pode gerenciar com integridade total a complexidade de múltiplos SKUs e grades em tempo real, ele compartilha a mesma base lógica necessária para o controle de fluxos e processos industriais.
> **O RetailPro prova que a capacidade de estruturar soluções robustas é independente da stack, mas totalmente dependente da disciplina técnica.**

---

## ✨ Diferenciais Técnicos

O RetailPro utiliza uma arquitetura **Enterprise-Ready**, focada em segurança de dados e performance no mundo real.

### 🛡️ Segurança por Design (Pilar de Confiança)

Diferente de sistemas convencionais, o RetailPro implementa segurança no nível do servidor para evitar vulnerabilidades de manipulação via browser:

* **Role-Based Access Control (RBAC):** Funcionalidades sensíveis (como Telemetria e Gestão Global) não são apenas escondidas no front-end; elas **não são renderizadas** pelo servidor se o usuário não possuir a permissão adequada.
* **SameStore Requirement:** Uma política de autorização personalizada que garante que gerentes de unidade acessem apenas os dados de seu próprio estoque, enquanto coordenadores possuem visão macro de toda a rede.
* **Cookie Authentication:** Migramos do armazenamento inseguro de tokens no LocalStorage para Cookies protegidos pelo servidor (`HttpOnly` e `Secure`), mitigando ataques de XSS.

### 📦 Lógica de Inventário por Grade (O Desafio do Varejo)

O sistema resolve o problema clássico da gestão de vestuário e calçados através de um modelo de **SKU Granular**:

* Um produto "Pai" (ex: Tênis Esportivo) possui múltiplas variações "Filhas" vinculadas por **Grade (Tamanho e Cor)**.
* O controle de estoque é feito individualmente por SKU, permitindo rastreabilidade total e evitando erros de inventário físico.

---

## 🏗️ Arquitetura Híbrida Modernizada

O projeto utiliza uma stack equilibrada para garantir SEO, segurança e interatividade:

| Camada | Responsabilidade | Tecnologia |
| --- | --- | --- |
| **Cérebro (Back-end)** | Gestão de Estado, Autorização e Processamento Fiscal | **ASP.NET Core 10 (Razor Pages)** |
| **Executor (Front-end)** | Interatividade em tempo real, Telemetria e UI SPA-like | **Vanilla JavaScript (Modules)** |
| **Persistência** | Relações complexas e integridade referencial | **PostgreSQL + EF Core** |

---

## 🎯 Funcionalidades de Destaque

* **Telemetria de Vendas:** Monitoramento em tempo real do desempenho de cada unidade.
* **Sales Coach Section:** Módulo de assistência que sugere ações baseadas no estado atual do estoque e das metas.
* **Session Intelligence:** Temporizador de sessão sincronizado com o servidor que evita expirações inesperadas e mantém o feedback visual ao usuário.
* **Filtro Zero-Latency:** Navegação rápida entre lojas e produtos com atualizações parciais via API (`PATCH`).

---

## 📁 Estrutura do Projeto

A organização segue padrões de separação de responsabilidades para facilitar a manutenção:

```
📦 RetailPro
├── 📁 Controllers       # APIs para interações assíncronas do JS
├── 📁 Security          # Lógica de autorização (SameStoreRequirement)
├── 📁 Service           # Camada de abstração e regras de negócio
├── 📁 Models            # Entidades de domínio (Enums, DTOs e SKUs)
├── 📁 Pages             # Interfaces Razor com pré-renderização
└── 📁 wwwroot/js        # Módulos JS organizados por funcionalidade

```

---

## 🐧 Otimização para Linux

O RetailPro foi desenvolvido e validado em ambientes **AlmaLinux e Arch Linux**, garantindo que o sistema seja:

* Leve o suficiente para rodar em hardware de balcão (Mini-PCs Industriais).
* Facilmente conteinerizado via Docker para deploys rápidos em servidores locais ou nuvem.

---

## 🚀 Como Executar

1. **Pré-requisitos:** .NET 10 SDK e PostgreSQL instalado.
2. **Configuração:** Ajuste a `ConnectionString` no arquivo `appsettings.json`.
3. **Banco de Dados:** Execute `dotnet ef database update` para aplicar as migrações e o `DbSeeder` (população inicial de dados).
4. **Rodar:** Execute `dotnet run` e acesse o sistema através do navegador.

---

**Desenvolvido com foco em resiliência lógica e eficiência operacional.**