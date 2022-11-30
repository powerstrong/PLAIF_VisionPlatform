rem test code
rem cd C:\Users\power\source\repos\PLAIF_VisionPlatform\PLAIF_VisionPlatform\Linux bash

IF [%1] == [] (set IP="192.158.1.75:9090") ELSE (set IP=%1)
echo %IP%
