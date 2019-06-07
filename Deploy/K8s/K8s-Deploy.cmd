ECHO OFF

ECHO "-----------------------------------------------------------"
ECHO "--------------------- MyPhysicalStore ---------------------"
ECHO "-----------------------------------------------------------"
ECHO "Build & Run Execution Kubernete Cluster with/without Azure"
ECHO "Version				2019-05-28"
ECHO "-----------------------------------------------------------"

rem Configuration.
SET REPO_SRC=C:\_DevPerso\Apps\MyPhysicalStore
SET REGISTRY=xjeunot59
SET VERSION=latest

rem Open Work Directory.
cd %REPO_SRC%\Deploy\K8s

rem Cleaning up old deployment.
ECHO "#################### Cleaning up old deployment ####################"
kubectl -n psa-dev delete ingress --all
kubectl -n psa-dev delete deployments --all
kubectl -n psa-dev delete services --all
kubectl -n psa-dev delete secret --all
kubectl -n psa-dev delete configmap psa-dev-config-files

rem Cleaning Helm.
ECHO "#################### Cleaning Helm ####################"
helm del --purge psa-dev-ing
helm delete psa-dev-ing

rem Remove Namespaces.
echo "#################### Remove Namespaces ####################"
kubectl delete namespaces psa-dev

rem By-Pass if Args '--no-build'.
if "%~1"=="--no-build" (
	TIMEOUT 50
	goto NO_BUILD
)

rem Open Work Directory.
cd %REPO_SRC%

rem Docker Build Images.
echo "#################### Docker Build Images ####################"
docker-compose build

rem Docker Tag Images.
echo "#################### Docker Tag Images ####################"
docker tag psa_storeapi:%VERSION% %REGISTRY%/psa_storeapi:%VERSION%
docker tag psa_customerapi:%VERSION% %REGISTRY%/psa_customerapi:%VERSION%
docker tag psa_identityapi:%VERSION% %REGISTRY%/psa_identityapi:%VERSION%
docker tag psa_ocelotapigw:%VERSION% %REGISTRY%/psa_ocelotapigw:%VERSION%
docker tag psa_ocelotapigwspa:%VERSION% %REGISTRY%/psa_ocelotapigwspa:%VERSION%
docker tag psa_aspmvcweb:%VERSION% %REGISTRY%/psa_aspmvcweb:%VERSION%
docker tag psa_spaweb:%VERSION% %REGISTRY%/psa_spaweb:%VERSION%

rem Docker Push Images.
echo "#################### Docker Push Images ####################"
docker push %REGISTRY%/psa_storeapi:%VERSION%
docker push %REGISTRY%/psa_customerapi:%VERSION%
docker push %REGISTRY%/psa_identityapi:%VERSION%
docker push %REGISTRY%/psa_ocelotapigw:%VERSION%
docker push %REGISTRY%/psa_ocelotapigwspa:%VERSION%
docker push %REGISTRY%/psa_aspmvcweb:%VERSION%
docker push %REGISTRY%/psa_spaweb:%VERSION%

rem Open Work Directory.
cd %REPO_SRC%\Deploy\K8s

rem Label NO_BUILD.
:NO_BUILD

rem Create Namespaces.
echo "#################### Create Namespaces ####################"
kubectl create -f ./psa-dev-namespace.yaml

rem Install Nginx Ingress.
ECHO "#################### Install Nginx Ingress ####################"
helm install nginx-stable/nginx-ingress --name psa-dev-ing --namespace psa-dev

rem Deploying Configuration Map.
echo "#################### Deploying Configuration Map ####################"
kubectl -n psa-dev create configmap psa-dev-config-files --from-env-file=Configs-Secrets/k8s-config-files.properties
kubectl -n psa-dev label configmap psa-dev-config-files app=my-physical-store env=dev

rem Deploying Secret.
echo "#################### Deploying Secret ####################"
kubectl -n psa-dev create secret generic secret-store-api --from-file=Configs-Secrets/k8s-secret-store-api.json
kubectl -n psa-dev create secret generic secret-customer-api --from-file=Configs-Secrets/k8s-secret-customer-api.json
kubectl -n psa-dev create secret generic secret-identity-api --from-file=Configs-Secrets/k8s-secret-identity-api.json
kubectl -n psa-dev create secret generic secret-ocelot-api-gw --from-file=Configs-Secrets/k8s-secret-ocelot-api-gw.json
kubectl -n psa-dev create secret generic secret-ocelot-api-gw-spa --from-file=Configs-Secrets/k8s-secret-ocelot-api-gw-spa.json
kubectl -n psa-dev create secret generic secret-aspmvc-web --from-file=Configs-Secrets/k8s-secret-aspmvc-web.json
kubectl -n psa-dev create secret tls psa-dev-local-secret --key Configs-Secrets/psa-dev.local.key --cert Configs-Secrets/psa-dev.local.crt

rem Creating application service definitions.
echo "#################### Creating application service definitions ####################"
kubectl -n psa-dev create -f Service/store-api-service.yaml
kubectl -n psa-dev create -f Service/customer-api-service.yaml
kubectl -n psa-dev create -f Service/identity-api-service.yaml
kubectl -n psa-dev create -f Service/ocelot-api-gw-service.yaml
kubectl -n psa-dev create -f Service/ocelot-api-gw-spa-service.yaml
kubectl -n psa-dev create -f Service/aspmvc-web-service.yaml
kubectl -n psa-dev create -f Service/spa-web-service.yaml

rem Create application pod deployments.
echo "#################### Create application pod deployments ####################"
kubectl -n psa-dev create -f Deployment/store-api-deployment.yaml
kubectl -n psa-dev create -f Deployment/customer-api-deployment.yaml
kubectl -n psa-dev create -f Deployment/identity-api-deployment.yaml
kubectl -n psa-dev create -f Deployment/ocelot-api-gw-deployment.yaml
kubectl -n psa-dev create -f Deployment/ocelot-api-gw-spa-deployment.yaml
kubectl -n psa-dev create -f Deployment/aspmvc-web-deployment.yaml
kubectl -n psa-dev create -f Deployment/spa-web-deployment.yaml

rem Execute rollout.
echo "#################### Rollout application pods deployments ####################"
kubectl -n psa-dev rollout resume deployments/store-api-deploy
kubectl -n psa-dev rollout resume deployments/customer-api-deploy
kubectl -n psa-dev rollout resume deployments/identity-api-deploy
kubectl -n psa-dev rollout resume deployments/ocelot-api-gw-deploy
kubectl -n psa-dev rollout resume deployments/ocelot-api-gw-spa-deploy
kubectl -n psa-dev rollout resume deployments/aspmvc-web-deploy
kubectl -n psa-dev rollout resume deployments/spa-web-deploy

rem Deploying Ingress.
echo "#################### Deploying Ingress ####################"
kubectl -n psa-dev create -f  .\Ingress\k8s-psa-dev-ing.yaml

rem End.
echo "#################### End ####################"
