DELIMITER;
#判断存储过程是否存在，存在就删除
DROP PROCEDURE IF EXISTS proc_sql_Paging; #分隔符
CREATE PROCEDURE proc_sql_Paging (
	#输入参数
	_fields VARCHAR(2000), #要查询的字段，用逗号(,)分隔
	_sqlDataTable TEXT,  #要查询的表
	_strWhere VARCHAR(2000),   #查询条件
	_strOrderBy VARCHAR(200),  #排序规则
	_pageIndex INT,  #查询页码
	_pageSize INT,   #每页记录数
	#输出参数
	OUT _recordCount INT  #总记录数
)
BEGIN
	#分页存储过程
	#计算起始行号
	SET @startRow = _pageSize * (_pageIndex - 1);
	SET @pageSize = _pageSize;
	SET @rowindex = 0; #行号

	#合并字符串
	SET @strsql = CONCAT(
		#'select sql_calc_found_rows  @rowindex:=@rowindex+1 as rownumber,' #记录行号
		'select sql_calc_found_rows '
		,_fields
		,' from '
		,_sqlDataTable
		,CASE IFNULL(_strWhere, '') WHEN '' THEN '' ELSE CONCAT(' where ', _strWhere) END
		,CASE IFNULL(_strOrderBy, '') WHEN '' THEN '' ELSE CONCAT(' order by ', _strOrderBy) END
	  ,' limit ' 
		,@startRow
		,',' 
		,@pageSize
	);

	PREPARE strsql FROM @strsql;#定义预处理语句 
	EXECUTE strsql;							#执行预处理语句 
	DEALLOCATE PREPARE strsql;	#删除定义 
	#通过 sql_calc_found_rows 记录没有使用 limit 语句的记录，使用 found_rows() 获取行数
	SET _recordCount = FOUND_ROWS();
END;
DELIMITER ; #修改分隔符为分号(;)


