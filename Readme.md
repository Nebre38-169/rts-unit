# RTS UNIT

This is a sandbox/engine project. I personnaly use it to develop gameplay elements used in the next gamejam the Boogie Game Studio will take part. Our idea is mainly focused around a 3D strategie RTS-like gameplay. In order to develop our game faster during the gamejam, I've started working on how unit behave and are usable in our game. In a first time, being able to move them and give them order. Next combat and finally ressource and combat.

## How to use it

This project is mainly focues on script development for Unity GameObject. As such, all script are found in the Script section of this github.

## Requirements

This project use the free version of the [A* project](https://arongranberg.com/astar/) from Aron Granberg. You need to have a graph setup in your worl in order for the unit to move around.

## 1. GameRTSController

This script handle selection and order to unit. It must set in an empty gameObject containing atleast a mesh collider, a mesh filler and a mesh render (used for debug). A Canvas must be set as an UI. It will indicate the player which unit were in the selection zone. To select unit, drag your left click over a group of unit or left click on one unit. To unselect unit(s), left click in the void. Using the right click, you gives order to unit, such as attacking an enemy, gathering ressource or juste move to a position. Handles having multiple unit selected and gives them differents position when moving to the same point.

- Settings :  
  - Debug (*boolean*) : Check it if you want this script to gives you detail about what it does
  - Unit By Lign (int): number of unit by lign on a static formation
  - Space Between Ligns (float): space between two ligns in the formation
  - Space Between Unit (float): space between two units on the same line in the formation
  - Selection Panel (*RectTransform*): UI canvas where the selection rectangle appears

## 2. Unit

Unit is the base for allies and enemies unities. They are state machine with 4 state : Idle, Move, Attack, Force Attack, Harvest, Bring. Each unit has is own life, speed, damage, ranges and cooldown. They are the most basic unit, they can fight and harvest ressource. To create a new unit object, create a blank one that holds the unit script. As child of it, add a Ressource Detector, a Target Detector and GFX. See prefabs folder for more information.

- Order :
  - Idle : do nothing and not move
  - Move : Move to a direction. Can pick-up target will moving
  - Attack : Attack a enemy unit. Target is a picked-up target.
  - Force Attack : Attack a selected enemy unit.
  - Harvest : Goes to a ressource source to gather the selected ressource until full.
  - Bring : Goes to the closest depot to unload ressource they carry.
- Settings :
  - Debug (*boolean*): Check it if you want this script to gives you detail about what it does
  - New Waypoint Distance (*float*): Distance at which the unit picks the next waypoint as target. Used for moving arround
  - Max Life (*float*): maximum HP the unit as
  - Damage (*float*): damage the unit deals at each attacks
  - Search Range (*float*): Range in which an enemy unit will be picked-up as target
  - Attack Range (*float*): Range in which the unit will deal damage
  - Lost Range (*float*): Range out of which an enemy unit will be lost. Ignored when in force attack
  - Cool Down Duration (*float*): Cool down in sec between to attacks
  - Harvest Range (*float*): Range in which the unit will harvest from the source
  - Harvest Cool Down (*float*): Cool down between two harvest action or two unload action
  - Max Load (*float*): maximum weight the can carry
  - Unload Packet (*int*): number of ressource the unit can unload at once

As the unit script herits from the Target class, it alos herits from all its settings such as max life and Ally.

## 3. Ressource

Ressource is a scriptable object. It describe a ressource can be gathered in a Ressource Source and droped into a Depot. It has a name, a weight and a icon.

## 4. Ressource Source

Ressource Source describes a place where unit can harvest ressource. It hold a max quantity of ressource and gives the given quantity of ressource at best. When the source as no ressource left, it is destroyed.

- Settings :
  - Max Quantity (*int*): maximum quantity the source holds
  - Given Quantity (*int*) : maximum quantity the source can give
  - Ressource (*Ressource*) : which ressource is given

## 5. Depot

Depot describes a place where unit can unload ressources they have gather. They accept one or many ressources.

- Settings :
  - StoredRessource (*List\<Ressource>*): Ressource(s) that can be stored in the depot
  - Manager (*RessourceManager*): Scripts that handle UI and total ressource count
  
As Caserne script herits from Building class, it herits from its settings such as max life, ally, constructed and initialMat.

## 6. Caserne

Caserne creates unit using ressources. Each caserne as its creation queue and can create a list of unit.

- Settings :
  - Spawnable Unit (*List\<UnitHolder>*) : List of unit the caserne can spawn
  - Spawn Point (*Transform*) : Child object where unit spawn by default
  - Spawn Obstacke Layer : Layer to detect collision when spawning unit. If there is collision with elements of this layer, the unit will spawn futher in x direction.
 
As Caserne script herits from Building class, it herits from its settings such as max life, ally, constructed and initialMat.

## 7. Turret

Turret are buildings that shoots at incomming enemy.

- Settings:
  - Attack Range (*float*): range the turret can attack
  - Damage (*float*) : how much damage an attack deals to the target
  - Cool Down Duration (*float*) : time in seconds between two attacks

As Caserne script herits from Building class, it herits from its settings such as max life, ally, constructed and initialMat.

## 8. Ressource Manager

Ressource Manager is a global scripts that hold every depot and every ressource used in the game and update UI when the amount of ressources changes. On startup, it generates the UI using RessourceDiv.

- Settings :
  - Ressourcs (*List\<Ressource>*) : Ressource(s) that can be used in the game
  - Prefabs (*RessourceDiv*) : UI element that is used to show the ressource and how much you have of it

## 9. Ressource Detector/Trigger

Ressource detector is the empty gameobject that hold the Ressource Trigger script. It handles range detection for ressource source and depot. It must be placed as child object of a unit.

If you need an example, this Unit project is a made of a single scene using all those components.

## 10. UIManager

The UI Manager handles creating and updating all UI elements. It holds UI elements for building construction and casernes.

- Settings :
  - Ressource Panel (*RectTransform*): Panel where *RessourceDiv* will appeare
  - Building Panel (*RectTransform*): Panel where *BuildingButton* will appeare
  - Caserne Panel (*RectTranform*) : Panel where the creation queue will appeare
  - Ressource Div Prefab (*RessourceDiv*) : Prefab gameOjbect that handles displaying ressource icon and count
  - Building Button Prefab (*BuildingButton*) : Prefab gameObject that handles displaying the building icon and handles interaction.
  - Unit Button Prefab (*UnitButton*) : Prefab gameObject that handles displayin the unit icon and handles interaction.
  
## 11. GameRTSBuilder

The GameRTSBuilder handles interaction with player to construct buidling. When the order is given from the UI Manager, it use a placeholder the size of the selected building and sticks it to the mouse position. When the placeholder is green, the building can be placed. To construct building, it places a builder thath holds the building and make it appears from the ground.

- Settings :
  - Building (*List\<BuildingHolder>*) : List of building that can be constructed.
  - Builder Prefab (*Builder*) : Will be placed to handle construction
  - PlaceHolder (*GameObject*) : Placeholder to represent the size of the futur build
  - Space Detection Mask (*LayerMask*) : Layer where collision will be detected to place building

## 12. Target

Abstract class that describe general behaviour of target elements. Building and unit herits from those.

- Settings :
  - maxLife (*float*) : Maximum life of a target
  - ally (*boolean*) : Indicates if the target if with the player or not
  - debug (*boolean*) : Indicates if the target will display debug message

## 13. Other

- BuildingButton :UI element to hold icon and handles selected and disable state
- BuildingHolder : Scriptable objects to store icon, name and cost of the building
- RessourceObserver : Interface of all Observer of ressources count
- OpponentInterface : Interface for opponent, that target a Target
- QueueSlot : UI element to holds icon and handle interaction within the creation queue
- Utils : general function regarding mouse position and ally/enemy relation


## Release

- 0.1.0
  - Added GameRTSController to handle selection using a pyramid mesh
  - Added Unit, that move using a single left click
- 0.2.0
  - Rework GameRTSController to only target capsule collider when selecting unit or target
  - Added AllieUnit and EnemyUnit class to differentiate allie from enemy
  - Worked on Unit. It is now a state machine that handle executing order, following a target and attack it or move to a destination.
- 0.3.0
  - Added Ressource, RessourceSource, RessourceDetector/Trigger and Depot to handle ressource gathering logic
  - Modified GameRTSController to target RessourceSource
  - Made Prefabs of every logic brics used in this project
- 0.4.0
  - Added Caserne, UIManager, Builder, GameRTSBuilder and ally/enemy logic.
  - UI was improved, it is all handled by the UIManager that communicates with Caserne and Depot
  - Added the possibility to construct building, such as caserne, depot and turret.
