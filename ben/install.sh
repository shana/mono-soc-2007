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
