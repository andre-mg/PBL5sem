# 📌 Visão Geral

Este projeto consiste em um sistema completo para monitoramento de estufas inteligentes, integrando:

- Dispositivos **ESP32** com sensores **DHT22**
- **Broker MQTT** para comunicação IoT
- **Interface web** para visualização de dados em tempo real
- **Banco de dados** para armazenamento histórico

---

# 🛠️ Tecnologias Utilizadas

- **Backend**: ASP.NET Core MVC
- **Frontend**: HTML5, CSS3, JavaScript, Plotly.js
- **Banco de Dados**: SQL Server
- **IoT**: ESP32, DHT22, Protocolo MQTT
- **Cloud**: AWS (para broker MQTT)

---

# 🔌 Componentes do Sistema

## 1. Firmware ESP32 (`wokwi.txt`)

- Leitura de temperatura/umidade com **DHT22**
- Conexão **Wi-Fi**
- Publicação de dados via **MQTT**
- Recebimento de comandos para acionamento de **LED**

```cpp
// Configurações principais:
const int DHT_PIN = 16;       // Pino do sensor DHT22
const char* BROKER_MQTT = "35.169.152.96"; // Broker AWS
const int PORTA_MQTT = 1883;
```

---

## 2. Backend ASP.NET Core

**Controllers:**

- `EstufaController`: Gerencia estufas e dados de monitoramento
- `MonitoramentoController`: Processa dados dos sensores

**Models:**

- `EstufaViewModel`
- `MonitoramentoViewModel`
- Outros modelos auxiliares

**DAOs:**

- Responsáveis pelo acesso ao banco de dados

---

## 3. Frontend

- Dashboard interativo com **gráficos**
- Visualização de **histórico**
- Configuração de **alertas**

![Dashboard](https://github.com/user-attachments/assets/4122e33d-15f1-48a1-8669-6ce8a06a61fd)

---

# 🗄️ Estrutura do Banco de Dados

**Principais tabelas:**

- `Estufa`: Cadastro de estufas monitoradas
- `MonitoramentoEstufa`: Armazena leituras de temperatura
- `EspDevice`: Dispositivos IoT conectados
- `Usuario`: Sistema de autenticação

```sql
CREATE TABLE MonitoramentoEstufa (
    Id INT PRIMARY KEY,
    EstufaId INT NOT NULL,
    DataHora DATETIME NOT NULL DEFAULT GETDATE(),
    Temperatura DECIMAL(5,2) NOT NULL
);
```

---

# 🚀 Como Executar

## Pré-requisitos

- .NET 6 SDK
- SQL Server
- Node.js (para assets frontend)

## Passos

1. Restaure o banco de dados executando `database.sql`
2. Configure a **connection string** em `appsettings.json`
3. Execute a aplicação:

```bash
dotnet run
```

4. Acesse: [http://localhost:5000](http://localhost:5000)

---

# 🔄 Fluxo de Dados

1. **ESP32** coleta dados do sensor **DHT22**
2. Publica no **broker MQTT** (AWS)
3. Backend consome dados do broker
4. Dados são armazenados no **SQL Server**
5. Frontend exibe dados em tempo real via **AJAX**

---

# 🌡️ Monitoramento em Tempo Real

O sistema oferece:

- Gráficos interativos de **temperatura**
- Histórico configurável
- Alertas visuais para temperaturas fora da faixa ideal
- Suporte a múltiplos **fusos horários**

---

# 📝 Licença

Este projeto está licenciado sob a **licença MIT** - veja o arquivo [LICENSE](LICENSE) para detalhes.
