USE Projeto;
GO
 
-- Remover tabelas se existirem
DROP TABLE IF EXISTS Venda;
DROP TABLE IF EXISTS Vendedor;

CREATE TABLE Vendedor (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Vendedor VARCHAR(100) UNIQUE NOT NULL,  -- Adicionando UNIQUE para garantir que não haja vendedores duplicados
    CPF VARCHAR(14),
    RG VARCHAR(20),
    Gmail VARCHAR(100),
    Telefone VARCHAR(20)
);

CREATE TABLE Venda (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IdAutomovel INT NOT NULL,
    Vendedor VARCHAR(100) NOT NULL,  -- Alterado para o mesmo tamanho do Vendedor na tabela Vendedor
    Marca VARCHAR(50) NOT NULL,
    Modelo VARCHAR(50) NOT NULL,
    DataVenda DATE NOT NULL,
    Valor DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Venda_Automovel 
        FOREIGN KEY (IdAutomovel) 
        REFERENCES Automovel(Id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_Venda_Vendedor 
        FOREIGN KEY (Vendedor) 
        REFERENCES Vendedor(Vendedor)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
