﻿CREATE DATABASE ENGEMANINTRANET
ON
PRIMARY ( NAME = TSDENGE,
          FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\ENGEMANINTRANET_DTA.MDF',
          SIZE = 20MB,
          FILEGROWTH = 10%),
FILEGROUP INDICE
( NAME = TSIENGE,
  FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\ENGEMANINTRANET_IDX.NDF',
  SIZE = 5MB,
  FILEGROWTH = 10%)
LOG ON ( NAME = TSTEMP,
  FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\ENGEMANINTRANET_LOG.LDF',
  SIZE = 1MB,
  FILEGROWTH = 10%)
  COLLATE SQL_Latin1_General_CP1_CI_AI;
GO

--EXEC SP_ADDLOGIN [ENGEMANINTRANET], [ENGEMANINTRANET], [ENGEMANINTRANET]
--GO

USE ENGEMANINTRANET
GO

EXEC SP_GRANTDBACCESS [ENGEMANINTRANET],[ENGEMANINTRANET]
EXEC SP_ADDROLEMEMBER [DB_OWNER],[ENGEMANINTRANET]

/* ============================================================================================

   Atenção!

   Se o owner do banco de dados não for ENGEMANINTRANET, localize todas as ocorrências de ENGEMANINTRANET
   no script abaixo e troque pelo nome do novo owner.


   ============================================================================================ */
/*
	Traduzindo o MSSQL para PORTUGUÊS
*/
 
EXEC SP_DEFAULTLANGUAGE 'ENGEMANINTRANET', 'BRAZILIAN'

/* ===================================== Criação das Sequences ===================================================== */

CREATE SEQUENCE GENDEPARTMENT START WITH 1 AS NUMERIC(18);
CREATE SEQUENCE GENUSERACCOUNT START WITH 1 AS NUMERIC(18);
CREATE SEQUENCE GENPOST START WITH 1 AS NUMERIC(18);
CREATE SEQUENCE GENCOMMENT START WITH 1 AS NUMERIC(18);
CREATE SEQUENCE GENPOSTFILE START WITH 1 AS NUMERIC(18);
CREATE SEQUENCE GENCOMMENTFILE START WITH 1 AS NUMERIC(18);
CREATE SEQUENCE GENPOSTRESTRICTION START WITH 1 AS NUMERIC(18);
CREATE SEQUENCE GENLOG START WITH 1 AS NUMERIC(18);
CREATE SEQUENCE GENBLACKLISTTERM START WITH 1 AS NUMERIC(18);
CREATE SEQUENCE GENKEYWORD START WITH 1 AS NUMERIC(18);
CREATE SEQUENCE GENPOSTKEYWORD START WITH 1 AS NUMERIC(18);
CREATE SEQUENCE GENNOTIFICATION START WITH 1 AS NUMERIC(18);
CREATE SEQUENCE GENNOTIFICATIONTYPE START WITH 1 AS NUMERIC(18);

/* ====================================== Criação das Tabelas ====================================================== */

CREATE TABLE DEPARTMENT ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENDEPARTMENT),       
	ACTIVE                        BIT DEFAULT 1                        NOT NULL,
  CODE                          VARCHAR(25)                          NOT NULL,
	DESCRIPTION                   VARCHAR(100)                         NOT NULL,
  CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP       NULL,

	CHECK (CODE <> ''),
	CHECK (DESCRIPTION <> ''),

	CONSTRAINT PK_DEPARTMENT PRIMARY KEY CLUSTERED (ID)
	)ON [PRIMARY]

