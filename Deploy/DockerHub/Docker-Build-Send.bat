@ECHO OFF
REM ---------------------------------------------------------------------
REM ------------------------- MyPhysicalStore ---------------------------
REM ---------------------------------------------------------------------
REM ----- Build & Send Application into Docker Registry -----------------
REM ----- Version				2019-06-19						---------
REM ---------------------------------------------------------------------

rem Configuration.
SET DOCKER_REGISTRY=xjeunot59
SET DOCKER_VERSION=latest

rem Open Work Directory.
cd ..\..

rem Docker Build Images.
echo "#################### Docker Build Images ####################"
docker-compose build

rem Docker Tag Images.
echo "#################### Docker Tag Images ####################"
docker tag psa_storeapi:%DOCKER_VERSION% %DOCKER_REGISTRY%/psa_storeapi:%DOCKER_VERSION%
docker tag psa_customerapi:%DOCKER_VERSION% %DOCKER_REGISTRY%/psa_customerapi:%DOCKER_VERSION%
docker tag psa_identityapi:%DOCKER_VERSION% %DOCKER_REGISTRY%/psa_identityapi:%DOCKER_VERSION%
docker tag psa_ocelotapigw:%DOCKER_VERSION% %DOCKER_REGISTRY%/psa_ocelotapigw:%DOCKER_VERSION%
docker tag psa_ocelotapigwspa:%DOCKER_VERSION% %DOCKER_REGISTRY%/psa_ocelotapigwspa:%DOCKER_VERSION%
docker tag psa_aspmvcweb:%DOCKER_VERSION% %DOCKER_REGISTRY%/psa_aspmvcweb:%DOCKER_VERSION%
docker tag psa_spaweb:%DOCKER_VERSION% %DOCKER_REGISTRY%/psa_spaweb:%DOCKER_VERSION%

rem Docker Push Images.
echo "#################### Docker Push Images ####################"
docker push %DOCKER_REGISTRY%/psa_storeapi:%DOCKER_VERSION%
docker push %DOCKER_REGISTRY%/psa_customerapi:%DOCKER_VERSION%
docker push %DOCKER_REGISTRY%/psa_identityapi:%DOCKER_VERSION%
docker push %DOCKER_REGISTRY%/psa_ocelotapigw:%DOCKER_VERSION%
docker push %DOCKER_REGISTRY%/psa_ocelotapigwspa:%DOCKER_VERSION%
docker push %DOCKER_REGISTRY%/psa_aspmvcweb:%DOCKER_VERSION%
docker push %DOCKER_REGISTRY%/psa_spaweb:%DOCKER_VERSION%
