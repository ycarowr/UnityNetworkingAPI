[![Unity Version](https://img.shields.io/badge/Unity-2019.2.19f1%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![Twitter](https://img.shields.io/twitter/follow/LagrangeSpot.svg?label=Follow@LagrangeSpot&style=social)](https://twitter.com/intent/follow?screen_name=LagrangeSpot)

# Unity Networking API

The repository contains a solution for networking in Unity, it's made in pure C# without the usage of Unity's built-in HLAPI components. 
Please consider that this is a project made for learning purposes, so there are limitations, bugs and a lot of room for further improvements and tests, feel free to mail me with suggestions about the architecture or anything else. 

### Objetives

The main goal is to have two unitypackages, client and server, whitch will be starting points for future multiplayer prototypes. And also learn the basics of networking.

### Projects

Here are the code of both projects:

1. The [Client Project](/UnityNetworkingAPI/UnityGameClient).
2. The [Server Project](/UnityNetworkingAPI/UnityGameServer).

Here is the link to both [Unity Packages](/UnityNetworkingAPI/Versions).

### How to use it? And Demo Apps

Inside each package there are demo applications made to help with understanding the tool.

Currently in the demo you can:

1. Instantiate players;
2. Move players around and jump;
3. Send text messages Client <-> Server;

Note: all the messages are processed in the server side.

Here is an image of two demo players connected to the server:
<img width="450" height="200" src="/UnityNetworkingAPI/Images/TwoClients.GIF">

Here are the content of the demo scenes:

<img width="350" height="180" src="/UnityNetworkingAPI/Images/DemoServerApp.GIF"><img width="350" height="180" src="/UnityNetworkingAPI/Images/DemoClientApp.GIF">

### Creating the Server Application

The easiest way to create a server app is described below:

1. Download the Server UnityPackage and import it into your game server. Link [here](/UnityNetworkingAPI/UnityGameServer).
2. Create a new script and open it. 
3. Add the following import on top: `using UnityGameServer;` 
4. Change the parent class of the script to: `BaseServerApplication` 
5. Drop the script into a gameObject inside the scene.
6. Drag and drop the scriptable object that contains the server configurations. The asset is located at: `UnityNetworkingAPI/UnityGameServer/Assets/UnityGameServer/ServerConfigs.asset`

It should look like this picture from the [DemoServerApp.cs](/UnityNetworkingAPI/UnityGameServer/Assets/UnityGameServer/Scripts/Demo/ServerApplication/DemoServerApplication.cs) : 

<img width="450" height="200" src="/UnityNetworkingAPI/Images/DemoServerAppGO.GIF">

7. Press Play and you should be able to see the following start up logs, which means the server is online in your local machine: 

<img width="550" height="500" src="/UnityNetworkingAPI/Images/ServerStartUpLogs.GIF">

### Creating the Client Application

The client application has almost the same set of steps as the server:

1. Download the Client UnityPackage and import it into your game client. Link [here](/UnityNetworkingAPI/UnityGameClient).
2. Create a new script and open it. 
3. Add the following import on top: `using UnityGameClient;` 
4. Change the parent class of the script to: `BaseNetworkApplication` 
5. Drop the script into a gameObject inside the scene.
6. Drag and drop the scriptable object that contains the client configurations. The asset is located at: `UnityNetworkingAPI/UnityGameClient/Assets/UnityGameClient/ClientConfigs.asset`

It should look like this picture from the [DemoClientApp.cs](/UnityNetworkingAPI/UnityGameClient/Assets/UnityGameClient/Scripts/Demo/Client/DemoClientApp.cs) : 

<img width="400" height="200" src="/UnityNetworkingAPI/Images/DemoClientAppGO.GIF">

7. Press Play and, if the server is online in your local machine, you should be able to see the following start up logs: 

<img width="550" height="250" src="/UnityNetworkingAPI/Images/ClientStartUpLogs.GIF">


After both applications are connected a player will be instantiated and you will be able to move and using the keyboard WASD and Space.

Feel free to use the tool as you need :)

### References

I used [Tom Weiland](https://github.com/tom-weiland/tcp-udp-networking/) tutorial series as a base to start my work. Check out his videos to have complementary explanations about networking.