CREATE TABLE USERACCOUNT ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENUSERACCOUNT),
	ACTIVE                        BIT DEFAULT 1                       NOT NULL,
  NAME                          VARCHAR(30)                         NOT NULL,
	USERNAME                      VARCHAR(100)                        NOT NULL,	 
  EMAIL                         VARCHAR(50)                         NOT NULL,
	PHOTO                         VARBINARY(MAX)					            DEFAULT 0x89504E470D0A1A0A0000000D49484452000000E1000000E10803000000096D224800000033504C5445E4E6E7AEB4B7AAB1B4E7E9EAA9AFB3C9CDCFE3E5E6DFE1E2D9DCDDB0B6B9C1C6C8CDD0D2C6CACCBABFC1D3D6D8ACB2B6BCC0C3714CA3BD0000050E49444154789CED9DDD7A83200C4095F8AFA0EFFFB443995B3BDB55312989CDB9D96DCF17480021CB3245511445511445511445511445511445511445511445511445511445F9600052FF022AC0AB35CEF51ED734155CCBD4DBB8C1E6C6337A96BF7670D5552CA12EAD57CAFF308E455BD6F225019C2D36763F96A6ED33D18E9095A379A6173066A8E43A42B91D9C8F0239088D23B87C875F70EC052A42655F8CCFBBB1DAD6D21CC11DF05B1C858511BA83825ED18A523C324257C6B64AFDB37753B5C7FD96303642C258C7F9C951AC77D608B18AF1110C8AA97FFF6B22E7E0CA58A71678019C14CCF336B5C2FF44D4C10DACEBE2E195CC234CC958B14210CCF3826FB6018B21E8A722D720428F12423F4E07AE8A274AFD1F459E25033A3443A6F9B4461AA3332C973630E109F20C625D601A320C22E62C9C611844C45938C32E9DA2D5C295915B4D3CBFA7D8283233C42C1501E3523BDD01256E9E99E95805117F907A581956A8C530C06B1385B1F3FDCBC869278C5DEE039C8A3EC934643511816090F25AD6340489865745A44834AC520D45BD9FE153F3695229A7648ABBBDFF85CFA922D639E9063E8634E5302F2E6F68AE6F985AEC07A011E464A8A334124699E6FAD5E2FA15FFFAABB6CBAFBCAFBF7BCA1A1A43463BE08C668FCFE81483A62072FA72011D85219F54EA7104C99453A2F984537D8A89C8691A7EC2D7B5EB7F21FD80AFDCD0230FD391D920CDAE7FDB04BDE8B32AF781CBDFFA42DEE8F3D9DEDF70F9DB971F708316E9A6FE4CC12E9106D06EEFB1BBB5F703D6C286659A09E0241B5EDBA67B705E05B11DA33318F994691E5D41988AA9155E519DF4E3B7E2DE702EDBC878087C42B190209841FC636EE3240866F1CFB9450CD140D5468551527F1398223A7FA4FED1C738DEBD85D519FE1E2EDF81275BC2B8BB8B92B8000600865D9DB08A4E6EB72FA806F362AC8EA69338407F81AC6F9F07726EBB27377E2B00F5D0160F2CBDDED0C8EF2AB80050B9CE7EF785FCEE0DD976FD059A26DE025EB3717D5F96737BCFFA62DD3D6F80ABAAC12352FF28141691DAB97EE8266BDB156BA76E28FD68AD04AB86AEBA65D7E6C59262B6A974CE3785696DD7CB6BBA3BCBF59D2F11BB966D8BAA1D9C94EC137A06EF73FBE3D94EFC0B0840F3B067F06E4D93778E6F436CAFD7E5CF7B06EFC514B6E7B8929BF5C6F8E0DD331ACBAD813254E53FEBEB38C9C9B119ADFF37B48EC7E43CF61D007E7B84AFB7E0F78EC9CF17217BB9C53D87B1491D673F9A77083C1C615F4372B98ED0EF6D488EE038BDFF2C071AB2FCF290E2CD9FBEA13A7E6A7F92317FE77729940B098731D3BBCAE3B1F37A54C7F784314D00BF15A7770822740A8E67CCA90B07447EFBC4A3A0FD1F0A40F30CEF98E244A8083DC92BBC838C741DBF61481FC100D10A27E2DB3C15349736C026CE31B75028B212A4506426887F3F8C9D207614D32E649E80794F134A0E75700B9EA0E3298877A71FFF7D2812586F14A9FAEB2080B36184895D1AFD0523DBA4DCF0EE00632A328E608ED17881AA41121AA79FB931D8F2BEE0E4131BC67974A538D59900FDFF019070268854ED0251194F9C4D51F5F042E644F3419A26D6E8C4FF9356F45E1764441BF24FA481E8462FFC6BE14A644DA469E04542EC025C4C086357A7BC3715F7446D31040DD2C8F5374829153351EB1ADC8E3AD4447C729353EE03118692A66154BD10B3A009442C6B84ACBA57222A22DB63E0271C3F1CA6E8484AC9F1A5A988F38B1B0EAF6A60981FF308A2381AC2ECC97B33B644082A8AA2288AA2288AA2288AA228EFE40BC9C65651C8D5962F0000000049454E44AE426082,
	DESCRIPTION				            VARCHAR(MAX)                        NULL,
	DEPARTMENT_ID                 NUMERIC(18)                         NOT NULL,
	PERMISSIONS                   NVARCHAR(2000)											NOT NULL,
	CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP  NULL,

	CHECK (NAME <> ''),
	CHECK (USERNAME <> ''),
	CHECK (EMAIL <> ''),
	CHECK (DEPARTMENT_ID > 0),

	CONSTRAINT PK_USERACCOUNT PRIMARY KEY CLUSTERED (ID),
	CONSTRAINT UK_USERNAME UNIQUE(USERNAME),
	CONSTRAINT FK_USERACCOUNT_DEPARTMENT FOREIGN KEY (DEPARTMENT_ID) REFERENCES DEPARTMENT(ID)
	)ON [PRIMARY]

