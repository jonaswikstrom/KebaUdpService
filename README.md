# KebaUdpService
A .NET Core based application used for reading data from Keba Chargers (KeContact P20/P30) towards MQTT.

## Configurations
Configure is done in appsettings.json file:

## Topics
| Value|Topic|Description|
|-------------|-------------|-------------|
|Chargingstate:|/chargingstate|State of charging|
|Plugstate:|/plugstate|State of the plug|
|Epres:|/epres|Value in kWh of the power consumption for the current charging session|
|Etotal:|/etotal|Value in kWh of the power consumption for the charging station including the current charging session|
|Power:|/power|Value in W of the current energy consumption|






