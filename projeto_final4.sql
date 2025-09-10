-- Verificar todas as constraints da tabela Venda
SELECT 
    name AS ConstraintName,
    type_desc AS ConstraintType,
    is_disabled AS IsDisabled
FROM sys.foreign_keys 
WHERE parent_object_id = OBJECT_ID('Venda');

-- Ver detalhes da FOREIGN KEY
SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTableName,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumnName,
    fk.is_disabled AS IsDisabled
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) = 'Venda';