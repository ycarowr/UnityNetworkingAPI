[![Unity Version](https://img.shields.io/badge/Unity-2019.2.19f1%2B-blue.svg)](https://unity3d.com/get-unity/download)

# Unity Networking API

The repository contains a solution for networking in Unity made with C# with no use of Unity's built-in HLAPI components. 
Please consider that this is a project made for learning purposes, so there are some limitations and a lot of room for further improvements. 

### Objetives

1. My main goal was to make two unitypackages, one for a client app and other for the server app.
2. Both projects will be starting points whenever a multiplayer prototype pops up in my head.

### Projects

Here are the links for both unity projects with their networking functionalities:

1. The [Client Project](/UnityNetworkingAPI/UnityGameClient).
2. The [Server Project](/UnityNetworkingAPI/UnityGameServer).

Here are the links for both unity packages:

1. The [Client UnityPackage](/UnityNetworkingAPI/UnityGameClient).
2. The [Server UnityPackage](/UnityNetworkingAPI/UnityGameServer).

### How to use it? and Demo Apps. 

You will be able to find Demo Apps with code and scenes that you can use to understand the projects. The demos are located inside each respective unity package. 

Currently in the demo you can (all the messages are processed in the server side):

1. Instantiate players;
2. Move the players around and jump;
3. Send text messages Client <-> Server;

Here is an image of two demo players connected to the server:
<img width="450" height="200" src="/UnityNetworkingAPI/Images/TwoClients.GIF">

Here are the content of the demo scenes:

Server

<img width="350" height="180" src="/UnityNetworkingAPI/Images/DemoServerApp.GIF">

Client

<img width="350" height="180" src="/UnityNetworkingAPI/Images/DemoClientApp.GIF">

In short words, once configurated, which is basically filling a scriptable object with some data, there are base classes for the each application, client and server. It's just to inherit from these respective classes and you are good to go. The steps are described below.

### Creating the Server Application

The easiest way to do it is described below:

1. Download the Server UnityPackage and import it into your game. [here](/UnityNetworkingAPI/UnityGameServer).
2. Create a new script and open it. 
3. Add the following import on top `using UnityGameServer;` 
4. Change the parent class of the script to `BaseServerApplication` 
5. Drop the script into a gameObject inside the scene.
6. Drag and drop the scriptable object that contains the server configurations. The asset is located at: `UnityNetworkingAPI/UnityGameServer/Assets/ServerConfigs.asset`
7. Override the necessary methods.

It should look like this picture from the demo: 

<img width="450" height="200" src="/UnityNetworkingAPI/Images/DemoServerAppGO.GIF">

The demo scene is inside the project. Here is the script: [DemoServerApp.cs](/UnityNetworkingAPI/UnityGameServer/Assets/Scripts/Demo/ServerApplication/DemoServerApplication.cs) 

### Creating the Client Application

#### Configurating the Client

### References

I used [Tom Weiland](https://github.com/tom-weiland/tcp-udp-networking/) tutorial series as a base to start my work. Check out his videos to have complementary explanations about networking.
