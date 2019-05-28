ECHO OFF

ECHO "-----------------------------------------------------------"
ECHO "--------------------- MyPhysicalStore ---------------------"
ECHO "-----------------------------------------------------------"
ECHO " Build WebSPA												 "
ECHO " Version				2019-05-28							 "
ECHO "-----------------------------------------------------------"

rem Configuration.
SET REPO_SRC=C:\_DevPerso\Apps\MyPhysicalStore

rem Open Work Directory.
cd %REPO_SRC%\Web\WebSPA

rem Build.
ECHO "#################### Build ####################"
ng build --prod --base-href /WebSPA/

rem End.
ECHO "#################### End ####################"
