
USE projeto;

-- Tabela de Clientes
CREATE TABLE cliente (
    id_cliente INT PRIMARY KEY IDENTITY(1,1),
    nome VARCHAR(100) NOT NULL,
    cpf VARCHAR(14) UNIQUE NOT NULL,
    telefone VARCHAR(20),
    email VARCHAR(100)
);

-- Tabela de Vendedores
CREATE TABLE vendedor (
    id_vendedor INT PRIMARY KEY IDENTITY(1,1),
    nome VARCHAR(100) NOT NULL,
    telefone VARCHAR(20),
    email VARCHAR(100)
);

-- Tabela de Imóveis
CREATE TABLE imovel (
    id_imovel INT PRIMARY KEY IDENTITY(1,1),
    endereco VARCHAR(200) NOT NULL,
    tipo VARCHAR(50), 
    valor DECIMAL(12,2),
    disponivel BIT DEFAULT 1
);

-- Tabela de Automóveis
CREATE TABLE automovel (
    id_automovel INT PRIMARY KEY IDENTITY(1,1),
    modelo VARCHAR(100),
    marca VARCHAR(50),
	tipo VARCHAR(50),
    ano INT,
    valor DECIMAL(12,2),
    disponivel BIT DEFAULT 1
);

-- Tabela de Fornecedores
CREATE TABLE fornecedor (
    id_fornecedor INT PRIMARY KEY IDENTITY(1,1),
    nome_empresa VARCHAR(100),
    cnpj VARCHAR(18) UNIQUE,
    telefone VARCHAR(20),
    email VARCHAR(100)
);

-- Tabela de Vendas
CREATE TABLE venda (
    id_venda INT PRIMARY KEY IDENTITY(1,1),
    id_cliente INT,
    id_vendedor INT,
    id_imovel INT,
    id_automovel INT,
    data_venda DATE,
    valor_total DECIMAL(12,2),
    FOREIGN KEY (id_cliente) REFERENCES cliente(id_cliente),
    FOREIGN KEY (id_vendedor) REFERENCES vendedor(id_vendedor),
    FOREIGN KEY (id_imovel) REFERENCES imovel(id_imovel),
    FOREIGN KEY (id_automovel) REFERENCES automovel(id_automovel)
);
