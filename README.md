# CADASTRO DE ENDEREÇOS POR USUÁRIOS.
Aplicação web em C# que permita ao usuário realizar login e gerenciar um CRUD de endereços, ele poderá inserir o endereço manualmente ou informar um CEP para aplicação buscar os dados do endereço através da integração com a API do ViaCEP(https://viacep.com.br/), além disso ele também poderá exportar os seus endereços salvos para um arquivo CSV.
Foi implementado um Cadastro de Usuário para utilização do Sistema.

## Instruções
- Fazer o Clone do Fonte ou Baixar.
- Abrir o projeto no Visual Studio 2022 (Foi o que utilizei para Implementar) ou se preferir no VSCode (lembrar de instalar as dependencias necessárias para C#).
- Verificar as dependencias e reinstalar as que forem necessárias.
- Alterar o conection string no arquivo Web.config de acordo com as configurações da sua Base de Dados.
- Criar apenas a base de dados vazia no SQL.
- Executar a Migration (no Console de Gerenciador de Pacotes) ou dotnet ef database update no CLI do .NET (Executar na ordem: Enable-Migrations, Add-Migrations InitialCreate e Update-Database).
- Ao executar a Migration se gerar algum erro é devido ao fato de a mesma já ter sido criada, então basta gerar o Update-Database para que as tabelas sejam criadas na Base de Dados.
- Executar a Aplicação.

## 

## Tecnologias, Metodologias e Conhecimentos utilizados
- ASP.NET MVC C#
- EntityFramework 
- SQL Server
