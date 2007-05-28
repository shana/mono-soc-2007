#! /bin/bash
for x in  *
do
        if [ -e ".svn" ]
        then
               rm -rf .svn 
        fi
                
        if [ -d "$x" ]
	then 
	        cd $x
		for j in *
                do
                        if [ -e ".svn" ]
                        then
                                rm -rf .svn
                        fi

                        if [ -d "$j" ]
	                then 
	                        cd $j
		                for k in *
                                do
                                        if [ -e ".svn" ]
                                        then
                                                rm -rf .svn
                                        fi

                                        if [ -d "$k" ]
	                                then 
	                                        cd $k
		                                for l in *
                                                do
                                                        if [ -e ".svn" ]
                                                        then
                                                                rm -rf .svn
                                                        fi

                                                        if [ -d "$l" ]
                                                        then
                                                                cd $l
                                                                for m in *
                                                                do
                                                                        if [ -e ".svn" ]
                                                                        then
                                                                                rm -rf .svn
                                                                        fi
                                                                        
                                                                        if [ -d "$m" ]
                                                                        then
                                                                                cd $m
                                                                                for n in *
                                                                                do
                                                                                        if [ -e ".svn" ]
                                                                                        then
                                                                                                rm -rf .svn
                                                                                        fi
                                                                                done
                                                                                cd ..
                                                                        fi
                                                                done
                                                                cd ..
                                                        fi
                                                done
		                                cd ..
                                        fi
                                done
		                cd ..
                        fi
                done
		cd ..
        fi
done
