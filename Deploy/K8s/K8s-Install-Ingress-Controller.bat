@ECHO OFF
REM ---------------------------------------------------------------------
REM ------------------------- MyPhysicalStore ---------------------------
REM ---------------------------------------------------------------------
REM ----- Manage Install Ingress Controller into Kubernetes Cluster -----
REM ----- Version				2019-12-03						---------
REM ---------------------------------------------------------------------

rem Install.
if "%~1"=="--install" (

	rem Install Helm Nginx Ingress Package.
	ECHO "#################### Install Helm Nginx Ingress Package ####################"
	helm repo add nginx-stable https://helm.nginx.com/stable
	helm repo update

	rem Install Nginx Ingress.
	ECHO "#################### Install Nginx Ingress ####################"
	helm install psa-ingress-controller nginx-stable/nginx-ingress --set controller.service.externalIPs="{192.168.137.22,192.168.137.23}"

	GOTO END
)

rem Remove.
if "%~1"=="--remove" (

	rem Remove Nginx Ingress.
	ECHO "#################### Remove Nginx Ingress ####################"
	helm uninstall psa-ingress-controller

	GOTO END
)

rem Usage.
ECHO Usage :
ECHO	--install : Install NgInx Ingress Controller
ECHO	--remove  : Remove  NgInx Ingress Controller

rem End.
:END