CREATE TABLE POST ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENPOST),
  ACTIVE                        BIT DEFAULT 1                       NOT NULL,
	RESTRICTED                    BIT DEFAULT 0                       NOT NULL,     
	SUBJECT                       VARCHAR(255)                        NOT NULL,
	DESCRIPTION                   NVARCHAR(MAX)                       NOT NULL,
	CLEAN_DESCRIPTION             VARCHAR(MAX)                        NOT NULL,
  POST_TYPE	                    CHAR(1)                             NOT NULL, /* I = Informativa, Q = Pergunta  M = Manual, D = Documento,  */
	REVISED                       BIT DEFAULT 0                       NOT NULL,
	USER_ACCOUNT_ID               NUMERIC(18)                         NOT NULL,
	CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP      NULL,

  CHECK (SUBJECT <> ''),
  CHECK (DESCRIPTION <> ''),
	CHECK (POST_TYPE in ('I', 'Q', 'M', 'D')),
	CHECK (USER_ACCOUNT_ID > 0),

	CONSTRAINT PK_POST PRIMARY KEY CLUSTERED(ID),
	CONSTRAINT FK_POST_USERACCOUNT FOREIGN KEY (USER_ACCOUNT_ID) REFERENCES USERACCOUNT(ID)
	)ON [PRIMARY]

CREATE TABLE COMMENT ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENCOMMENT),
  ACTIVE                        BIT DEFAULT 1                       NOT NULL,
	DESCRIPTION                   NVARCHAR(MAX)                       NOT NULL,	 
	CLEAN_DESCRIPTION             VARCHAR(MAX)                        NOT NULL,
	REVISED                       BIT DEFAULT 0                       NOT NULL,
	USER_ACCOUNT_ID               NUMERIC(18)                         NOT NULL,
	POST_ID                       NUMERIC(18)                         NOT NULL,
	CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP      NULL,

	CHECK (DESCRIPTION <> ''),
	CHECK (USER_ACCOUNT_ID > 0),
	CHECK (POST_ID > 0),

	CONSTRAINT PK_COMMENT PRIMARY KEY CLUSTERED(ID),
	CONSTRAINT FK_COMMENT_POST FOREIGN KEY (POST_ID) REFERENCES POST(ID) ON DELETE CASCADE,
	CONSTRAINT FK_COMMENT_USERACCOUNT FOREIGN KEY (USER_ACCOUNT_ID) REFERENCES USERACCOUNT(ID)
	)ON [PRIMARY]

CREATE TABLE POSTFILE ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENPOSTFILE),
	NAME                          VARCHAR(100)                        NOT NULL,
	BINARY_DATA				            VARBINARY(MAX)                      NOT NULL,
	POST_ID                       NUMERIC(18)                         NOT NULL,
	CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP  NULL,

  CHECK (NAME <> ''),
  CHECK (BINARY_DATA <> ''),
	CHECK (POST_ID > 0),
	
	CONSTRAINT PK_POSTFILE PRIMARY KEY CLUSTERED(ID),
  CONSTRAINT FK_POSTFILE_POST FOREIGN KEY (POST_ID) REFERENCES POST(ID) ON DELETE CASCADE)ON [PRIMARY]

