rem -----------------------------------------------------------
rem --------------------- MyPhysicalStore ---------------------
rem -----------------------------------------------------------
rem Build & Run Execution Kubernete Cluster with/without Azure
rem Version				2019-05-07
rem -----------------------------------------------------------

ECHO OFF

rem Configuration.
SET REPO_SRC=C:\_DevPerso\Apps\MyPhysicalStore
SET REGISTRY=xjeunot59
SET VERSION=latest

rem Open Work Directory.
cd %REPO_SRC%

rem Build.
docker-compose build

rem Docker Tag.
docker tag psa_storeapi:%VERSION% %REGISTRY%/psa_storeapi:%VERSION%
docker tag psa_customerapi:%VERSION% %REGISTRY%/psa_customerapi:%VERSION%
docker tag psa_identityapi:%VERSION% %REGISTRY%/psa_identityapi:%VERSION%
docker tag psa_ocelotapigw:%VERSION% %REGISTRY%/psa_ocelotapigw:%VERSION%
docker tag psa_aspmvcweb:%VERSION% %REGISTRY%/psa_aspmvcweb:%VERSION%
docker tag psa_spaweb:%VERSION% %REGISTRY%/psa_spaweb:%VERSION%

rem Docker Push.
docker push %REGISTRY%/psa_storeapi:%VERSION%
docker push %REGISTRY%/psa_customerapi:%VERSION%
docker push %REGISTRY%/psa_identityapi:%VERSION%
docker push %REGISTRY%/psa_ocelotapigw:%VERSION%
docker push %REGISTRY%/psa_aspmvcweb:%VERSION%
docker push %REGISTRY%/psa_spaweb:%VERSION%

rem Cleaning up old deployment.
ECHO "#################### Cleaning up old deployment ####################"
kubectl delete deployments --all
kubectl delete services --all
kubectl delete secret --all
kubectl delete configmap psa-test-config-files

rem Deploying Configuration Map.
echo "#################### Deploying Configuration Map ####################"
kubectl create configmap psa-test-config-files --from-file=Configs-Secrets\k8s-config-files.yaml
kubectl label configmap psa-test-config-files app=my-physical-store env=test

rem Deploying Secret.
echo "#################### Deploying Secret ####################"
kubectl create secret generic secret-store-api --from-file=Configs-Secrets\k8s-secret-store-api.json
kubectl create secret generic secret-customer-api --from-file=Configs-Secrets\k8s-secret-customer-api.json
kubectl create secret generic secret-identity-api --from-file=Configs-Secrets\k8s-secret-identity-api.json
kubectl create secret generic secret-ocelot-api-gw --from-file=Configs-Secrets\k8s-secret-ocelot-api-gw.json
kubectl create secret generic secret-aspmvc-web --from-file=Configs-Secrets\k8s-secret-aspmvc-web.json
kubectl create secret generic secret-front --from-file=Configs-Secrets\k8s-secret-dev.docker.local.json

rem Creating application service definitions.
echo "#################### Creating application service definitions ####################"
kubectl create -f Service\store-api-service.yaml
kubectl create -f Service\customer-api-service.yaml
kubectl create -f Service\identity-api-service.yaml
kubectl create -f Service\ocelot-api-gw-service.yaml
kubectl create -f Service\aspmvc-web-service.yaml
kubectl create -f Service\spa-web-service.yaml

rem Create application pod deployments.
echo "#################### Create application pod deployments ####################"
kubectl create -f Deployment\store-api-deployment.yaml
kubectl create -f Deployment\customer-api-deployment.yaml
kubectl create -f Deployment\identity-api-deployment.yaml
kubectl create -f Deployment\ocelot-api-gw-deployment.yaml
kubectl create -f Deployment\aspmvc-web-deployment.yaml
kubectl create -f Deployment\spa-web-deployment.yaml
