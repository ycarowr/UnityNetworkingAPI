# Unity Networking API
 
Why I made this repository?

I wanted to learn about networking in Unity/C#. Digging the internet a bit I've found [Tom Weiland](https://github.com/tom-weiland/tcp-udp-networking/) tutorial series, I used his projects as base to start my work.

### Goal

The goal was to build a server and a client project that could be used as default whenever I start a new project.

### Projects

I decided to split the work in two separated unity projects with the basic networking functionalities:

1. The [Client Project](/UnityNetworkingAPI/UnityGameClient).
2. The [Server Project](/UnityNetworkingAPI/UnityGameServer).

There are base classes for the client and the server, that way, whenever I have to start a new project I am able inherit from the respective classes below and you are good to go. 

1. The base class for a [Client Application](/UnityNetworkingAPI/UnityGameClient/Assets/Scripts/NetworkClient/BaseNetworkApplication.cs)
2. The base class for a [Server Application](/UnityNetworkingAPI/UnityGameServer/Assets/Scripts/Server/BaseServerApplication.cs)

These two classes are able to talk each other with the basic messages that make the server to work. They can be used to build a new project "on top".


### How to use it? 

There are some steps in order to get everything working. They are described below.

### Creating the Server Application

#### Configurating the Server

### Creating the Client Application

#### Configurating the Client

##