CREATE TABLE COMMENTFILE ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENCOMMENTFILE),
	NAME                          VARCHAR(100)                        NOT NULL,
	BINARY_DATA				            VARBINARY(MAX)                      NOT NULL,
	COMMENT_ID                    NUMERIC(18)                         NOT NULL,
	CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP  NULL,

  CHECK (NAME <> ''),
  CHECK (BINARY_DATA <> ''),
	CHECK (COMMENT_ID > 0),

	CONSTRAINT PK_COMMENTFILE PRIMARY KEY CLUSTERED(ID),
	CONSTRAINT FK_COMMENTFILE_COMMENT FOREIGN KEY (COMMENT_ID) REFERENCES COMMENT(ID) ON DELETE CASCADE
	)ON [PRIMARY]

CREATE TABLE POSTRESTRICTION ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENPOSTRESTRICTION),
	POST_ID                       NUMERIC(18)                         NOT NULL,
	DEPARTMENT_ID                 NUMERIC(18)                         NOT NULL,
	CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP  NULL,

	CHECK (POST_ID > 0),
  CHECK (DEPARTMENT_ID > 0),

	CONSTRAINT PK_POSTRESTRICTION PRIMARY KEY CLUSTERED(ID),
	CONSTRAINT FK_POSTRESTRICTION_POST FOREIGN KEY (POST_ID) REFERENCES POST(ID) ON DELETE CASCADE,
	CONSTRAINT FK_POSTRESTRICTION_DEPARTMENT FOREIGN KEY (DEPARTMENT_ID) REFERENCES DEPARTMENT(ID) ON DELETE CASCADE
	)ON [PRIMARY]

CREATE TABLE LOG ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENLOG),
	OPERATION										  VARCHAR(30)                         NOT NULL,
	REGISTRY_TYPE                 VARCHAR(200)                        NOT NULL,
	REGISTRY_ID                   NUMERIC(18) DEFAULT NULL            NULL,
	REGISTRY_TABLE                VARCHAR(100) DEFAULT NULL           NULL,
	USERNAME                      VARCHAR(100)                        NOT NULL,
	CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP  NULL,

	CONSTRAINT PK_LOG PRIMARY KEY CLUSTERED(ID),

  CHECK (OPERATION <> ''),
	CHECK (REGISTRY_TYPE <> ''),
	CHECK (REGISTRY_ID > 0),
	CHECK (REGISTRY_TABLE <> ''),
	CHECK (USERNAME <> '')
	)ON [PRIMARY]

CREATE TABLE BLACKLISTTERM ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENBLACKLISTTERM),
	DESCRIPTION										VARCHAR(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP  NULL,

	CONSTRAINT PK_BLACKLISTTERM PRIMARY KEY CLUSTERED(ID),
	CONSTRAINT UK_BLACKLISTTERM_DESCRIPTION UNIQUE(DESCRIPTION),

	CHECK (DESCRIPTION <> ''),
	)ON [PRIMARY]
GO

CREATE TABLE KEYWORD ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENKEYWORD),
	DESCRIPTION										VARCHAR(100)                        NOT NULL,
	CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP  NULL,

	CONSTRAINT PK_KEYWORD PRIMARY KEY CLUSTERED(ID),
	CONSTRAINT UK_KEYWORD_DESCRIPTION UNIQUE(DESCRIPTION),

	CHECK (DESCRIPTION <> ''),
	)ON [PRIMARY]
GO

