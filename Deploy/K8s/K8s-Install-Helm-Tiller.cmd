@ECHO OFF
REM --------------------------------------------------------------
REM ---------------------- MyPhysicalStore -----------------------
REM --------------------------------------------------------------
REM ----- Manage Install Helm/Tiller into Kubernetes Cluster -----
REM ----- Version				2019-06-19						--
REM --------------------------------------------------------------

rem Install.
if "%~1"=="--install" (

	rem Install Helm.
	ECHO "#################### Install Helm ####################"
	kubectl create serviceaccount tiller --namespace kube-system
	kubectl create -f Tiller-Helm\tiller-clusterrolebinding.yaml
	helm init --service-account tiller --upgrade

	GOTO END
)

rem Remove.
if "%~1"=="--remove" (
	rem Remove Helm.
	ECHO "#################### Remove Helm ####################"
	helm reset --force
	kubectl delete -f Tiller-Helm\tiller-clusterrolebinding.yaml
	kubectl delete serviceaccount tiller --namespace kube-system
	RM %UserProfile%/.helm -d -r

	GOTO END
)

rem Usage.
ECHO Usage :
ECHO	--install : Install Helm/Tiller
ECHO	--remove  : Remove  Helm/Tiller

rem End.
:END
