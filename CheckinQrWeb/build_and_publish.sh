docker build -t wallet:latest .
docker tag wallet:latest wallet:pub_latest
docker tag wallet:latest stevekellaway/wallet:pub_latest
docker push stevekellaway/wallet:pub_latest
#docker run -p 80:80/tcp -p 443:443/tcp --name CheckInQrWeb checkinqrweb:latest
#docker run -p 8082:8082/tcp --name CheckInQrWeb checkinqrweb:latest
