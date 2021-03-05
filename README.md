<h1 align="center">Sistema Gerenciador de Caixas Eletrônicos</h1>


#### Descrição do Projeto
<p align="center">Aplicação para visualizar o status dos caixas eletrônicos em tempo real.</p>

### Status

- [x] Tela de status dos caixas
- [x] Tela de saque
- [ ] Testes
=================
<!--ts-->
   * [Como rodar](#como-rodar)
      * [Pre Requisitos](#pre-requisitos)
      * [Preparando o banco](#prepara-banco)
      * [Back-End](#back-end)
      * [Front-End](#fron-end)
   * [Tests](#testes)
   * [Tecnologias](#tecnologias)
<!--te-->

### Pré-requisitos

Para iniciar, instalado em sua máquina as seguintes ferramentas:
[VisualStudio](https://visualstudio.microsoft.com/pt-br/downloads/), [VisualCode](https://code.visualstudio.com/), [Angular Cli](https://github.com/angular/angular-cli),
[SqlServer Express](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads), [Sql Management Studio](https://docs.microsoft.com/pt-br/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15), [Postman](https://www.postman.com/downloads/),
[NodeJs](https://nodejs.org/en/download/). 


## Preparando o banco
Esta aplicação foi feita considerando um banco pré-existente (database first). :
```bash
# Crie um banco chamado Sgca utilizando o SqlManagement Studio:
-> Este nome deve ser o mesmo configurado no arquivo appsettings.json e no arquivo Controllers/CaixaController.cs

# Execute os scripts de Create Tables que estão na pasta db/create_tables.sql

# Para popular o banco com dados iniciais, rode o script db/inserts.sql
```
### Rodando o Back End (api)

```bash
# Clone este repositório
$ git clone <https://github.com/andre-washington/SGCAv3.git>

# Pasta do projeto:
$ cd SGCAv3

# Abra a solution com o visual studio. Se no debug estiver selecionado IISExpress, selecione a aplicação (SGCAv1) e aperte f5 para startar a aplicação.
A aplicação irá startar e deverá ser exibida uma página web com a url: http://localhost:5000/api/caixa/getstatus exibindo as informações dos caixas.
```
## Testando a API com o Postman
Na pasta postman_collections está o arquivo das requisições que deve ser importado no Postman. Nesta colection existem alguns testes que podem ser feitos na api, 
como GET, POST e PUT de caixa e notas.

### Rodando o Front End (cliente)
O projeto Fron-end encontra-se na pasta UI\sgceui
```bash
$ cd UI\sgceui
$ ng serve --open
```
A aplicação cliente roda na porta 4200 e será exibida no browser (http://localhost:4200).

### Testando a aplicação
Na aplicação cliente, temos dois botões Caixa e Saque.
O botão Caixa exibe uma tabela que é atualizada com o status dos caixas eletrônicos cadastrados.
O botão Saque abre em outra guia uma interface para efetuar saques nos caixas marcados como "ATIVO".


### Tecnologias
O projeto back-end é uma api feita em AspNet Core 5.0.x, e algumas bibliotecas são necessárias: Entity Framework 6.4.4, NewtonsoftJson 5.03, SqlClient 4.8.2,.

O projeto front-end foi feito em Angular 10 + componentes Material UI.

