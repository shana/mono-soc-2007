#! /bin/sh

DIR=/usr/lib/monodevelop/AddIns/MonoDevelop.Database
rm -rf $DIR
mkdir -p $DIR
echo $DIR

cp ./contrib/QuickGraph.dll $DIR/
cp ./contrib/Mono.Data.Sqlite.dll $DIR/

find ./MonoDevelop.Database -type f -iname "*.dll" -or -iname "*.addin.xml" | while read a; do
	cp -f "$a" $DIR
#	echo "$a"
done


# temp remove some addins
rm $DIR/MonoDevelop.Database.Sql.Firebird.addin.xml
rm $DIR/MonoDevelop.Database.Sql.Odbc.addin.xml
rm $DIR/MonoDevelop.Database.Sql.Oracle.addin.xml
rm $DIR/MonoDevelop.Database.Sql.Sybase.addin.xml
rm $DIR/MonoDevelop.Database.Visualization.addin.xml
rm $DIR/MonoDevelop.Database.Project.addin.xml
rm $DIR/MonoDevelop.Database.GlueGenerator.addin.xml

rm $DIR/MonoDevelop.Database.Sql.Firebird.dll
rm $DIR/MonoDevelop.Database.Sql.Odbc.dll
rm $DIR/MonoDevelop.Database.Sql.Oracle.dll
rm $DIR/MonoDevelop.Database.Sql.Sybase.dll
rm $DIR/MonoDevelop.Database.Visualization.dll
rm $DIR/MonoDevelop.Database.Project.dll
rm $DIR/MonoDevelop.Database.GlueGenerator.dll
