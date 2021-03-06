# CPE 400 Final Project - Self Healing Graph

## Introduction
This program is an implementation of a self-healing graph algorithm. 
It is worth noting I wrote this algorithm without researching the topic other 
than understanding what problem I needed to solve. I implemented this algorithm blind.
After finishing the algorithm I started to research other self-healing algorithms and found 
that what I had implemented, very closely resembles the DASH algorithm [outlined here](http://digitalrepository.unm.edu/cgi/viewcontent.cgi?article=1008&context=cs_etds)

## How To Use

### Requirements
Both of these are included in the latest version of visual studio 
- .Net Runtime Version >= 4.5 
- C# Compiler Version >= 7.0

### Setup
Graphs can either be user defined, or generated by the program. This is done through the defining two new run configurations:
 - SelfHealingNetwork-GenerateGraph
 - SelfHealingNetwork-XmlGraph

To generate these run configurations follow these steps:
1. Right click on project file in solution explorer
2. Click "Properties" or "Options" on macOS
3. A window should pop up, on the left hand side you should see a section called "Run" click it
4. There should be a section called "configurations" click it and look for a button to add a new configuration and click it 

![Could not fetch example image](https://github.com/jakeleveroni/SelfHealingGraph/blob/master/SelfHealingNetwork/Assets/configuration2.png)

5. A dialogue should pop up and ask for a name, call it GenerateGraph
6. In the "Arguments" text box enter the string "gen" (without the quotes)

![Could not fetch example image](https://github.com/jakeleveroni/SelfHealingGraph/blob/master/SelfHealingNetwork/Assets/configuration1.png)

7. Repeat steps 1 - 7 but in again but instead of naming your configuration "GenerateGraph" call it "XmlGraph" and the "Argument" should be "xml"
 
If you want the program to generate a graph, select the __SelfHealingNetwork-GenerateGraph__ run configuration
before running the project it will generate a graph of size 65 nodes and 200 edges, not configurable for the time being. 

Otherwise if you want to define your own graph you need to select the __SelfHealingNetwork-XmlGraph__ run configuration
before running the project then, you will then need to create your own graph using the following instructions:

Graphs are defined in XML, you can customize the input graph structure by editing the graph.mxl file located at __SelfHealingNetwork/SelfHealingNetwork/Xml__
The format of the XML defined graph is as follows:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Graph xmlns="SelfHealingGraph">
    <Node Value="A">
        <Edge EdgeTo="B" Weight="5"></Edge>
        <Edge EdgeTo="C" Weight="1"></Edge>
    </Node>
    <Node Value="B">
        <Edge EdgeTo="A" Weight="5"></Edge>
        <Edge EdgeTo="C" Weight="3"></Edge>
        <Edge EdgeTo="D" Weight="2"></Edge>
        <Edge EdgeTo="E" Weight="6"></Edge>
    </Node>
    <Node Value="C">
        <Edge EdgeTo="A" Weight="1"></Edge>
        <Edge EdgeTo="B" Weight="3"></Edge>
        <Edge EdgeTo="D" Weight="4"></Edge>
    </Node>
    <Node Value="D">
        <Edge EdgeTo="B" Weight="2"></Edge>
        <Edge EdgeTo="C" Weight="4"></Edge>
        <Edge EdgeTo="E" Weight="10"></Edge>
        <Edge EdgeTo="F" Weight="2"></Edge>
    </Node>
    <Node Value="E">
        <Edge EdgeTo="B" Weight="6"></Edge>
        <Edge EdgeTo="D" Weight="10"></Edge>
        <Edge EdgeTo="F" Weight="7"></Edge>
    </Node>
    <Node Value="F">
        <Edge EdgeTo="D" Weight="2"></Edge>
        <Edge EdgeTo="E" Weight="7"></Edge>
        <Edge EdgeTo="G" Weight="1"></Edge>
    </Node>
    <Node Value="G">
        <Edge EdgeTo="F" Weight="1"></Edge>
    </Node>
</Graph>
```

- All Nodes must be declared inside a `<Graph>` tag. 
- All Edges must be declared inside a `<Node>` tag.
- Edges must be defined in both nodes that it connects, for example if you have a nodes A and B and they share an edge, you must declare that edge in both the node A and node B tags with the 'EdgeTo' attribute pointing to the other node

### Compiling
1. Open the SelfHealingGraph.sln file in Visual Studio or JetBrains' Rider (MacOS or PC)
2. Restore package dependancies through NuGet 
    - Visual Studio: Right click solution in solution explorer, select "Restore NuGet Packages"
    - Rider: Right click roject file in solution explorer, select "Manage NuGet Packages" a panel will open up, click the icon in the top right labaled "restore"
3. Make sure all NuGet Package installations succeeded
4. Run the project
    - Visual Studio: click the green play button on the top of the window
    - Rider: click the play button in the top left of the window 
5. The program will run until it is manually stopped, a node will be killed every 10 seconds and information will be output to the console. 

### Note
There is a bug in the Dijkstra shortest path function that can cause a crash. This is random and I believe due to a race condition. 
I have verified it does not effect the output information but it will crash the program. If this does happen just rerun the program
and it will start again. I am sorry I havn't been able to get this bug solved but I will continue to work on it until I do.

## Disclaimer
The structure and implementation of the NetworkGraph, Node, WeightedEdge classes was heavily influenced 
by the code in this post on codereview.stackoverflow.com originally written by Vardominator with edits
provided by t3chb0t and myself. 
[code review post](https://codereview.stackexchange.com/questions/138475/weighted-graph-and-pathfinding-implementation-in-c)
