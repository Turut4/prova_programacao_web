# Prova Sistema - Gestão de Clientes

Este é um sistema de gerenciamento administrativo moderno desenvolvido em **ASP.NET Core MVC** como projeto acadêmico. A aplicação permite realizar o cadastro completo de clientes, controle de acesso seguro, persistência de dados local em formato JSON e preenchimento dinâmico de dados geográficos.

---

## 🚀 Funcionalidades Principais

- **CRUD Completo de Clientes:** Cadastro, edição, listagem e exclusão de perfis.
- **Autenticação Segura:** Controle de sessões baseado em Cookies do ASP.NET com criptografia forte de senhas via `PasswordHasher`.
- **Privacidade & Segurança:** Restrição de acesso em nível de controlador onde cada cliente comum só tem permissão de visualizar, editar e excluir seu **próprio perfil**.
- **Validação de Unicidade:** Bloqueio inteligente contra cadastros duplicados de **CPF/CNPJ** (`CodigoFiscal`) ou **Inscrição Estadual** (`InscricaoEstadual`).
- **Localização Dinâmica:** Seletores interligados de Estado e Cidade carregados dinamicamente via Fetch API a partir do arquivo nacional de dados [municipios.json](wwwroot/dados/municipios.json).
- **Exportação de Dados:** Botão dedicado para exportar o cadastro atual de um cliente em formato `.json` limpo e estruturado diretamente pelo navegador.
- **Layout Responsivo:** Menu de navegação reativo com login/logout dinâmico e rodapé flutuante estruturado com Flexbox (Sticky Footer).

---

## 🛠️ Como Executar o Projeto

1. Certifique-se de ter o **.NET SDK** instalado na sua máquina.
2. Navegue até a pasta raiz do projeto pelo terminal:
   ```bash
   cd Prova
   ```
3. Restaure as dependências do projeto:
   ```bash
   dotnet restore
   ```
4. Compile e execute a aplicação em modo de desenvolvimento:
   ```bash
   dotnet watch
   ```
5. Abra o navegador no endereço indicado (geralmente [http://localhost:5082](http://localhost:5082)).
   - Acesso Admin Padrão: Usuário **Admin** / Senha **123**.
