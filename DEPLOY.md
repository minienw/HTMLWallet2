# Pre-requisites
Experience with git, docker and docker compose command line. For each individual service, experience with either ASP.NET or Springboot will be required to configure the component.

# System
The system comprises of 3 modules:

Wallet – standalone web application.
Validation service - including a DCC verifier and a Redis cache.
Airline stub – services and website to demonstrate the workflow of the above 2 components.
The system can be deployed as 3 docker containers using the images and process below.

# Images
Publicly available on https://hub.docker.com/r/stevekellaway:
* Verifier:latest – image used in validation_service composed container.
* validation_service:latest_noconfig – base image without configuration
* wallet:latest – deployable image
* airline_stub:latest_noconfig  – base image without configuration

# Building and Running Images
The Verifier and Wallet images are ready for deployment.

For Validation Service and Airline Stub:

1. Create an identity.json and application.properties file following the instructions in the configuration document. 
2. Replace the 2 files in the ‘deploy’ folder and run the ‘docker build’ command from the file ‘build_and_run_docker.sh’ to create the deployable image with tag ‘pub_latest’. 
3. Run the docker-compose file in the ‘deploy’ folder to create the container. 

For the wallet:

Run the image in Docker, optionally with the environment variable ASPNETCORE_ENVIRONMENT set to a value of ‘Development’ to show debug information on the Process page, otherwise leave blank.









11 May 2022
13:02


Windows - Install WSL (we used Ubuntu 20.04)
Install AZ command line
Docker Desktop and Compose

az login
export KUBECONFIG=/mnt/d/Users/ms1/.kube/config

2. Create a resource group in which to install the k8s clusters.

cd… pick one of the 

./kubectl_login.sh 

cd /mnt/d/Repos/MinIenW/validationservice/azurekubernetes
./delete_aks.sh
cd /mnt/d/Repos/MinIenW/airlinestub/azurekubernetes
./delete_aks.sh
cd /mnt/d/Repos/MinIenW/fixit/HTMLWallet2/CheckinQrWeb/azurekubernetes
./delete_aks.sh

//Reverse order
./setup_aks.sh
./kubectl_login.sh
./deploy_aks.sh

cd /mnt/d/Repos/MinIenW/airlinestub/azurekubernetes
./setup_aks.sh
./kubectl_login.sh
./deploy_aks.sh

cd /mnt/d/Repos/MinIenW/validationservice/azurekubernetes
./setup_aks.sh
./kubectl_login.sh
./deploy_aks.sh
