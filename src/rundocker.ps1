Write-Output "*****************************************"
docker-compose build

Write-Output "**********docker compile succeed*****************"

docker login -u development@30595107 registry.cn-shenzhen.aliyuncs.com -p password

Write-Output "**********login succeed*****************"

$images= Read-Host " input imagesname "
$tag=Read-Host "input tagname "

docker tag $images registry.cn-shenzhen.aliyuncs.com/NetProkou/NetPropublic:$tag

Write-Output '*************set tag succeed*************'

docker push registry.cn-shenzhen.aliyuncs.com/NetProkou/NetPropublic:$tag

Write-Output '*************docker push succeed*************'

docker rmi $(docker images -f "dangling=true" -q) 

Write-Output '*************delete non images succeed*************'