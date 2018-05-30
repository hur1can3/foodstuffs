#! /bin/bash

docker build --build-arg env=Staging -t foodstuffs-staging .
docker run -it --rm -p 3333:80 --name foodstuffs-prod foodstuffs-staging