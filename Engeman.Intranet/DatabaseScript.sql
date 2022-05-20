CREATE DATABASE ENGEMANINTRANET
ON
PRIMARY ( NAME = TSDENGE,
          FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\ENGEMANINTRANET_DTA.MDF',
          SIZE = 20MB,
          FILEGROWTH = 10%),
FILEGROUP INDICE
( NAME = TSIENGE,
  FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\ENGEMANINTRANET_IDX.NDF',
  SIZE = 5MB,
  FILEGROWTH = 10%)
LOG ON ( NAME = TSTEMP,
  FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\ENGEMANINTRANET_LOG.LDF',
  SIZE = 1MB,
  FILEGROWTH = 10%)
  COLLATE SQL_Latin1_General_CP1_CI_AI;
GO

--EXEC SP_ADDLOGIN [ENGEMANINTRANET], [ENGEMANINTRANET], [ENGEMANINTRANET]
--GO

USE ENGEMANINTRANET
GO
EXEC SP_GRANTDBACCESS [ENGEMANINTRANET],[ENGEMANINTRANET]
GO
EXEC SP_ADDROLEMEMBER [DB_OWNER],[ENGEMANINTRANET]
GO

/* ============================================================================================

   Atenção!

   Se o owner do banco de dados não for ENGEMANINTRANET, localize todas as ocorrências de ENGEMANINTRANET
   no script abaixo e troque pelo nome do novo owner.


   ============================================================================================ */
/*
	Traduzindo o MSSQL para PORTUGUÊS
*/
 
EXEC SP_DEFAULTLANGUAGE 'ENGEMANINTRANET', 'BRAZILIAN'

GO

/* ===================================== Criação das Sequences ===================================================== */

CREATE SEQUENCE GENDEPARTMENT START WITH 1 AS NUMERIC(18);
GO

CREATE SEQUENCE GENUSERACCOUNT START WITH 1 AS NUMERIC(18);
GO

/* ====================================== Criação das Tabelas ====================================================== */

CREATE TABLE DEPARTMENT ( 
     ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENDEPARTMENT) CONSTRAINT PK_DEPARTMENT PRIMARY KEY CLUSTERED,       
     CODE                          VARCHAR(25)                          NOT NULL,
	   DESCRIPTION                   VARCHAR(100)                         NOT NULL,
	   ACTIVE                        CHAR(1) DEFAULT 'S'                  NOT NULL,
     CHANGEDATE                    DATETIME DEFAULT CURRENT_TIMESTAMP       NULL,
     CHECK (DESCRIPTION <> ''))ON [PRIMARY]
GO

CREATE TABLE USERACCOUNT ( 
     ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENUSERACCOUNT) CONSTRAINT PK_USERACCONT PRIMARY KEY CLUSTERED,
     ACTIVE                        CHAR(1) DEFAULT 'S'                 NOT NULL,
     NAME                          VARCHAR(100)                        NOT NULL,
	   DOMAINACCOUNT                 VARCHAR(100)                        NOT NULL,
	   DEPARTMENT_ID                 NUMERIC(18,0)                       NOT NULL,
     CHANGEDATE                        DATETIME DEFAULT CURRENT_TIMESTAMP      NULL,     
     CHECK (NAME <> ''),
     CHECK (DOMAINACCOUNT <> ''))ON [PRIMARY]
GO

/* ====================================== Criação de Triggers ====================================================== */

CREATE TRIGGER DEPARTMENT_CHANGEDATE ON DEPARTMENT AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE DEPARTMENT SET CHANGEDATE = CURRENT_TIMESTAMP FROM DEPARTMENT
  INNER JOIN INSERTED ON DEPARTMENT.ID = INSERTED.ID;
END
GO

CREATE TRIGGER USERACCOUNT_CHANGEDATE ON USERACCOUNT AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE USERACCOUNT SET CHANGEDATE = CURRENT_TIMESTAMP FROM USERACCOUNT
  INNER JOIN INSERTED ON USERACCOUNT.ID = INSERTED.ID;
END
GO

/* =================================== Inserção de Dados para Testes =============================================== */

INSERT INTO DEPARTMENT (CODE, DESCRIPTION, ACTIVE) VALUES ('001', 'TI Engeman Web', 'S')
GO
INSERT INTO DEPARTMENT (CODE, DESCRIPTION, ACTIVE) VALUES ('002', 'TI Engeman Client/Server', 'S')
GO
INSERT INTO DEPARTMENT (CODE, DESCRIPTION, ACTIVE) VALUES ('003', 'TI Engeman Mobile', 'S')
GO
INSERT INTO DEPARTMENT (CODE, DESCRIPTION, ACTIVE) VALUES ('004', 'TI Engeman Personalização', 'N')
GO
SELECT * FROM DEPARTMENT
GO
INSERT INTO USERACCOUNT (NAME, DOMAINACCOUNT, ACTIVE, DEPARTMENT_ID) VALUES ('Michel Queiroz', 'michel.queiroz', 'S', 1)
GO
INSERT INTO USERACCOUNT (NAME, DOMAINACCOUNT, ACTIVE, DEPARTMENT_ID) VALUES ('Samuel Moreira', 'samuel.moreira', 'S', 1)
GO
INSERT INTO USERACCOUNT (NAME, DOMAINACCOUNT, ACTIVE, DEPARTMENT_ID) VALUES ('Alan Vasconcelos', 'alan.vasconcelos', 'S', 3)
GO
SELECT * FROM USERACCOUNT
GO
UPDATE DEPARTMENT SET ACTIVE = 'N' WHERE CODE = '004'
GO
SELECT * FROM DEPARTMENT
GO

--Senha do banco: Engeman.1