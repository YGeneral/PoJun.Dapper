/*
	分页存储过程  
	编写人：杨江军 
	编写时间：2012-4-13
*/
--判断存储过程是否存在，存在就删除
if exists(select * from sysobjects where id=object_id('proc_Sql_Paging') and xtype='P')
 --删除存储过程
 drop proc proc_Sql_Paging
GO
--创建分页存储过程 
create PROCEDURE proc_Sql_Paging                              
@sqlDataTable nvarchar(4000),--表名
@primaryKey nvarchar(4000),--主键名称
@Fields nvarchar(4000),--要返回的字段 
@pageSize INT,--页尺寸 
@pageIndex INT,--页码 
@recordCount INT OUTPUT,--记录总数
@strOrderBy nvarchar(4000),--排序
@strWhere nvarchar(4000)--查询条件
AS
BEGIN
  SET NOCOUNT ON 
  DECLARE @strSQL1 nvarchar(4000)--SQL语句1
  DECLARE @strSQL2 nvarchar(4000)--SQL语句2
  DECLARE @strSQL3 nvarchar(4000)--SQL语句3
  
  SET @strSQL1 = 'SELECT ' + @primaryKey + ', ROW_NUMBER() OVER (' + SUBSTRING(@strOrderBy,CHARINDEX('order',@strOrderBy),len(@strOrderBy)) +  ') AS RowNumber FROM ' + @sqlDataTable + ' ' + @strWhere                        
  --获取总记录数
  SET @strSQL3 = 'SELECT @recordCount = COUNT(*) FROM ' + @sqlDataTable + ' ' + @strWhere
  EXEC SP_EXECUTESQL 
  @stmt = @strSQL3,
  @params = N'@recordCount AS INT OUTPUT',
  @recordCount = @recordCount OUTPUT
           
  --分页查询
  IF @pageIndex > @recordCount * 1.0 / @pageSize + 1.0 OR @recordCount <= @pageSize
  BEGIN
    SET @pageIndex = 1
  END
  
  SET @strSQL2 = 'SELECT ' + @Fields + ' FROM ' + @sqlDataTable + ' WHERE ' + @primaryKey + ' IN (SELECT '+@primaryKey+' FROM ('+@strSQL1+') TempTable WHERE RowNumber BETWEEN ' + Str((@pageIndex - 1) * @pageSize + 1) + ' AND ' + Str(@pageIndex * @pageSize) + ') ' + @strOrderBy
  
  EXEC SP_EXECUTESQL @strSQL2  
END
GO
 