------------------------------------------------
-------- PROCEDURES GENERICAS
------------------------------------------------

CREATE OR ALTER PROCEDURE spProximoId
	(@tabela VARCHAR(max))
AS
BEGIN
	 EXEC('select isnull(max(id) +1, 1) as MAIOR from '
	 +@tabela)
END

GO

CREATE OR ALTER PROCEDURE spDelete
	(
	 @id int ,
	 @tabela varchar(max)
	)
AS
BEGIN
	 DECLARE @sql varchar(max);
	 SET @sql = ' delete ' + @tabela +
	 ' where id = ' + cast(@id as varchar(max))
	 EXEC(@sql)
END

GO

CREATE OR ALTER PROCEDURE spConsulta
	(
	 @id int,
	 @tabela varchar(max)
	)
AS
BEGIN
	 DECLARE @sql VARCHAR(max);
	 SET @sql = 'select * from ' + @tabela +
	 ' where id = ' + cast(@id as varchar(max))
	 EXEC(@sql)
END
EXEC spConsulta @id = 1, @tabela = 'EspDevice';
GO

CREATE OR ALTER PROCEDURE spListagem
	(
	 @tabela VARCHAR(max),
	 @ordem VARCHAR(max)
	 )
AS
BEGIN
	 EXEC('select * from ' + @tabela +
	 ' order by ' + @ordem)
END


------------------------------------------------
-- Tabela Cargo
------------------------------------------------
CREATE TABLE Cargo (
    Id INT NOT NULL PRIMARY KEY,
    Nome VARCHAR(50) NOT NULL
);

INSERT INTO Cargo (Id, Nome) VALUES 
(1, 'Administrador'),
(2, 'Usuário');

------------------------------------------------
-- Tabela Empresa
------------------------------------------------
CREATE TABLE Empresa (
    Id INT NOT NULL PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Logo VARBINARY(MAX) NULL
);

INSERT INTO EMPRESA (Id, Nome) VALUES
(1, 'FESA')

GO

CREATE OR ALTER PROCEDURE spInsert_Empresa
    @Id INT,
    @Nome VARCHAR(100),
    @Logo VARBINARY(MAX) = NULL
AS
BEGIN
    INSERT INTO Empresa (Id, Nome, Logo)
    VALUES (@Id, @Nome, @Logo)
END

GO

CREATE OR ALTER PROCEDURE spUpdate_Empresa
    @Id INT,
    @Nome VARCHAR(100),
    @Logo VARBINARY(MAX) = NULL
AS
BEGIN
    UPDATE Empresa SET
        Nome = @Nome,
        Logo = CASE WHEN @Logo IS NOT NULL THEN @Logo ELSE Logo END
    WHERE Id = @Id
END

GO
-- Verificar usuários por empresa para confirmação em AJAX
CREATE OR ALTER PROCEDURE spVerificaUsuariosPorEmpresa
    @EmpresaId INT
AS
BEGIN
    SELECT Id, Nome, Email 
    FROM Usuario 
    WHERE EmpresaId = @EmpresaId
END
GO
-- Deletar usuários quando uma empresa é excluída
CREATE OR ALTER PROCEDURE spDeleteEmpresaComUsuarios
    @EmpresaId INT
AS
BEGIN
    DELETE FROM Usuario WHERE EmpresaId = @EmpresaId
	DELETE FROM Empresa WHERE Id = @EmpresaId
END

------------------------------------------------
-- Tabela Usuario
------------------------------------------------
CREATE TABLE Usuario (
    Id INT NOT NULL PRIMARY KEY,
    Email VARCHAR(100) NOT NULL,
    Nome VARCHAR(100) NOT NULL,
    Senha VARCHAR(255) NOT NULL,
    EmpresaId INT NULL,
    CargoId INT NULL
);

INSERT INTO Usuario (Id, Email, Nome, Senha, EmpresaId, CargoId) VALUES
(1, 'andre.m.garcia042@gmail.com','André Mendes','$2a$11$URNZYO2JMEyzGsUoXDRUVOxCp1wSGLP0BCYfI.cc25uW3DYDQDFM6',1,2),
(2, '081230012@faculdade.cefsa.edu.br','André Mendes (Admin)','$2a$11$URNZYO2JMEyzGsUoXDRUVOxCp1wSGLP0BCYfI.cc25uW3DYDQDFM6',1,1)

GO

CREATE OR ALTER PROCEDURE spInsert_Usuario
    @Id INT,
    @Nome VARCHAR(100),
    @Email VARCHAR(100),
    @Senha VARCHAR(255),
    @EmpresaId INT,
    @CargoId INT
