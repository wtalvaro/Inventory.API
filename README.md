# RetailPro - AI Sales Assistant

## 📋 Visão Geral

O **RetailPro** é um sistema inteligente de assistência a vendas desenvolvido para o setor varejista, combinando inteligência artificial com uma interface moderna para otimizar o processo de vendas e aumentar a performance dos vendedores.

## 🚀 Funcionalidades Principais

### 🏪 **Gestão de Catálogo Inteligente**
- Listagem completa de produtos com busca em tempo real
- Grade de tamanhos e variantes organizadas
- Controle de estoque com atualizações instantâneas
- Localização física dos produtos (corredor/prateleira)

### 👨‍🏫 **Coach de Vendas Programável**
- Timeline de vendas personalizável por produto
- Mensagens temporizadas (info/alerta/sucesso)
- Sistema de pontuação por performance
- Timer de atendimento com feedback visual

### 🛒 **Carrinho de Vendas com Cross-Sell**
- Adição de itens com pontuação diferenciada
- Sugestões inteligentes de produtos complementares
- Controle de quantidades em tempo real
- Sincronização automática com o estoque

### 📊 **Sistema de Performance**
- Pontuação baseada em ações do vendedor
- Barra de progresso visual
- Telemetria completa do atendimento
- Relatórios de desempenho

## 🛠️ Arquitetura Técnica

### **Backend (ASP.NET Core 10)**
- **Framework:** ASP.NET Core 10
- **Banco de Dados:** PostgreSQL com JSONB
- **ORM:** Entity Framework Core
- **API:** RESTful com endpoints documentados

### **Frontend**
- **HTML5** com estrutura modular
- **Tailwind CSS** para estilização
- **Lucide Icons** para ícones
- **JavaScript** vanilla para interatividade
- **Design Responsivo** (Mobile First)

### **Modelos de Dados Principais**
```csharp
// Produto com timeline de vendas
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SKU { get; set; }
    public List<SalesStep> SalesTimeline { get; set; }
    // ... outras propriedades
}

// Sessão de vendas com telemetria
public class SalesSession
{
    public int Id { get; set; }
    public int SellerId { get; set; }
    public decimal TotalOrderValue { get; set; }
    public string Status { get; set; }
    // ... outras propriedades
}
```

## 📁 Estrutura do Projeto

```
Inventory.API/
├── Controllers/
│   └── ProductsController.cs    # API de produtos e vendas
├── Models/
│   ├── Product.cs               # Modelo do produto
│   ├── SalesSession.cs          # Sessão de vendas
│   ├── CartItem.cs              # Item do carrinho
│   ├── Seller.cs                # Vendedor
│   └── InventoryLog.cs          # Log de estoque
├── Data/
│   └── InventoryDbContext.cs    # Contexto do banco
└── Frontend/
    └── index.html              # Interface completa
```

## 🚀 Como Executar

### **Pré-requisitos**
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/) 14+
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

### **Passo a Passo**

1. **Clone o repositório**
   ```bash
   git clone https://github.com/seu-usuario/retailpro.git
   cd retailpro
   ```

2. **Configure o banco de dados**
   ```bash
   # Crie o banco PostgreSQL
   createdb InventoryDB
   
   # Atualize a connection string em appsettings.json
   ```

3. **Execute as migrações**
   ```bash
   dotnet ef database update
   ```

4. **Execute a aplicação**
   ```bash
   dotnet run
   ```

5. **Acesse a interface**
   - Abra o navegador em: `http://localhost:5000`
   - Ou execute diretamente o `index.html` do frontend

## 📡 Endpoints da API

### **Produtos**
- `GET /api/products` - Lista todos os produtos
- `GET /api/products/sku/{sku}` - Busca produto por SKU
- `GET /api/products/model/{name}` - Busca variantes do modelo
- `PATCH /api/products/sell/{sku}` - Realiza venda
- `PATCH /api/products/restock/{sku}` - Reabastece estoque
- `PUT /api/products/update-timeline/{sku}` - Atualiza timeline

### **Estatísticas**
- `GET /api/products/stats/top-searched` - Produtos mais buscados
- `GET /api/products/export/inventory` - Exporta inventário CSV

## 🎯 Funcionamento do Sistema

### **1. Início do Atendimento**
- Vendedor seleciona um produto da lista
- Sistema inicia timer de atendimento
- Coach exibe primeira dica de vendas

### **2. Processo de Venda**
- Adição de itens ao carrinho com pontuação
- Sugestões de cross-sell baseadas no produto
- Atualização em tempo real do estoque

### **3. Finalização**
- Sincronização com banco de dados
- Registro da sessão de vendas
- Atualização da pontuação do vendedor

## 🎨 Interface do Usuário

### **Layout de Três Colunas**
1. **Esquerda:** Catálogo de produtos com busca
2. **Centro:** Detalhes do produto + Coach de vendas
3. **Direita:** Cross-sell + Editor de timeline

### **Componentes Principais**
- **Header:** Performance do vendedor e controles
- **Carrinho Lateral:** Slide-in com itens e total
- **Modal Quick View:** Visualização rápida de produtos
- **Toast Notifications:** Feedback visual das ações

## 🔧 Configuração Avançada

### **Personalização do Coach**
```javascript
// Exemplo de timeline programada
const timeline = [
    { second: 5, message: "Destaque o material premium", type: "info" },
    { second: 15, message: "Ofereça o combo com 10% off", type: "alert" },
    { second: 30, message: "Feche a venda com garantia estendida", type: "success" }
];
```

### **Hardware Integration**
- Suporte a LEDs indicadores por SKU
- Configuração de tempo de iluminação
- Logs de acionamento para análise

## 📊 Sistema de Pontuação

| Ação | Pontos |
|------|--------|
| Venda de item normal | 100 pts |
| Venda de cross-sell | 150 pts |
| Reabastecimento | 50 pts |
| Finalização de pedido | 200 pts |

## 🤝 Contribuindo

1. Faça um Fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## ✨ Próximas Funcionalidades

- [ ] Dashboard analítico para gestores
- [ ] Integração com sistema de pagamento
- [ ] App mobile para vendedores
- [ ] Relatórios de conversão por vendedor
- [ ] Sistema de metas e bonificações

## 📞 Suporte

Para suporte, abra uma issue no GitHub ou entre em contato através do email: suporte@retailpro.com

---

**Desenvolvido com ❤️ para revolucionar o varejo brasileiro**

*RetailPro - Transformando vendas através da inteligência*
