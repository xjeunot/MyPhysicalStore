ECHO OFF

ECHO "-----------------------------------------------------------"
ECHO "--------------------- MyPhysicalStore ---------------------"
ECHO "-----------------------------------------------------------"
ECHO " Prepare Kubernete Cluster with/without Azure				 "
ECHO " Version				2019-05-28							 "
ECHO "-----------------------------------------------------------"

rem Install Helm.
ECHO "#################### Install Helm ####################"
cd %UserProfile%
rmdir .helm /S /Q
helm init

rem Install Nginx Ingress Package.
ECHO "#################### Install Nginx Ingress Package ####################"
helm repo add nginx-stable https://helm.nginx.com/stable
helm repo update
