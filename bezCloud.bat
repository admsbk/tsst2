@echo off

echo Wielkosc sieci:

echo ClientNode
set/p "ClientNode=>>"
echo NetworkNode
set/p "NetworkNode=>>"


START NetworkManager\NetworkManager\bin\Debug\NetworkManager.exe Config/ManagerConfig.xml
START SubNetwork\SubNetwork\bin\Debug\SubNetwork.exe Config/DomainController/DC1.xml Config/NetworkTopology.xml

pause

for /L %%A in (1,1,%ClientNode%) do (
	START ClientNode\ClientNode\bin\Debug\ClientNode.exe Config/Client/Client%%A.xml
	)

for /L %%A in (1,1,%NetworkNode%) do (
	START NetworkNode\NetworkNode\bin\Debug\NetworkNode.exe Config/Nodes/Node%%A.xml
	)




end