@echo off

set clientUrl="https://n6bdo6uf3kz4zy4/svn/FirClient"
set clientPath="E:\workspace\ProjectOne\FirClient"
set dataUrl="https://n6bdo6uf3kz4zy4/svn/DataTable"
set editorUrl="https://n6bdo6uf3kz4zy4/svn/ClientEditor"
set serverUrl="https://n6bdo6uf3kz4zy4/svn/FirServer"
set toolPath="E:\workspace\ProjectOne\Tools\Bin"


set user="admin"
set passwd="admin888"

::更新代码工程
cd %clientPath%
if exist %clientPath%\Assets (
	svn up %clientPath% --username %user% --password %passwd% --force
) else (
	svn checkout %clientUrl%FirClient %clientPath% --username %user% --password %passwd% --force
)

::编辑器资源合并到代码工程
rd/s /q %clientPath%\Assets\res
svn export %editorUrl%/Assets/res %clientPath%/Assets/res --username %user% --password %passwd% --force

::更新编辑器工程里面公用的代码
rd/s /q %clientPath%\Assets\Scripts\Common
svn export %editorUrl%/Assets/Scripts/Common %clientPath%/Assets/Scripts/Common --username %user% --password %passwd% --force


::更新编辑器工程里面VIEW的代码
rd/s /q %clientPath%\Assets\Scripts\UI\View
svn export %editorUrl%/Assets/Scripts/View %clientPath%/Assets/Scripts/UI/View --username %user% --password %passwd% --force

::更新编辑器工程里面的配置文件
rd/s /q %clientPath%\ProjectSettings\TagManager.asset
svn export %editorUrl%/ProjectSettings/TagManager.asset %clientPath%/ProjectSettings/TagManager.asset --username %user% --password %passwd% --force

rd/s /q %clientPath%\ProjectSettings\QualitySettings.asset
svn export %editorUrl%/ProjectSettings/QualitySettings.asset %clientPath%/ProjectSettings/QualitySettings.asset --username %user% --password %passwd% --force

::更新EXCEL配置表
rd/s /q %clientPath%\Assets\DataTable
svn export %dataUrl% %clientPath%\Assets\DataTable --username %user% --password %passwd% --force

::编译数据表
rd/s /q %clientPath%\Assets\Scripts\Tables\*.*
%toolPath%\TableTool.exe %toolPath% %clientPath%\Assets\DataTable %clientPath%\Assets\Scripts\Tables\ 

echo "更新完成!"
pause  