CREATE TABLE POSTKEYWORD ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENPOSTKEYWORD),
	POST_ID                       NUMERIC(18)                         NOT NULL,
	KEYWORD_ID                    NUMERIC(18)                         NOT NULL,
	KEYWORD                       VARCHAR(100)                        NOT NULL,
	CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP  NULL,

  CHECK (KEYWORD <> ''),
	CHECK (POST_ID > 0),
	CHECK (KEYWORD_ID > 0),
	
	CONSTRAINT PK_POSTKEYWORD PRIMARY KEY CLUSTERED(ID),
  CONSTRAINT FK_POSTKEYWORD_POST FOREIGN KEY (POST_ID) REFERENCES POST(ID) ON DELETE CASCADE,
  CONSTRAINT FK_POSTKEYWORD_KEYWORD FOREIGN KEY (KEYWORD_ID) REFERENCES KEYWORD(ID) ON DELETE CASCADE)ON [PRIMARY]
	GO

  CREATE TABLE NOTIFICATIONTYPE ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENNOTIFICATIONTYPE),
	CODE                          VARCHAR(4)                          NOT NULL,
	DESCRIPTION                   VARCHAR(100)                        NOT NULL,
	CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP  NULL,

  CHECK (DESCRIPTION <> ''),

  CONSTRAINT PK_NOTIFICATIONTYPE PRIMARY KEY CLUSTERED(ID)) ON [PRIMARY]
	GO

  CREATE TABLE NOTIFICATION ( 
	ID                            NUMERIC(18) DEFAULT (NEXT VALUE FOR GENNOTIFICATION),
	BY_USER_ID                    NUMERIC(18)                         NOT NULL,
	TO_USER_ID                    NUMERIC(18)                         NOT NULL,
	REGISTRY_ID                   NUMERIC(18)                         NOT NULL,
  NOTIFICATION_TYPE_ID          NUMERIC(18)                         NOT NULL,
	DESCRIPTION                   VARCHAR(200)                        NULL,
  VIEWED                        BIT DEFAULT 0                       NOT NULL,
	REVISED                       BIT DEFAULT 1                       NULL,
	CHANGE_DATE                   DATETIME DEFAULT CURRENT_TIMESTAMP  NULL,

  CHECK (BY_USER_ID > 0),
  CHECK (TO_USER_ID > 0),
  CHECK (REGISTRY_ID > 0),
  CHECK (NOTIFICATION_TYPE_ID > 0),

  CONSTRAINT PK_NOTIFICATION PRIMARY KEY CLUSTERED(ID),
  CONSTRAINT FK_NOTIFICATION_NOTIFICATIONTYPE FOREIGN KEY (NOTIFICATION_TYPE_ID) REFERENCES NOTIFICATIONTYPE(ID) ON DELETE CASCADE,
  CONSTRAINT FK_NOTIFICATION_USERACCOUNT1 FOREIGN KEY (BY_USER_ID) REFERENCES USERACCOUNT(ID) ON DELETE CASCADE,
  CONSTRAINT FK_NOTIFICATION_USERACCOUNT2 FOREIGN KEY (TO_USER_ID) REFERENCES USERACCOUNT(ID)) ON [PRIMARY]
	GO
/* ====================================== Criação de Stored Procedures ====================================================== */

CREATE PROCEDURE NEW_LOG
		@operation VARCHAR(11),
		@registry_type VARCHAR(50),
    @registry_id int,
		@registry_table VARCHAR(50),
		@username VARCHAR(50)
    AS
    BEGIN
        SET NOCOUNT ON;
			SET @operation = CASE
				WHEN @operation = 'I' THEN 'Inserção'
				WHEN @operation = 'D' THEN 'Exclusão'
				WHEN @operation = 'U' THEN 'Atualização'
				WHEN @operation = 'A' THEN 'Aprovação'
				ELSE NULL  
			END
			SET @registry_type = CASE
				WHEN @registry_type = 'POS' THEN 'Postagem'
				WHEN @registry_type = 'COM' THEN 'Comentário'
				WHEN @registry_type = 'USU' THEN 'Usuário'
				WHEN @registry_type = 'TER' THEN 'Termo'
				WHEN @registry_type = 'KEY' THEN 'Palavra-chave'
				ELSE NULL  
			END
				INSERT INTO LOG (OPERATION, REGISTRY_TYPE, REGISTRY_ID, REGISTRY_TABLE, USERNAME)
                VALUES(@operation,@registry_type,@registry_id,@registry_table,@username) 
    END
		GO

/* ====================================== Criação de Triggers ====================================================== */

CREATE TRIGGER DEPARTMENT_CHANGEDATE ON DEPARTMENT AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE DEPARTMENT SET CHANGE_DATE = CURRENT_TIMESTAMP FROM DEPARTMENT
  INNER JOIN INSERTED ON DEPARTMENT.ID = INSERTED.ID;
