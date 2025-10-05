🧾 Descrição do Projeto
--

Este projeto é um sistema desktop desenvolvido em C# com Windows Forms, voltado para o gerenciamento de vendas de -consórcios. 
O sistema permite o cadastro e controle de informações relacionadas a clientes, vendedores, fornecedores, automóveis, imóveis e vendas.

---

🎯 Objetivo
-

Facilitar o processo de venda de consórcios, oferecendo uma interface intuitiva para o registro e consulta de dados, além de melhorar a organização e eficiência da equipe de vendas.

---

🧩 Funcionalidades
-

Cadastro de Cliente: Nome, CPF,RG,CEP, Data de nascimento, Renda Mensal, e-mail,telefone.

Cadastro de Vendedor: Nome, CPF,RG,CEP,Gmail, telefone.

Cadastro de Fornecedor: Nome da empresa, Produto,Tipo de produto, Preço Bruto,Gmail, Telefone, Data de recebimento.

Cadastro de Automóvel: Marca, modelo, ano, valor.

Cadastro de Imóvel: Tipo, localização, valor estimado.

Registro de Venda: Automovel ID,Cliente ID, vendedor,Marca,Modelo, valor, data da venda.

---

🖥️ Tecnologias Utilizadas:
-
C#

.NET Framework

Windows Forms

SQL Server 

---

📦 Estrutura do Projeto
-
/ConsorcioApp <br>
│<br>
├── Forms<br>
│   ├── ClienteForm.cs <br>
│   ├── VendedorForm.cs<br>
│   ├── FornecedorForm.cs<br>
│   ├── AutomovelForm.cs<br>
│   ├── ImovelForm.cs<br>
│   └── VendaForm.cs<br>
│<br>
├── Models<br>
│   ├── Cliente.cs<br>
│   ├── Vendedor.cs<br>
│   ├── Fornecedor.cs<br>
│   ├── Automovel.cs<br>
│   ├── Imovel.cs<br>
│   └── Venda.cs<br>
│<>
├── Program.cs<br>
└── README.md<br>

---

🚀 Como Executar
-
Cole e rode os arquivos de texto do SqlServer.

Abra o projeto no Visual Studio.

Mude o endereçamento do servidor para o seu.

Compile e execute o projeto (F5).

Navegue pelos formulários para cadastrar e gerenciar os dados.

---

📌 Observações
-
O projeto pode ser expandido com funcionalidades como geração de relatórios,autenticação de usuários e exportação de dados.
Ideal para uso em pequenas e médias empresas que trabalham com consórcios de veículos e imóveis.

---
📝Licença 
-
este programa esta sobre a licença BSD 2.
