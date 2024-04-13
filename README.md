# Motorcycle Rental

## Introdução

Esta solução gerencia operações relacionadas a usuários, aluguéis de motocicletas e pedidos. Projetada para sistemas que necessitam de gerenciamento eficiente de entregas e aluguéis, este conjunto de ferramentas facilita o controle e a administração através de interfaces de código limpas e métodos bem definidos.

## Pré-requisitos

Antes de iniciar, certifique-se de que as seguintes tecnologias estejam instalados/configurados:

- .NET 8.0
- MongoDB
- Azure Service Bus
- Azure Blob Storage

## Configuração

### Configuração do Ambiente

Para configurar o ambiente necessário para rodar a API, execute os seguintes comandos:

```bash
dotnet restore
dotnet build
```

### Variáveis de Ambiente

Configure as variáveis seguintes variáveis no seu arquivo `secrets.json` para a correta inicialização e operação da API:

```plaintext
{
  "DatabaseOptions": {
    "ConnectionString": "[mongo db connection string]"
  },
  "JwtAuthenticationOptions": {
    "Key": "[some 256bit string here]"
  },
  "StoragingOptions": {
    "ConnectionString": "azure blob storage connection string",
    "ContainerName": ""
  },
  "PublisherOptions": {
    "ConnectionString": "azure service bus connection string"
  }
}
```

## Execução dos Testes

Execute os testes unitários e de integração com o seguinte comando:

```bash
dotnet test
```

# <br>Endpoints de Usuários

## Descrição Geral
Esta seção documenta os endpoints relacionados ao gerenciamento de usuários, incluindo administradores e parceiros de entrega, bem como processos de autenticação.

## Endpoints

### Criar Administrador
- **Endpoint**: `POST /api/users/create-admin`
- **Tags**: `User`
- **Autorização Necessária**: `Administrator`
- **Descrição**: Cria um administrador no sistema.
- **Payload de Entrada**:
  ```json
  {
    "Email": "admin@example.com",
    "Password": "senhaSegura123"
  }
  ```
- **Resposta**: `204 No Content` - Sucesso, sem retorno de conteúdo.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `500 Internal Server Error` - Erro interno no servidor.

### Criar Parceiro de Entrega
- **Endpoint**: `POST /api/users/create-delivery-partner`
- **Tags**: `User`
- **Descrição**: Registra um novo parceiro de entrega no sistema.
- **Payload de Entrada**:
  ```json
  {
    "Email": "partner@example.com",
    "Password": "senhaSegura123",
    "FullName": "Nome Completo",
    "NationalId": "12345678900",
    "BirthDate": "1990-01-01"
  }
  ```
- **Resposta**: `204 No Content` - Sucesso, sem retorno de conteúdo.
- **Erros Possíveis**:
  - `500 Internal Server Error` - Erro interno no servidor.

### Autenticar
- **Endpoint**: `POST /api/users/authenticate`
- **Tags**: `User`
- **Descrição**: Autentica um usuário e retorna um token JWT, se bem-sucedido.
- **Payload de Entrada**:
  ```json
  {
    "Email": "user@example.com",
    "Password": "user123"
  }
  ```
- **Resposta**: `200 OK` - Retorna o token JWT.
  ```json
  {
    "Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "Expires": "2023-01-01T00:00:00Z"
  }
  ```
- **Erros Possíveis**:
  - `401 Unauthorized` - Credenciais inválidas.
  - `500 Internal Server Error` - Erro interno no servidor.


# <br>Endpoints de Motocicletas

## Descrição Geral
Esta seção documenta os endpoints relacionados ao gerenciamento de motocicletas, incluindo criação, listagem, atualização e exclusão de motocicletas no sistema.

## Endpoints

### Criar Motocicleta
- **Endpoint**: `POST /api/motorcycles/create`
- **Tags**: `Motorcycle`
- **Autorização Necessária**: `Administrator`
- **Descrição**: Registra uma nova motocicleta no sistema.
- **Payload de Entrada**:
  ```json
  {
    "Year": 2021,
    "Model": "Model X",
    "Plate": "XYZ1234"
  }
  ```
