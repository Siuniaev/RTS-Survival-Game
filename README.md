## Test assignment for the "Unity3D Developer"
### Objective: Implement "The Last Stand" style game with the strategy elements.
***
Mandatory conditions:
* The presence of several waves of different opponents in terms of parameters;
* The presence of several types of buildings that can create and improve minions;
* The ability to control the hero with the mouse: movement and attack opponents;
* The hero must gain experience by killing opponents;
* It is possible to control the individual minions fighting on the player’s side and the groups of minions;
* The presence of a 3D map;
* The presence of two camera modes: The camera holds the main character in the center and the player can freely move the camera;
* The code style follows Microsoft’s recommendations.

Click on the [video link](https://vimeo.com/396650302) to see how the project works:  
[![Video](https://i32.servimg.com/u/f32/11/33/69/31/plariu10.jpg)](https://vimeo.com/396650302)

***
### What is interesting inside?

**1. K-D Tree implementation for the fast searching GameObjects.**  
> Assets.Scripts.DataStructures.KDTree.cs   

Search: Average O(logn) - Worst O(n). This structure allowed up to a thousand active units to be located in the game scene at the same time without a critical decrease in performance.

**2. DI-container for Unity projects with a minimum of required functionality.**
> Assets.Scripts.DI.DIContainer.cs

Dependency injection container, composition root. Uses properties with ```[Injection]``` attribute for injection.
Can inject itself as a dependency ```IDependencyInjector``` so that it can be used to make injections into newly created objects in real-time. Resolves objects with suppression of endless circular injections.

**3. The single point for processing user input.**
> Assets.Scripts.InputHandling.InputProvider.cs

The root of user input handling, handles user input and provides access to input handlers - includes the ability to subscribe to expected input and unsubscribe in real-time.

**4. Continued code execution from other threads in the Unity thread.** 
> Assets.Scripts.UnityThreadUtil

Allows to use the thread pool, asynchronous code, and successfully continuing to execute code in ```MonoBehaviour``` classes by returning to the Unity thread.

**5. Object Pool for GameObjects, grouping and searching by its prefab instance id.**
> Assets.Scripts.ObjectsPool.cs

Reduces the total number of the created objects by the reusing returned objects. Groups and searches for created objects by the prefab instance id out of which they were created. Unity's documentation is guaranteed the unique instances id of an object given with ```GetInstanceID()``` method.

**6. Also:**  
```Switch```-```Case``` statemens are not used anywhere in the code (as a bad practice).  
There are no godclasses with the names *Manager*, *Controller*, etc.  
There are no *Singletons* anywhere except one place: the ```GameStarter```, - it creates and configures the *DIContainer*, so it should be alone in the scene, and this is the only place where the use of singleton is justified. Otherwice only the *DIContainer* is responsible for the presence of single-objects in the scene according to how it is configured.  
All important game objects settings for convenience are placed in *Scriptable Objects*. In this project, I tried to simplify the adding of new types of units, weapons, skills, etc. and make it flexible enough.  

### What can be improved?
* The code can be covered by unit tests.
* The self-expanding game scene can be made, instead of many prefabs pre-configured in the scene, as it is now.
* All colliders from Units, Buildings can be removed and their search for a click can be changed to the search for the nearest point in the KD-Tree.
* The property of the dimensions for the Target GameObject can be added so that the Units do not move to the center of the object, but to its point closest to itself.
* And much more, but I'm already too lazy :P