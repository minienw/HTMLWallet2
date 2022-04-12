docker build -t checkinqrweb:latest .
#docker tag airline_stub:latest stevekellaway/airline_stub:latest
#docker push stevekellaway/airline_stub:latest
#docker run -p 80:80/tcp -p 443:443/tcp --name CheckInQrWeb checkinqrweb:latest
docker run -p 8082:8082/tcp --name CheckInQrWeb checkinqrweb:latest

