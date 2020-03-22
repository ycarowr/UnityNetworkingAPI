[![Unity Version](https://img.shields.io/badge/Unity-2019.2.19f1%2B-blue.svg)](https://unity3d.com/get-unity/download)

# Unity Networking API

The repository contains a solution for networking in Unity built in pure C# with no use of Unity's built-in HLAPI or whatsover. 
Please consider that this is a project made for learning purposes, so there are some limitations and a lot of room for further improvements. 

### Objetives

1. My main goal was to build two projects, client and server.
2. Both projects will be starting points whenever a multiplayer prototype or idea pops up in my head.

Note: I won't get deep into details about the process of building it or why I took decision A or B. 

### Projects

Here is the link for both unity projects with their basic networking functionalities:

1. The [Client Project](/UnityNetworkingAPI/UnityGameClient).
2. The [Server Project](/UnityNetworkingAPI/UnityGameServer).

Once configurated, there are base classes for the each of them, it's just to inherit from the respective classes and you are good to go. 

### How to use it? 

There are some steps in order to get everything working and configurated. They are described below.

### Creating the Server Application

The easiest way to do it is described below:

1. Download the Server Project. Link [here](/UnityNetworkingAPI/UnityGameServer).
2. Create a new script and open it. 
3. Add the following import on top `using UnityGameServer;` 
4. Change the parent class of the script to `BaseServerApplication` 
5. Drop the script into a gameObject inside the scene.
6. Drag and drop the scriptable object that contains the server configurations. The asset is located at: `UnityNetworkingAPI/UnityGameServer/Assets/ServerConfigs.asset`

It should look like this demo: 

<img width="450" height="200" src="/UnityNetworkingAPI/Images/DemoServerAppGO.GIF">

The demo scene is inside the project. Here is the script: [DemoServerApp.cs](/UnityNetworkingAPI/UnityGameServer/Assets/Scripts/Demo/ServerApplication/DemoServerApplication.cs) 

### Creating the Client Application

#### Configurating the Client

### References

I used [Tom Weiland](https://github.com/tom-weiland/tcp-udp-networking/) tutorial series as a base to start my work. Check out his videos to have complementary explanations about networking.
