@ECHO OFF
REM ---------------------------------------------------------------------
REM ------------------------- MyPhysicalStore ---------------------------
REM ---------------------------------------------------------------------
REM ----- Manage Install Application into Kubernetes Cluster ------------
REM ----- Version				2019-06-19						---------
REM ---------------------------------------------------------------------

rem Install.
if "%~1"=="--install" (

	rem Setup IngressFile.
	if "%~2"=="BareMetal" (
		SET IngressFile=k8s-psa-ing-br.yaml
	) ELSE (
		GOTO USAGE
	)

	rem Create Namespaces.
	echo #################### Create Namespaces ####################
	kubectl create -f Namespace/psa-dev-namespace.yaml

	rem Deploying Configuration Map.
	echo #################### Deploying Configuration Map ####################
	kubectl -n psa-dev create configmap psa-dev-config-files --from-env-file=Configs-Secrets/k8s-config-files.properties
	kubectl -n psa-dev label configmap psa-dev-config-files app=my-physical-store env=dev

	rem Deploying Secret.
	echo #################### Deploying Secret ####################
	kubectl -n psa-dev create secret generic secret-store-api --from-file=Configs-Secrets/k8s-secret-store-api.json
	kubectl -n psa-dev create secret generic secret-customer-api --from-file=Configs-Secrets/k8s-secret-customer-api.json
	kubectl -n psa-dev create secret generic secret-identity-api --from-file=Configs-Secrets/k8s-secret-identity-api.json
	kubectl -n psa-dev create secret generic secret-ocelot-api-gw --from-file=Configs-Secrets/k8s-secret-ocelot-api-gw.json
	kubectl -n psa-dev create secret generic secret-ocelot-api-gw-spa --from-file=Configs-Secrets/k8s-secret-ocelot-api-gw-spa.json
	kubectl -n psa-dev create secret generic secret-aspmvc-web --from-file=Configs-Secrets/k8s-secret-aspmvc-web.json
	kubectl -n psa-dev create secret tls psa-dev-local-secret --key Configs-Secrets/psa-dev.local.key --cert Configs-Secrets/psa-dev.local.crt

	rem Creating application service definitions.
	echo #################### Creating application service definitions ####################
	kubectl -n psa-dev create -f Service/store-api-service.yaml
	kubectl -n psa-dev create -f Service/customer-api-service.yaml
	kubectl -n psa-dev create -f Service/identity-api-service.yaml
	kubectl -n psa-dev create -f Service/ocelot-api-gw-service.yaml
	kubectl -n psa-dev create -f Service/ocelot-api-gw-spa-service.yaml
	kubectl -n psa-dev create -f Service/aspmvc-web-service.yaml
	kubectl -n psa-dev create -f Service/spa-web-service.yaml

	rem Create application deployments.
	echo #################### Create application pod deployments ####################
	kubectl -n psa-dev create -f Deployment/store-api-deployment.yaml
	kubectl -n psa-dev create -f Deployment/customer-api-deployment.yaml
	kubectl -n psa-dev create -f Deployment/identity-api-deployment.yaml
	kubectl -n psa-dev create -f Deployment/ocelot-api-gw-deployment.yaml
	kubectl -n psa-dev create -f Deployment/ocelot-api-gw-spa-deployment.yaml
	kubectl -n psa-dev create -f Deployment/aspmvc-web-deployment.yaml
	kubectl -n psa-dev create -f Deployment/spa-web-deployment.yaml

	rem Execute rollout.
	echo #################### Rollout application pods deployments ####################
	kubectl -n psa-dev rollout resume deployments/store-api-deploy
	kubectl -n psa-dev rollout resume deployments/customer-api-deploy
	kubectl -n psa-dev rollout resume deployments/identity-api-deploy
	kubectl -n psa-dev rollout resume deployments/ocelot-api-gw-deploy
	kubectl -n psa-dev rollout resume deployments/ocelot-api-gw-spa-deploy
	kubectl -n psa-dev rollout resume deployments/aspmvc-web-deploy
	kubectl -n psa-dev rollout resume deployments/spa-web-deploy

	rem Deploying Ingress.
	echo #################### Deploying Ingress ####################
	kubectl -n psa-dev create -f  .\Ingress\%IngressFile%

	GOTO END
)

rem Remove.
if "%~1"=="--remove" (

	rem Cleaning up old deployment.
	ECHO #################### Cleaning up old deployment ####################
	kubectl -n psa-dev delete ingress --all
	kubectl -n psa-dev delete deployments --all
	kubectl -n psa-dev delete services --all
	kubectl -n psa-dev delete secret --all
	kubectl -n psa-dev delete configmap --all

	rem Remove Namespaces.
	echo #################### Remove Namespaces ####################
	kubectl delete namespaces psa-dev

	GOTO END
)

:USAGE
rem Usage.
ECHO Usage :
ECHO   --install BareMetal : Install Psa Application on Metal-Bare Serveur.
ECHO   --remove            : Remove  Psa Application

rem End.
:END
