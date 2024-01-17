set -e



#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn

export NUGET_SERVER=https://api.nuget.org/v3/index.json
export NUGET_KEY=xxxxxxxxxx

# "



#----------------------------------------------
echo "72.nuget-push.sh"

docker run -i --rm \
--env LANG=C.UTF-8 \
-v $basePath:/root/code \
serset/dotnet:sdk-6.0 \
bash -c "


if [ ! -d \"/root/code/Publish/release/release/nuget\" ]; then
    echo '72.nuget-push.sh -> skip for no nuget files exist'
    exit 0
fi


for file in /root/code/Publish/release/release/nuget/*.nupkg
do
    echo nuget push \$file
    dotnet nuget push \$file -k ${NUGET_KEY} -s ${NUGET_SERVER} --skip-duplicate
done
" || true


 