- **Resposta**: `200 OK` - Retorna o ID da motocicleta criada.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `500 Internal Server Error` - Erro interno no servidor.

### Listar Motocicletas
- **Endpoint**: `GET /api/motorcycles/list`
- **Tags**: `Motorcycle`
- **Autorização Necessária**: `Administrator`
- **Descrição**: Lista todas as motocicletas com base em critérios de pesquisa.
- **Query Parameters**: Ano, Modelo, Placa, Status
- **Resposta**: `200 OK` - Retorna uma lista de motocicletas.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `500 Internal Server Error` - Erro interno no servidor.

### Atualizar Placa da Motocicleta
- **Endpoint**: `PATCH /api/motorcycles/update-plate`
- **Tags**: `Motorcycle`
- **Autorização Necessária**: `Administrator`
- **Descrição**: Atualiza a placa de uma motocicleta existente.
- **Payload de Entrada**:
  ```json
  {
    "OldPlate": "XYZ1234",
    "NewPlate": "XYZ5678"
  }
  ```
- **Resposta**: `204 No Content` - Sucesso, sem retorno de conteúdo.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `404 Not Found` - Motocicleta não encontrada.
  - `500 Internal Server Error` - Erro interno no servidor.

### Atualizar Status da Motocicleta
- **Endpoint**: `PATCH /api/motorcycles/update-status`
- **Tags**: `Motorcycle`
- **Autorização Necessária**: `Administrator`
- **Descrição**: Atualiza o status de uma motocicleta.
- **Payload de Entrada**:
  ```json
  {
    "Plate": "XYZ1234",
    "Status": "Disponível"
  }
  ```
- **Resposta**: `204 No Content` - Sucesso, sem retorno de conteúdo.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `404 Not Found` - Motocicleta não encontrada.
  - `500 Internal Server Error` - Erro interno no servidor.

### Excluir Motocicleta
- **Endpoint**: `DELETE /api/motorcycles/delete`
- **Tags**: `Motorcycle`
- **Autorização Necessária**: `Administrator`
- **Descrição**: Exclui uma motocicleta do sistema pelo número da placa.
- **Parameter**: Placa
- **Resposta**: `204 No Content` - Sucesso, sem retorno de conteúdo.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `404 Not Found` - Motocicleta não encontrada.
  - `500 Internal Server Error` - Erro interno no servidor.

# <br>Endpoints de Carteira de Habilitação

## Descrição Geral
Esta seção documenta os endpoints relacionados ao gerenciamento de licenças de condução para parceiros de entrega, incluindo criação e atualização de licenças.

## Endpoints

### Criar Carteira de Habilitação
- **Endpoint**: `POST /api/driver-license/create`
- **Tags**: `DriverLicense`
- **Autorização Necessária**: `DeliveryPartner`
- **Descrição**: Cadastra uma nova carteira de Habilitação para um parceiro de entrega.
- **Tipo de Dado Aceito**: `multipart/form-data`
- **Payload de Entrada**:
  ```plaintext
  - 'DriverLicenseId' (text)
  - 'DriverLicenseType' (text)
  - 'DriverLicenseImage' (file)
  ```
- **Resposta**: `204 No Content` - Confirma que a licença foi criada sem retornar nenhum conteúdo.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `500 Internal Server Error` - Erro interno no servidor.

### Atualizar Carteira de Habilitação
- **Endpoint**: `PATCH /api/driver-license/update`
- **Tags**: `DriverLicense`
- **Autorização Necessária**: `DeliveryPartner`
- **Descrição**: Atualiza uma carteira de Habilitação existente para um parceiro de entrega.
- **Tipo de Dado Aceito**: `multipart/form-data`
- **Payload de Entrada**:
  ```plaintext
  - 'DriverLicenseId' (text)
  - 'NewDriverLicenseType' (text)
  - 'NewDriverLicenseImage' (file, optional)
  ```
- **Resposta**: `204 No Content` - Sucesso, sem retorno de conteúdo.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `404 Not Found` - carteira de Habilitação não encontrada.
  - `500 Internal Server Error` - Erro interno no servidor.