AS
BEGIN
    INSERT INTO Usuario (Id, Nome, Email, Senha, EmpresaId, CargoId)
    VALUES (@Id, @Nome, @Email, @Senha, @EmpresaId, @CargoId)
END

GO

CREATE OR ALTER PROCEDURE spUpdate_Usuario
    @Id INT,
    @Nome VARCHAR(100),
    @Email VARCHAR(100),
    @Senha VARCHAR(255),
    @EmpresaId INT,
    @CargoId INT
AS
BEGIN
    UPDATE Usuario SET
        Nome = @Nome,
        Email = @Email,
        Senha = CASE WHEN @Senha IS NOT NULL THEN @Senha ELSE Senha END,
        EmpresaId = @EmpresaId,
        CargoId = @CargoId
    WHERE Id = @Id
END

GO

-- Listagem com Join
CREATE OR ALTER PROCEDURE spListagemUsuarioEmpresa
AS
BEGIN
    SELECT 
        u.Id,
        u.Nome,
        u.Email,
        u.Senha,
        u.EmpresaId,
        u.CargoId,
        e.Nome AS NomeEmpresa,
        e.Logo AS LogoEmpresa,
        c.Nome AS NomeCargo
    FROM Usuario u
    LEFT JOIN Empresa e ON e.Id = u.EmpresaId
    LEFT JOIN Cargo c ON c.Id = u.CargoId
END

GO

-- Consulta para Login
CREATE OR ALTER PROCEDURE spConsultaUsuarioPorEmail
    @Email VARCHAR(100)
AS
BEGIN
    SELECT 
        u.Id,
        u.Nome,
        u.Email,
        u.Senha,
        u.EmpresaId,
        u.CargoId,
        e.Nome AS NomeEmpresa,
        e.Logo AS LogoEmpresa,
        c.Nome AS NomeCargo
    FROM Usuario u
    LEFT JOIN Empresa e ON e.Id = u.EmpresaId
    LEFT JOIN Cargo c ON c.Id = u.CargoId
    WHERE u.Email = @Email
END

------------------------------------------------
-- Tabela EspDevice
------------------------------------------------
CREATE TABLE EspDevice (
    Id INT NOT NULL PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    EnderecoIP VARCHAR(15) NOT NULL,  -- IPv4 padrão (ex: 192.168.1.1)
);

GO

CREATE OR ALTER PROCEDURE spInsert_EspDevice
    @Id INT,
    @Nome VARCHAR(100),
    @EnderecoIP VARCHAR(15)
AS
BEGIN
    INSERT INTO EspDevice (Id, Nome, EnderecoIP)
    VALUES (@Id, @Nome, @EnderecoIP)
END

GO
CREATE OR ALTER PROCEDURE spUpdate_EspDevice
    @Id INT,
    @Nome VARCHAR(100),
    @EnderecoIP VARCHAR(15)
AS
BEGIN
    UPDATE EspDevice SET
        Nome = @Nome,
        EnderecoIP = @EnderecoIP
    WHERE Id = @Id
END

GO
-- Listar estufa por ESPs para confirmação em AJAX
CREATE OR ALTER PROCEDURE spVerificaEstufasPorEsp
    @EspId INT
AS
BEGIN
    SELECT Id, Nome 
    FROM Estufa 
    WHERE EspId = @EspId;
END

------------------------------------------------
-- Tabela Estufa
------------------------------------------------
CREATE TABLE Estufa (
    Id INT NOT NULL PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    TemperaturaIdeal DECIMAL(5,2) NOT NULL,
	AwsIP VARCHAR(15) NOT NULL,
	EspId INT NOT NULL
);

GO

CREATE OR ALTER PROCEDURE spInsert_Estufa
    @Id INT,
    @Nome VARCHAR(100),
    @TemperaturaIdeal DECIMAL(5,2),
	@AwsIP VARCHAR(15),
	@EspId INT
AS
BEGIN
    INSERT INTO Estufa (Id, Nome, TemperaturaIdeal, AwsIP, EspId)
    VALUES (@Id, @Nome, @TemperaturaIdeal, @AwsIP, @EspId)
END

GO

CREATE OR ALTER PROCEDURE spUpdate_Estufa
(
	 @Id INT,
	 @Nome varchar(max),
	 @TemperaturaIdeal DECIMAL(5,2),
	 @AwsIP VARCHAR(15),
	 @EspId INT
)
AS
BEGIN
	UPDATE Estufa set nome = @Nome, temperaturaIdeal = @TemperaturaIdeal, AwsIP = @AwsIP, EspId = @EspId where id = @id 
