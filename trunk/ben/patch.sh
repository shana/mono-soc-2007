#! /bin/sh

svn cat https://mono-soc-2007.googlecode.com/svn/trunk/ben/configure.patch > temp.patch
patch -p0 -i temp.patch
rm -f temp.patch

cd Extras/

svn cat https://mono-soc-2007.googlecode.com/svn/trunk/ben/makefile.patch > temp.patch
patch -p0 -i temp.patch

svn cat https://mono-soc-2007.googlecode.com/svn/trunk/ben/solution.patch > temp.patch
patch -p0 -i temp.patch
rm -f temp.patch

svn co https://mono-soc-2007.googlecode.com/svn/trunk/ben/MonoDevelop.Database MonoDevelop.Database
cd ..

cd contrib
svn cat https://mono-soc-2007.googlecode.com/svn/trunk/ben/contrib/MySql.Data.dll > MySql.Data.dll
svn cat https://mono-soc-2007.googlecode.com/svn/trunk/ben/contrib/FirebirdSql.Data.Firebird.dll > FirebirdSql.Data.Firebird.dll
svn cat https://mono-soc-2007.googlecode.com/svn/trunk/ben/contrib/FirebirdSql.Data.Firebird.license.txt > FirebirdSql.Data.Firebird.license.txt

cd ..

echo "patched!"
echo "use --enable-database when running configure"
