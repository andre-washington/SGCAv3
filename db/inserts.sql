INSERT INTO [dbo].[Caixa]
           ([CaixaQtdCritica]
           ,[CaixaSituacao])
     VALUES
           (30,'Ativo')
GO

INSERT INTO [dbo].[Caixa]
           ([CaixaQtdCritica]
           ,[CaixaSituacao])
     VALUES
           (35,'Ativo')
GO

INSERT INTO [dbo].[Caixa]
           ([CaixaQtdCritica]
           ,[CaixaSituacao])
     VALUES
           (30,'Ativo')
GO


INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 2,1)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 5,1)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 10,1)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 20,1)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 25,1)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 2,2)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 5,2)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 10,2)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 20,2)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 25,2)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 2,3)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 5,3)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 10,3)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 20,3)
INSERT INTO [dbo].[Nota]([NotaQuantidade],[NotaValor],[CaixaId])VALUES (100, 25,3)

GO