END

GO

-- Listagem com join
CREATE OR ALTER PROCEDURE spListagemEstufaEsp
AS
BEGIN
    SELECT 
        e.Id,
        e.Nome,
        e.TemperaturaIdeal,
		e.AwsIP,
        e.EspId,
        esp.Nome AS EspNome,
        esp.EnderecoIP AS EspEnderecoIP
    FROM Estufa e
    LEFT JOIN EspDevice esp ON e.EspId = esp.Id
    ORDER BY e.Id;
END

-- Deletar estufas quando um ESP é excluído
GO
CREATE OR ALTER PROCEDURE spDeleteEspDeviceComEstufas
    @EspId INT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        DELETE FROM Estufa WHERE EspId = @EspId;
        DELETE FROM EspDevice WHERE Id = @EspId;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = 'Erro ao deletar ESP e estufas associadas: ' + ERROR_MESSAGE();
        THROW 50000, @ErrorMessage, 1;
    END CATCH
END



------------------------------------------------
-- Tabela monitoramento
------------------------------------------------

CREATE TABLE MonitoramentoEstufa (
    Id INT PRIMARY KEY,
    EstufaId INT NOT NULL,
    DataHora DATETIME NOT NULL DEFAULT GETDATE(),
    Temperatura DECIMAL(5,2) NOT NULL,
    FOREIGN KEY (EstufaId) REFERENCES Estufa(Id)
);

CREATE INDEX IX_MonitoramentoEstufa_EstufaId ON MonitoramentoEstufa(EstufaId);
CREATE INDEX IX_MonitoramentoEstufa_DataHora ON MonitoramentoEstufa(DataHora DESC);

GO

CREATE OR ALTER PROCEDURE spInsert_MonitoramentoEstufa
    @Id INT,
    @EstufaId INT,
    @Temperatura DECIMAL(5,2),
    @DataHora DATETIME
AS
BEGIN
    INSERT INTO MonitoramentoEstufa (Id, EstufaId, Temperatura, DataHora)
    VALUES (@Id, @EstufaId, @Temperatura, @DataHora)
END

GO

-- Consulta com join
CREATE OR ALTER PROCEDURE spConsultaEstufa
    @Id INT
AS
BEGIN
    SELECT 
        e.Id,
        e.Nome,
        e.TemperaturaIdeal,
        e.AwsIP,
        e.EspId,
        esp.Nome AS EspNome,
        esp.EnderecoIP AS EspEnderecoIP
    FROM Estufa e
    LEFT JOIN EspDevice esp ON e.EspId = esp.Id
	WHERE e.Id = @Id
END
EXEC spConsultaEstufa @Id = 1
GO

CREATE OR ALTER PROCEDURE spConsultaMonitoramentoEstufa
    @EstufaId INT,
    @Horas INT = 24 
AS
BEGIN
    SELECT 
        DataHora,
        Temperatura
    FROM MonitoramentoEstufa
    WHERE EstufaId = @EstufaId
    AND DataHora >= DATEADD(HOUR, -@Horas, GETDATE())
    ORDER BY DataHora DESC
END

CREATE OR ALTER PROCEDURE spListagemMonitoramentoPorEstufa
    @EstufaId INT,
    @UltimosRegistros INT = 50
AS
BEGIN
    SELECT TOP (@UltimosRegistros) *
    FROM MonitoramentoEstufa
    WHERE EstufaId = @EstufaId
    ORDER BY DataHora DESC
END

CREATE OR ALTER PROCEDURE spUltimaLeituraMonitoramento
    @EstufaId INT
AS
BEGIN
    SELECT TOP 1 *
    FROM MonitoramentoEstufa
    WHERE EstufaId = @EstufaId
    ORDER BY DataHora DESC
END

CREATE OR ALTER PROCEDURE spVerificaExistenciaDataHora
    @EstufaId INT,
    @DataHora DATETIME
AS
BEGIN
    SELECT TOP 1 Id 
    FROM MonitoramentoEstufa 
    WHERE EstufaId = @EstufaId 
    AND ABS(DATEDIFF(SECOND, DataHora, @DataHora)) < 5 -- Tolerância de 5 segundos
END
SELECT TOP 10 * 
FROM MonitoramentoEstufa 
WHERE EstufaId = 1
ORDER BY DataHora DESC