# <br>Endpoints de Aluguel de Motocicletas

## Descrição Geral
Esta seção documenta os endpoints relacionados ao aluguel de motocicletas, incluindo operações para alugar motocicletas, verificar preços de aluguel e retornar motocicletas alugadas.

## Endpoints

### Alugar Motocicleta
- **Endpoint**: `POST /api/rentals/rent`
- **Tags**: `Rental`
- **Autorização Necessária**: `DeliveryPartner`
- **Descrição**: Registra um novo aluguel de motocicleta.
- **Payload de Entrada**:
  ```json
  {
    "MotorcycleId": "abc123",
    "StartDate": "2023-01-01",
    "EndDate": "2023-01-02"
  }
  ```
- **Resposta**: `200 OK` - Retorna o custo do aluguel.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `500 Internal Server Error` - Erro interno no servidor.

### Verificar Preço de Aluguel
- **Endpoint**: `POST /api/rentals/peek-price`
- **Tags**: `Rental`
- **Descrição**: Calcula o preço de um potencial aluguel sem criar um registro.
- **Payload de Entrada**:
  ```json
  {
    "MotorcycleId": "abc123",
    "StartDate": "2023-01-01",
    "EndDate": "2023-01-02"
  }
  ```
- **Resposta**: `200 OK` - Retorna o preço calculado do aluguel.
- **Erros Possíveis**:
  - `500 Internal Server Error` - Erro interno no servidor.

### Retornar Motocicleta Alugada
- **Endpoint**: `PATCH /api/rentals/return`
- **Tags**: `Rental`
- **Autorização Necessária**: `DeliveryPartner`
- **Descrição**: Finaliza o aluguel de uma motocicleta pelo parceiro de entrega.
- **Resposta**: `204 No Content` - Sucesso, sem retorno de conteúdo.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `404 Not Found` - Aluguel não encontrado.
  - `500 Internal Server Error` - Erro interno no servidor.

# <br>Endpoints de Pedidos

## Descrição Geral
Esta seção documenta os endpoints relacionados ao gerenciamento de pedidos, incluindo a criação de pedidos, consulta de parceiros notificados, verificação de disponibilidade de pedidos e atualização do status de pedidos.

## Endpoints

### Criar Pedido
- **Endpoint**: `POST /api/orders/create`
- **Tags**: `Order`
- **Autorização Necessária**: `Administrator`
- **Descrição**: Cria um novo pedido no sistema.
- **Payload de Entrada**:
  ```json
  {
    "DeliveryCost": 150.00,
    "Status": "Pending"
  }
  ```
- **Resposta**: `200 OK` - Retorna os detalhes do pedido criado.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `500 Internal Server Error` - Erro interno no servidor.

### Obter Parceiros Notificados
- **Endpoint**: `GET /api/orders/notified-partners`
- **Tags**: `Order`
- **Autorização Necessária**: `Administrator`
- **Descrição**: Retorna uma lista de parceiros de entrega que foram notificados sobre um pedido específico.
- **Query Parameters**: `publicOrderId`
- **Resposta**: `200 OK` - Retorna uma lista de parceiros notificados.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `500 Internal Server Error` - Erro interno no servidor.

### Verificar Disponibilidade do Pedido
- **Endpoint**: `GET /api/orders/check-availability`
- **Tags**: `Order`
- **Autorização Necessária**: `DeliveryPartner`
- **Descrição**: Verifica se um pedido específico está disponível para entrega.
- **Query Parameters**: `publicOrderId`
- **Resposta**: `204 No Content` - Confirma que o pedido está disponível para entrega.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `404 Not Found` - Pedido não encontrado.
  - `500 Internal Server Error` - Erro interno no servidor.

### Atualizar Status do Pedido
- **Endpoint**: `PATCH /api/orders/update-status`
- **Tags**: `Order`
- **Autorização Necessária**: `DeliveryPartner`
- **Descrição**: Atualiza o status de um pedido existente.
- **Payload de Entrada**:
  ```json
  {
    "PublicOrderId": "public_order_id_here",
    "Status": "Delivered"
  }
  ```
