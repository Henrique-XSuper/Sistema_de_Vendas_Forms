USE Projeto;
GO

-- Remover tabelas se existirem
DROP TABLE IF EXISTS Venda;
DROP TABLE IF EXISTS Automovel;

-- Criar tabela Automovel
CREATE TABLE Automovel (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Modelo VARCHAR(100),
    Marca VARCHAR(100),
    Valor DECIMAL(18,2),
    Ano INT
);

-- Criar tabela Venda com FOREIGN KEY explícita
CREATE TABLE Venda (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IdAutomovel INT NOT NULL,
    Marca VARCHAR(50) NOT NULL,
    Modelo VARCHAR(50) NOT NULL,
    DataVenda DATE NOT NULL,
    Valor DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Venda_Automovel 
        FOREIGN KEY (IdAutomovel) 
        REFERENCES Automovel(Id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

-- Habilitar verificação da FOREIGN KEY
ALTER TABLE Venda WITH CHECK CHECK CONSTRAINT FK_Venda_Automovel;

-- Inserir alguns automóveis de exemplo
INSERT INTO Automovel (Modelo, Marca, Valor, Ano) VALUES
('Corolla', 'Toyota', 85000.00, 2023),
('Civic', 'Honda', 90000.00, 2023),
('Onix', 'Chevrolet', 60000.00, 2023);

-- Verificar se as tabelas foram criadas corretamente
SELECT * FROM Automovel;
SELECT * FROM Venda;