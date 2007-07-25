ADDIN_BUILD = $(top_builddir)/build/AddIns/MonoDevelop.Database
ASSEMBLY = $(ADDIN_BUILD)/MonoDevelop.Database.Sql.dll
REFS =  \
	-pkg:monodevelop \
	-r:../../contrib/Mono.Addins.dll \
	-r:System \
	-r:System.Data \
	-r:System.Xml

FILES =  \
	Codons/DbFactoryCodon.cs \
	Collections/ColumnSchemaCollection.cs \
	Collections/ConstraintSchemaCollection.cs \
	Collections/DatabaseSchemaCollection.cs \
	Collections/DataTypeSchemaCollection.cs \
	Collections/IndexSchemaCollection.cs \
	Collections/IPropertyComparer.cs \
	Collections/ParameterSchemaCollection.cs \
	Collections/ProcedureSchemaCollection.cs \
	Collections/SortedCollectionBase.cs \
	Collections/SortedCollectionItemEventArgs.cs \
	Collections/TableSchemaCollection.cs \
	Collections/TriggerSchemaCollection.cs \
	Collections/UserSchemaCollection.cs \
	Collections/ViewSchemaCollection.cs \
	Configuration/DatabaseConnectionContext.cs \
	Configuration/DatabaseConnectionContextCollection.cs \
	Configuration/DatabaseConnectionContextEventArgs.cs \
	Configuration/DatabaseConnectionSettings.cs \
	Configuration/DatabaseConnectionSettingsCollection.cs \
	ConnectionContextService.cs \
	ConnectionPool/AbstractPooledDbConnection.cs \
	ConnectionPool/DefaultConnectionPool.cs \
	ConnectionPool/FakeConnectionPool.cs \
	ConnectionPool/IConnectionPool.cs \
	ConnectionPool/IPooledDbConnection.cs \
	ConnectionPool/PooledCommand.cs \
	ConnectionPool/PooledDataReader.cs \
	DbFactoryService.cs \
	Dialect/AbstractSqlDialect.cs \
	Dialect/Clauses/FromClause.cs \
	Dialect/Clauses/FromSelectClause.cs \
	Dialect/Clauses/FromTableClause.cs \
	Dialect/Clauses/GroupByClause.cs \
	Dialect/Clauses/HavingClause.cs \
	Dialect/Clauses/IClause.cs \
	Dialect/Clauses/JoinClause.cs \
	Dialect/Clauses/JoinClauseType.cs \
	Dialect/Clauses/OrderByClause.cs \
	Dialect/Clauses/UnionClause.cs \
	Dialect/Clauses/WhereClause.cs \
	Dialect/Expressions/AliasedIdentifierExpression.cs \
	Dialect/Expressions/BooleanExpression.cs \
	Dialect/Expressions/BooleanOperator.cs \
	Dialect/Expressions/IConditionalExpression.cs \
	Dialect/Expressions/IdentifierExpression.cs \
	Dialect/Expressions/IExpression.cs \
	Dialect/Expressions/Operator.cs \
	Dialect/Expressions/OperatorExpression.cs \
	Dialect/Expressions/ParameterExpression.cs \
	Dialect/ISqlDialect.cs \
	Dialect/Literals/BitLiteral.cs \
	Dialect/Literals/BooleanLiteral.cs \
	Dialect/Literals/FalseLiteral.cs \
	Dialect/Literals/HexLiteral.cs \
	Dialect/Literals/ILiteral.cs \
	Dialect/Literals/NullLiteral.cs \
	Dialect/Literals/NumericLiteral.cs \
	Dialect/Literals/StringLiteral.cs \
	Dialect/Literals/TrueLiteral.cs \
	Dialect/Statements/DeleteStatement.cs \
	Dialect/Statements/DropStatement.cs \
	Dialect/Statements/DropStatementType.cs \
	Dialect/Statements/InsertStatement.cs \
	Dialect/Statements/IStatement.cs \
	Dialect/Statements/SelectStatement.cs \
	Dialect/Statements/TruncateStatement.cs \
	Dialect/Statements/UpdateStatement.cs \
	IDbFactory.cs \
	MetaData/CheckConstraintMetaData.cs \
	MetaData/ColumnMetaData.cs \
	MetaData/ConnectionSettingsMetaData.cs \
	MetaData/DataTypeMetaData.cs \
	MetaData/ForeignKeyConstraintMetaData.cs \
	MetaData/MetaDataService.cs \
	MetaData/OperationMetaData.cs \
	MetaData/ParameterMetaData.cs \
	MetaData/PrimaryKeyConstraintMetaData.cs \
	MetaData/ProcedureMetaData.cs \
	MetaData/SchemaMetaData.cs \
	MetaData/TableMetaData.cs \
	MetaData/TriggerMetaData.cs \
	MetaData/UniqueConstraintMetaData.cs \
	MetaData/UserMetaData.cs \
	MetaData/ViewMetaData.cs \
	Providers/AbstractConnectionProvider.cs \
	Providers/AbstractSchemaProvider.cs \
	Providers/IConnectionProvider.cs \
	Providers/ISchemaProvider.cs \
	Providers/SchemaOperation.cs \
	QueryService.cs \
	Schema/AbstractSchema.cs \
	Schema/AggregateSchema.cs \
	Schema/CheckConstraintSchema.cs \
	Schema/ColumnSchema.cs \
	Schema/ConstraintSchema.cs \
	Schema/ConstraintType.cs \
	Schema/DatabaseSchema.cs \
	Schema/DataTypeCategory.cs \
	Schema/DataTypeSchema.cs \
	Schema/ForeignKeyConstraintSchema.cs \
	Schema/GroupSchema.cs \
	Schema/IndexSchema.cs \
	Schema/IndexSortType.cs \
	Schema/ISchema.cs \
	Schema/LanguageSchema.cs \
	Schema/OperatorSchema.cs \
	Schema/ParameterSchema.cs \
	Schema/ParameterType.cs \
	Schema/PrimaryKeyConstraintSchema.cs \
	Schema/PrivilegeSchema.cs \
	Schema/ProcedureSchema.cs \
	Schema/Range.cs \
	Schema/RoleSchema.cs \
	Schema/RuleSchema.cs \
	Schema/SchemaSchema.cs \
	Schema/SequenceSchema.cs \
	Schema/TableSchema.cs \
	Schema/TriggerEvent.cs \
	Schema/TriggerSchema.cs \
	Schema/TriggerType.cs \
	Schema/UniqueConstraintSchema.cs \
	Schema/UserSchema.cs \
	Schema/ViewSchema.cs

ADDIN = MonoDevelop.Database.Sql.addin.xml

all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN)

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/.

$(ADDIN_BUILD)/%: $(srcdir)/lib/%
	mkdir -p $(ADDIN_BUILD)
	cp $< $@

$(ASSEMBLY): $(FILES) $(build_resources)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) $(CSC_FLAGS) $(REFS) $(build_resources:%=/resource:%) $(build_sources) -out:$@ -target:library


assemblydir = $(MD_ADDIN_DIR)/MonoDevelop.Database
assembly_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = MonoDevelop.Database.Sql.addin.xml

include $(top_srcdir)/Makefile.include
