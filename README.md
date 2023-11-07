![The Eye Tribe](tet_logo.png)

Client for Eye Tribe Tracker
====

This repo handles resources to build a client for multi-connected Eyetribe. Instructions updated regarding LTS of .NET & .NET Framework, and examples from official website updated in consequence.

Official Documentation
----

The EyeTribe API reference is found at [Eye Tribe Developer Website](http://dev.theeyetribe.com/api/).


Steps
----

0. Download (Visual Studio 2022)[https://visualstudio.microsoft.com/en/downloads/], (.NET Framework 4.8)[https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48]
1. Select appropriate options in VS Installer to downlaod
2. In Visual Studio 2022, open the (samples project)[https://github.com/EyeTribe/tet-csharp-samples/tree/master] by selecting the ``.sln`` file.
3. Update to .NET Framework 4.8 in the window which will automatically pops up.
4. Right-click on the desired ``.csproj`` file and select ``Set as start-up project``
5. Run using the plain green play button on top. 

> Note: In order to run Eye Tribes in parralel, you'll need to run one Tribe server for each. To do so, use config files :

```json
{
   "config" : {
     "device" : [deviceIndex - until 8 according to official doc],
     "remote" : false,
     "framerate" : 60,
     "port" : [port]   
   }
}
```

and launch them from a terminal :
```bash
start "" "C:\Program Files (x86)\EyeTribe\Server\EyeTribe.exe" "path\to\config\file\EyeTribe2.cfg"
```

Multidevice client
---

1. Select ``MultideviceCalibration.csproj`` as startup project in VS.
2. Run the project
3. Changing of running device for calibration might take few seconds, the window may freeze meanwhile 