END
GO

CREATE TRIGGER USERACCOUNT_CHANGEDATE ON USERACCOUNT AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE USERACCOUNT SET CHANGE_DATE = CURRENT_TIMESTAMP FROM USERACCOUNT
  INNER JOIN INSERTED ON USERACCOUNT.ID = INSERTED.ID;
END
GO

CREATE TRIGGER POST_CHANGEDATE ON POST AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE POST SET CHANGE_DATE = CURRENT_TIMESTAMP FROM POST
  INNER JOIN INSERTED ON POST.ID = INSERTED.ID;
END
GO

CREATE TRIGGER COMMENT_CHANGEDATE ON COMMENT AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE COMMENT SET CHANGE_DATE = CURRENT_TIMESTAMP FROM COMMENT
  INNER JOIN INSERTED ON COMMENT.ID = INSERTED.ID;
END
GO

CREATE TRIGGER POSTFILE_CHANGEDATE ON POSTFILE AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE POSTFILE SET CHANGE_DATE = CURRENT_TIMESTAMP FROM POSTFILE
  INNER JOIN INSERTED ON POSTFILE.ID = INSERTED.ID;
END
GO

CREATE TRIGGER COMMENTFILE_CHANGEDATE ON COMMENTFILE AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE COMMENTFILE SET CHANGE_DATE = CURRENT_TIMESTAMP FROM COMMENTFILE
  INNER JOIN INSERTED ON COMMENTFILE.ID = INSERTED.ID;
END
GO

CREATE TRIGGER POSTRESTRICTION_CHANGEDATE ON POSTRESTRICTION AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE POSTRESTRICTION SET CHANGE_DATE = CURRENT_TIMESTAMP FROM POSTRESTRICTION
  INNER JOIN INSERTED ON POSTRESTRICTION.ID = INSERTED.ID;
END
GO

CREATE TRIGGER LOG_CHANGEDATE ON LOG AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE LOG SET CHANGE_DATE = CURRENT_TIMESTAMP FROM LOG
  INNER JOIN INSERTED ON LOG.ID = INSERTED.ID;
END
GO

CREATE TRIGGER BLACKLISTTERM_CHANGEDATE ON BLACKLISTTERM AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE BLACKLISTTERM SET CHANGE_DATE = CURRENT_TIMESTAMP FROM BLACKLISTTERM
  INNER JOIN INSERTED ON BLACKLISTTERM.ID = INSERTED.ID;
END
GO

CREATE TRIGGER KEYWORD_CHANGEDATE ON KEYWORD AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE KEYWORD SET CHANGE_DATE = CURRENT_TIMESTAMP FROM KEYWORD
  INNER JOIN INSERTED ON KEYWORD.ID = INSERTED.ID;
END
GO

CREATE TRIGGER POSTKEYWORD_CHANGEDATE ON POSTKEYWORD AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE POSTKEYWORD SET CHANGE_DATE = CURRENT_TIMESTAMP FROM POSTKEYWORD
  INNER JOIN INSERTED ON POSTKEYWORD.ID = INSERTED.ID;
END
GO

CREATE TRIGGER NOTIFICATION_CHANGEDATE ON NOTIFICATION AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE NOTIFICATION SET CHANGE_DATE = CURRENT_TIMESTAMP FROM NOTIFICATION
  INNER JOIN INSERTED ON NOTIFICATION.ID = INSERTED.ID;
END
GO

CREATE TRIGGER NOTIFICATIONTYPE_CHANGEDATE ON NOTIFICATIONTYPE AFTER UPDATE
AS 
 SET NOCOUNT ON
BEGIN
  UPDATE NOTIFICATIONTYPE SET CHANGE_DATE = CURRENT_TIMESTAMP FROM NOTIFICATIONTYPE
  INNER JOIN INSERTED ON NOTIFICATIONTYPE.ID = INSERTED.ID;
END
GO

