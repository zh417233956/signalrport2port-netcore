```sh
# 构建
export GITHASH=`git rev-parse --short HEAD`
docker build -f Web/Dockerfile -t registry.cn-beijing.aliyuncs.com/syzh/signalr:$GITHASH .
docker tag registry.cn-beijing.aliyuncs.com/syzh/signalr:$GITHASH registry.cn-beijing.aliyuncs.com/syzh/signalr:latest
docker push registry.cn-beijing.aliyuncs.com/syzh/signalr:$GITHASH
docker push registry.cn-beijing.aliyuncs.com/syzh/signalr:latest

# 发布
# docker images|grep none|awk '{print $3}'|xargs docker rmi
docker pull registry.cn-beijing.aliyuncs.com/syzh/signalr:latest
cd /opt/work/signalr_demo
docker-compose down
docker-compose up -d
```