# Segurança – Keycloak

A autenticação e autorização do sistema foram implementadas utilizando **Keycloak self-hosted**, garantindo independência de cloud provider e controle total da configuração de identidade.

---

# Objetivo da Escolha

Foi adotado Keycloak self-hosted para:

- Evitar dependência de serviços gerenciados como:
  - Azure Entra ID
  - AWS Cognito
- Manter a arquitetura agnóstica de cloud
- Permitir portabilidade total da solução
- Garantir ambiente 100% reproduzível via Docker

---

# Subindo o Keycloak

O Keycloak é iniciado automaticamente via Docker Compose.

A partir da raiz do projeto:

```bash
docker compose up -d
```

Após subir o container, acessar:

```
http://localhost:8080
```

Credenciais administrativas (definidas no docker-compose):

```
Usuário: admin
Senha: admin
```

---

# Realm Importado Automaticamente

Ao subir o container, o Keycloak importa automaticamente o arquivo:

```
docker/keycloak/realm-export.json
```

O seguinte realm é criado:

```
fluxocaixa
```

---

# Client Configurado

Client criado automaticamente:

```
fluxocaixa-api
```

Configuração:

- Access Type: Confidential
- Standard Flow Enabled
- Direct Access Grants Enabled
- JWT como formato de token

---

# Usuários Criados para Teste

## Usuário Autorizado

```
Username: comerciante01
Senha: teste@123
Role: comerciante
```

Permissões:

- Pode acessar endpoints protegidos
- Pode criar lançamentos
- Pode consultar consolidado

Este é o usuário válido para testar o sistema.

---

## Usuário Sem Permissão

```
Username: cliente01
Senha: teste@123
Role: cliente
```

Permissões:

- Não possui acesso aos endpoints protegidos
- Criado apenas para validação de autorização
- Deve receber **403 Forbidden** ao tentar acessar APIs

---

# Como Obter Access Token

Para testar as APIs via Swagger ou Postman:

## Endpoint de Token

```
http://localhost:8080/realms/fluxocaixa/protocol/openid-connect/token
```

### Método

```
POST
```

### Headers

```
Content-Type: application/x-www-form-urlencoded
```

### Body (x-www-form-urlencoded)

```
grant_type=password
client_id=fluxocaixa-api
username=comerciante01
password=teste@123
```

A resposta conterá:

```
access_token
```

Exemplo execução POSTMAN:
curl --location --request POST 'http://localhost:8081/realms/fluxocaixa/protocol/openid-connect/token' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--data-urlencode 'client_id=fluxocaixa-api' \
--data-urlencode 'client_secret=8pLZ5tO3IOshGOS7clK1sQnB6qqYypkV' \
--data-urlencode 'username=comerciante01' \
--data-urlencode 'password=teste@123' \
--data-urlencode 'grant_type=password'
---

# Como Usar Token no Swagger

1. Acesse o Swagger da API desejada.
2. Clique em **Authorize**.
3. Cole o token no formato:

```
Bearer {access_token}
```

4. Confirme.

Agora os endpoints protegidos poderão ser executados.

---

# Testando Controle de Acesso

Para validar autorização corretamente:

1. Gere token para o usuário `cliente01`.
2. Tente acessar qualquer endpoint protegido.
3. A resposta deve ser:

```
403 Forbidden
```

Isso valida corretamente a aplicação de roles.

---

# Arquitetura de Segurança

## Autenticação

- JWT (Bearer Token)
- OpenID Connect
- OAuth2 Password Flow (utilizado apenas para testes)

## Autorização

- Role-based access control (RBAC)
- Validação de roles via Policy nas APIs
- Controle por role `comerciante`

---

# Independência de Cloud

A solução foi projetada para não depender de serviços gerenciados de identidade.

Caso necessário, poderia ser substituído por:

- Azure Entra ID
- AWS Cognito
- Auth0

Sem impacto no domínio ou na arquitetura principal.

---

# Recriar Realm Após Alterações

Se o realm for alterado:

1. Exportar novamente o realm.
2. Substituir o arquivo:

```
docker/keycloak/realm-export.json
```

3. Recriar o container:

```bash
docker compose down
docker compose up -d
```

---

# Benefícios da Abordagem

- Ambiente 100% reproduzível
- Controle total do realm
- Configuração versionada no repositório
- Segurança desacoplada do domínio
- Preparado para ambientes de produção

---

# Conclusão

A implementação de segurança via Keycloak:

- Garante autenticação centralizada
- Implementa autorização baseada em roles
- Mantém independência de cloud
- Permite fácil migração futura
- Demonstra maturidade arquitetural na solução