--Trigger para atualizar as palavras-chave na tabela POSTKEYWORD após um UPDATE na tabela KEYWORD
CREATE TRIGGER UPDATE_KEYWORD
ON KEYWORD
FOR UPDATE
AS
BEGIN
	DECLARE @KEYWORD_ID INT, @KEYWORD VARCHAR(50)
	SELECT @KEYWORD_ID = ID, @KEYWORD = DESCRIPTION FROM INSERTED
	UPDATE POSTKEYWORD SET KEYWORD = @KEYWORD WHERE KEYWORD_ID = @KEYWORD_ID
END
GO

/* =================================== Configuração da pesquisa de texto completo =============================================== */

CREATE FULLTEXT CATALOG ENGEMAN_INTRANET_CATALOG AS DEFAULT

CREATE UNIQUE INDEX UI_POST ON POST(ID) 
CREATE UNIQUE INDEX UI_COMMENT ON COMMENT(ID)
CREATE UNIQUE INDEX UI_POSTKEYWORD ON POSTKEYWORD(ID)

CREATE FULLTEXT INDEX ON POST
(
	SUBJECT Language 1046,
	CLEAN_DESCRIPTION Language 1046
)
KEY INDEX UI_POST ON ENGEMAN_INTRANET_CATALOG 
WITH CHANGE_TRACKING AUTO, STOPLIST = SYSTEM

CREATE FULLTEXT INDEX ON COMMENT
(
	CLEAN_DESCRIPTION Language 1046
)
KEY INDEX UI_COMMENT ON ENGEMAN_INTRANET_CATALOG 
WITH CHANGE_TRACKING AUTO, STOPLIST = SYSTEM

CREATE FULLTEXT INDEX ON POSTKEYWORD
(
	KEYWORD Language 1046
)
KEY INDEX UI_POSTKEYWORD ON ENGEMAN_INTRANET_CATALOG 
WITH CHANGE_TRACKING AUTO, STOPLIST = SYSTEM

/* =================================== Inserção de Dados para Testes =============================================== */

INSERT INTO DEPARTMENT (CODE, DESCRIPTION) VALUES ('001', 'TI Web')
INSERT INTO DEPARTMENT (CODE, DESCRIPTION) VALUES ('002', 'TI Desktop')
INSERT INTO DEPARTMENT (CODE, DESCRIPTION) VALUES ('003', 'TI Mobile')
INSERT INTO DEPARTMENT (CODE, DESCRIPTION) VALUES ('004', 'TI Personalização')

INSERT INTO USERACCOUNT (NAME, USERNAME, DEPARTMENT_ID, EMAIL, PERMISSIONS) VALUES ('Samuel Moreira', 'samuel.moreira', 1, 'samuel.moreira@engeman.com.br', '{
  "PostType":{
  "Informative": {
    "CanPost": 1,
    "CanComment": 1,
    "EditAnyPost": 1,
    "DeleteAnyPost": 1,
    "RequiresModeration":0
  },
    "Question": {
    "CanPost": 1,
    "CanComment": 1,
    "EditAnyPost": 1,
    "DeleteAnyPost": 1,
    "RequiresModeration":0
  },
    "Manual": {
    "CanPost": 1,
    "CanComment": 1,
    "EditAnyPost": 1,
    "DeleteAnyPost": 1,
    "RequiresModeration":0
  },
    "Document": {
    "CanPost": 1,
    "CanComment": 1,
    "EditAnyPost": 1,
    "DeleteAnyPost": 1,
    "RequiresModeration":0
  }
}
}')
INSERT INTO USERACCOUNT (NAME, USERNAME, DEPARTMENT_ID, EMAIL, PERMISSIONS) VALUES ('Michel Queiroz', 'michel.queiroz', 1, 'michel.queiroz@engeman.com.br', '{
  "PostType":{
  "Informative": {
    "CanPost": 1,
    "CanComment": 1,
    "EditAnyPost": 1,
    "DeleteAnyPost": 1,
    "RequiresModeration":0
  },
    "Question": {
    "CanPost": 1,
    "CanComment": 1,
    "EditAnyPost": 1,
    "DeleteAnyPost": 1,
    "RequiresModeration":0
  },
    "Manual": {
    "CanPost": 1,
    "CanComment": 1,
    "EditAnyPost": 1,
    "DeleteAnyPost": 1,
    "RequiresModeration":0
  },
    "Document": {
    "CanPost": 1,
    "CanComment": 1,
    "EditAnyPost": 1,
    "DeleteAnyPost": 1,
    "RequiresModeration":0
  }
}
}')
INSERT INTO USERACCOUNT (NAME, USERNAME, DEPARTMENT_ID, EMAIL, PERMISSIONS) VALUES ('Pedro Silva', 'pedro.silva', 4, 'pedro.silva@engeman.com.br', '{
  "PostType":{
  "Informative": {
    "CanPost": 1,
    "CanComment": 1,
    "EditAnyPost": 0,
    "DeleteAnyPost": 0,
    "RequiresModeration":1
  },
    "Question": {
    "CanPost": 1,
    "CanComment": 1,
    "EditAnyPost": 0,
    "DeleteAnyPost": 0,
    "RequiresModeration":1
  },
    "Manual": {
    "CanPost": 1,
    "CanComment": 1,
    "EditAnyPost": 1,
    "DeleteAnyPost": 1,
    "RequiresModeration":0
  },
    "Document": {
    "CanPost": 1,
    "CanComment": 1,
    "EditAnyPost": 1,
    "DeleteAnyPost": 1,
    "RequiresModeration":0
  }}
}')

