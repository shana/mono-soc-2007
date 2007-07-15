#! /bin/sh

DIR=/usr/lib/monodevelop/AddIns/MonoDevelop.Database
rm -rf $DIR
mkdir -p $DIR
echo $DIR

cp ./contrib/QuickGraph.dll $DIR/

find ./MonoDevelop.Database -type f -iname "*.dll" -or -iname "*.addin.xml" | while read a; do
	cp -f "$a" $DIR
#	echo "$a"
done


# temp remove some addins
rm $DIR/Mono.Data.Sql.Firebird.addin.xml
rm $DIR/Mono.Data.Sql.Odbc.addin.xml
rm $DIR/Mono.Data.Sql.Oracle.addin.xml
rm $DIR/Mono.Data.Sql.Sybase.addin.xml
rm $DIR/MonoDevelop.Database.Visualization.addin.xml
