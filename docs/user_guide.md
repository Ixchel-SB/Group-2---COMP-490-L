The Vow - User Guide

Who This Guide Is For
This guide is intended for TAs, intructors, new developerd, and anyone who wants to download, run, and test The Vow Unity game project.
It explains the core gameplay system.

System Overview
The Vow is a Unity based game where the player explores the environment and collects items throughout the world.

When the player collides with collectible objects:
- The item is added to the player’s inventory system.
- The item is removed from the scene.

The project includes:
- A player controller
- An item pickup system
- An inventory management system
- ScriptableObject-based item data
- A UI system that displays collected items

Installation and Setup Instructions
Requirements
- Unity Hub installed
- Unity version: 6000.2.6f1
- Git (if cloning from the repository)

Setup Steps
1. Clone or download the project repository from GitHub.
2. Open Unity Hub.
3. Click Add Project.
4. Select the root folder of the downloaded repository.
5. Ensure the correct Unity version is selected.
6. Open the project.
7. In the Unity Editor:
- Navigate to Assets/Scenes/
- Open the main scene (MainMenu).
8. Press the Play button at the top of the Unity Editor to run the game.

Core Gameplay Workflow
1. Press Play in Unity.
2. Use the following movement controls:
- W – Move forward
- A – Move left
- S – Move backward
- D – Move right
3. Move the player toward collectible items in the world.
4. When the player collides with an item:
- The item is added to the inventory.
- The item disappears from the scene.
6. Collected items appear in the Inventory UI if a valid ScriptableObject is assigned.

Where Outputs Appear
- Game View Window – Displays the running game.
- Inventory UI Panel – Shows collected items.
- Unity Console Window – Displays debug logs and error messages.
If items are not appearing in the inventory, check that the item has a valid ScriptableObject assigned.

Troubleshooting
Issue 1: Player Cannot Pick Up Items
Possible Causes:
- The Player GameObject is not tagged as "Player".
- The item’s Collider does not have IsTrigger enabled.
- The PickupItem component is not attached.
- The item does not have a ScriptableObject assigned.

Solution:
- Select the Player GameObject and verify the Tag is set to "Player".
- Select the item and ensure its Collider has IsTrigger checked.
- Confirm that the PickupItem component is attached.
- Assign a valid Item ScriptableObject in the Inspector.

Issue 2: Item Disappears but Does Not Appear in Inventory
Cause:
- The item is missing its ScriptableObject data.
Solution:
- Select the item in the scene.
- Assign a valid ScriptableObject in the Inspector.
- Ensure the ScriptableObject includes item ID, name, sprite, and description.

Issue 3: Unity Version Errors
Cause:
- The project is opened in a different Unity version.
Solution:
- Install the required Unity version in Unity Hub.
- Reopen the project using the correct version.

Notes and Limitations
- Items are automatically collected upon collision (no interaction key implemented yet as development is still being done).
- Items will only appear in the inventory UI if a valid ScriptableObject is assigned.
- There is currently no item drop functionality implemented.
