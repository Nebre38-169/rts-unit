# RTS UNIT

This is a sandbox/engine project. I personnaly use it to develop gameplay elements used in the next gamejam the Boogie Game Studio will take part. You're idea is mainly focused around a 3D strategie RTS-like gameplay. In order to develop our game faster during the gamejam, I've started working on how unit behave and are usable in our game. In a first time, being able to move them and give them order. Next combat and finally ressource and combat.

## How to use it

This project is mainly focues on script development for Unity GameObject. As such, all script are found in the Script section of this github.

## Requirements

This project use the free version of the [A* project](https://arongranberg.com/astar/) from Aron Granberg. You need to have a graph setup in your worl in order for the unit to move around.

## 1. GameRTSController

This script handle selection and order to unit. It must set in an empty gameObject containing atleast a mesh collider, a mesh filler and a mesh render (used for debug). A Canvas must be set as an UI. It will indicate the player which unit were in the selection zone. To select unit, drag your left click over a group of unit or left click on one unit. To unselect unit(s), left click in the void.

## 2. Unit

Unit is the base for allies and enemies unities. They are state machine with 4 state : Idle, Move, Attack and force Attack. When in idle, the unit will not move but stay on lookout for target. It goes into attack once it has found a target or it is given by an other script. In attack, the unit will try to get in range with the target and deal damage when in range. It use a cool down between two attacks. It loose the attack once the target is dead or the target is futher from the lost range. The force attack mode is the same but the unit cannot loose its targer throught the lost range. The unit goes into move mode when a destination is given. Will in move mode, the unit will look for target. If it reachs the destination with a target in range, the unit will goes into attack mode.

If you need an example, this Unit project is a made of a single scene using all those components.

## Release

- 0.1.0
  - Added GameRTSController to handle selection using a pyramid mesh
  - Added Unit, that move using a single left click
- 0.2.0
  - Rework GameRTSController to only target capsule collider when selecting unit or target
  - Added AllieUnit and EnemyUnit class to differentiate allie from enemy
  - Worked on Unit. It is now a state machine that handle executing order, following a target and attack it or move to a destination.
