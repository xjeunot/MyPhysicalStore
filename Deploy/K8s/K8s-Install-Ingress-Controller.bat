@ECHO OFF
REM ---------------------------------------------------------------------
REM ------------------------- MyPhysicalStore ---------------------------
REM ---------------------------------------------------------------------
REM ----- Manage Install Ingress Controller into Kubernetes Cluster -----
REM ----- Version				2019-06-19						---------
REM ---------------------------------------------------------------------

rem Verify Helm/Tiller Installation.
IF NOT EXIST %UserProfile%/.helm (
	ECHO Error : No Helm/Tiller Install Detected !
	GOTO END
)

rem Install.
if "%~1"=="--install" (

	rem Install Helm Nginx Ingress Package.
	ECHO "#################### Install Helm Nginx Ingress Package ####################"
	helm repo add nginx-stable https://helm.nginx.com/stable
	helm repo update

	rem Install Nginx Ingress.
	ECHO "#################### Install Nginx Ingress ####################"
	helm install nginx-stable/nginx-ingress --name psa-ingress-controller --set controller.service.externalIPs="{192.168.137.21,192.168.137.22}"

	GOTO END
)

rem Remove.
if "%~1"=="--remove" (

	rem Remove Nginx Ingress.
	ECHO "#################### Remove Nginx Ingress ####################"
	helm del --purge psa-ingress-controller

	GOTO END
)

rem Usage.
ECHO Usage :
ECHO	--install : Install NgInx Ingress Controller
ECHO	--remove  : Remove  NgInx Ingress Controller

rem End.
:END
