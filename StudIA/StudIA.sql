CREATE DATABASE StudIA;
USE StudIA;

CREATE TABLE Usuario (
    IdUsuario INT(10) PRIMARY KEY NOT NULL AUTO_INCREMENT,
    NomeUsuario VARCHAR(50) NOT NULL,
    DataCriacao DATETIME NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Senha VARCHAR(25) NOT NULL,
    Pontuacao INT(20) DEFAULT 0,
    FotoDePerfil LONGBLOB
);

CREATE TABLE Chat (
    MensagemId INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
    IdUsuario INT NOT NULL,
    Conteudo TEXT(65535) NOT NULL,
    Remetente NVARCHAR(20) NOT NULL,
    DataEnvio DATETIME NOT NULL,
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);

CREATE VIEW relatorioUsuario AS SELECT IdUsuario, NomeUsuario, DataCriacao FROM Usuario;
INSERT INTO Usuario (NomeUsuario, DataCriacao, Email, Senha) VALUES ('NomeTeste', '2023-01-01', 'teste@teste.com', 'senha123');


SELECT * FROM Usuario;
SELECT * FROM relatorioUsuario;
SELECT NomeUsuario FROM Usuario ORDER BY NomeUsuario;