- **Resposta**: `204 No Content` - Sucesso, sem retorno de conteúdo.
- **Erros Possíveis**:
  - `403 Forbidden` - Usuário não autorizado.
  - `404 Not Found` - Pedido não encontrado.
  - `500 Internal Server Error` - Erro interno no servidor.

# <br>Consumer de Notificação de Pedidos

## Descrição Geral
Este serviço é responsável por monitorar e processar mensagens de novos pedidos para notificar parceiros de entrega disponíveis.

## Funcionalidades

### Execução do Serviço
O `OrderNotificationWorker` é um serviço de background que:
- Escuta por mensagens de novos pedidos na fila especificada.
- Deserializa e processa essas mensagens para determinar se o pedido pode ser notificado a parceiros.
- Notifica todos os parceiros de entrega disponíveis sobre os pedidos qualificados.

### Tratamento de Mensagens
- **Deserialização**: Converte o corpo da mensagem de volta para o objeto `Order`.
- **Validação**: Verifica se o pedido recebido pode ser notificado a parceiros.
- **Notificação**: Para cada parceiro disponível, adiciona o pedido às suas notificações.

### Logs e Erros
- Erros durante o processo de notificação ou atualizações de banco de dados são registrados para diagnóstico.

### Manutenção e Desligamento
- Implementa um controle de cancelamento que responde a sinais de interrupção do sistema para um desligamento não forçado.
- Gerencia a limpeza de recursos como conexões ao Service Bus ao finalizar.

# <br>Bibliotecas de Terceiros Utilizadas

## Descrição Geral
Esta seção lista as principais bibliotecas de terceiros utilizadas no projeto, destacando suas funções principais e como elas contribuem para a funcionalidade da aplicação.

## Bibliotecas

### Testes
- **xUnit**: Utilizado para testes unitários.
- **Moq**: Usado para criar objetos mock para facilitar os testes unitários.

### Serviços de Mensageria
- **Azure.Messaging.ServiceBus**: Usada para integração com o Azure Service Bus, permitindo operações de mensageria.

### Persistência de Dados
- **MongoDB.Bson**: Usada para manipulação de documentos BSON no MongoDB.
- **MongoDB.Driver**: Driver oficial do MongoDB para .NET, utilizado para interagir com o banco de dados MongoDB.

### Log e Monitoramento
- **Serilog**: Biblioteca de logging avançada com suporte a vários sinks e formatos de log.

### Mapeamento de Dados
- **AutoMapper**: Utilizado para mapeamento automático de objetos, simplificando as transformações entre os modelos de dados.

### Validação
- **FluentValidation**: Framework para a configuração de regras de validação de maneira fluente e clara.

### Autenticação e Segurança
- **Microsoft.AspNetCore.Authentication.JwtBearer**: Usado para implementar a autenticação com JWT (JSON Web Tokens).
- **BCrypt.Net-Next**: Biblioteca para hashing de senhas usando BCrypt.

### Documentação e API
- **Swashbuckle.AspNetCore**: Ferramentas para gerar documentação Swagger para APIs ASP.NET Core, incluindo a interface do usuário Swagger.
- **Swashbuckle.AspNetCore.Annotations**: Fornece funcionalidades adicionais de anotações para Swashbuckle.

### Utilitários
- **Newtonsoft.Json**: Popular biblioteca para manipulação de JSON em .NET.
- **Azure.Storage.Blobs**: Usada para operações com blobs no Azure Blob Storage.
- **System.IdentityModel.Tokens.Jwt**: Utilizado para trabalhar com JSON Web Tokens em .NET.
- **Microsoft.IO.RecyclableMemoryStream**: Usado para gerenciamento eficiente de memória ao manipular streams.

### Injeção de Dependência e Configuração
- **Scrutor**: Extensão para o Microsoft.Extensions.DependencyInjection que permite a montagem de serviços por convenção.

# <br> Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo LICENSE.md para detalhes.