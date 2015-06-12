@echo off

echo Wielkosc sieci:

echo ClientNode
set/p "ClientNode=>>"
echo NetworkNode
set/p "NetworkNode=>>"

START SubNetwork\SubNetwork\bin\Debug\SubNetwork.exe Config/Przeciazenie/NetworkTopologyPrzeciazenie.xml Config/DomainController/DC1.xml
START networkLibrary\Client\bin\Debug\Client.exe Config/Przeciazenie/NetworkTopologyPrzeciazenie.xml
START NetworkManager\NetworkManager\bin\Debug\NetworkManager.exe Config/ManagerConfig.xml

pause
for /L %%A in (1,1,%NetworkNode%) do (
	START NetworkNode\NetworkNode\bin\Debug\NetworkNode.exe Config/Przeciazenie/Node%%A.xml
	)
	
for /L %%A in (1,1,%ClientNode%) do (
	START ClientNode\ClientNode\bin\Debug\ClientNode.exe Config/Przeciazenie/Client%%A.xml
	)






end