INSERT INTO NOTIFICATIONTYPE (CODE, DESCRIPTION) VALUES ('NOCO','Fez um novo comentário na sua postagem')

--INSERT INTO POST (SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, USER_ACCOUNT_ID, POST_TYPE, REVISED)
--VALUES ('Como instalar o Engeman Web?', N'<p>Estou com dúvidas sobre a instalação do Engeman Web no Windows Server 2016. É possível instalar nessa versão? Tem alguma restrição quanto ao uso de um certificado autoassinado?</p>', 'Estou com dúvidas sobre a instalação do Engeman Web no Windows Server 2016. É possível instalar nessa versão? Tem alguma restrição quanto ao uso de um certificado autoassinado?', 1, 'Q', 1)
--INSERT INTO POST (SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, USER_ACCOUNT_ID, POST_TYPE, REVISED)
--VALUES ('Procedimentos para Migração Cloud', N'<p>Preciso de um manual que contenha instruções sobre como realizar corretamente a migração para o &nbsp;ambiente em nuvem da Engeman&reg;.</p>', 'Preciso de um manual que contenha instruções sobre como realizar corretamente a migração para o ambiente em nuvem da Engeman.', 1, 'I', 1)
--INSERT INTO POST (SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, USER_ACCOUNT_ID, POST_TYPE, REVISED)
--VALUES ('Dashboards Engeman', N'<p>Onde posso encontrar um documento explicativo sobre o Dashboard Engeman?</p>', 'Onde posso encontrar um documento explicativo sobre o Dashboard Engeman?', 9, 'Q', 1)
--INSERT INTO POST (SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, USER_ACCOUNT_ID, POST_TYPE, REVISED)
--VALUES ('Passo a passo para instalar o Engeman Client/Server.', N'<p>Estou com dúvida sobre a instalação do Engeman Client/Server, existe algum manual ou documento detalhado que possa me ajudar?</p>', 'Estou com dúvida sobre a instalação do Engeman Client/Server, existe algum manual ou documento detalhado que possa me ajudar?', 6, 'Q', 1)
--INSERT INTO POST (SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, USER_ACCOUNT_ID, POST_TYPE, REVISED)
--VALUES ('Conversão de banco de dados', N'<p>Necessito de algum documento que contenha o fluxo a ser seguido para conversão de banco de dados. Alguém pode me dizer onde posso encontrar?</p>', 'Necessito de algum documento que contenha o fluxo a ser seguido para conversão de banco de dados. Alguém pode me dizer onde posso encontrar?', 8, 'Q', 1)
--INSERT INTO POST (SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, USER_ACCOUNT_ID, POST_TYPE, REVISED)
--VALUES ('Como instalar o Engeman Mobile?', N'<p>Sobre a instalação do Engeman Mobile, qual a versão mínima requisitada dos sistemas iOS ou Android para que possa ser feita a instalação?</p>', 'Sobre a instalação do Engeman Mobile, qual a versão mínima requisitada dos sistemas iOS ou Android para que possa ser feita a instalação', 10, 'Q', 1)

--Senha do banco: Engeman.1