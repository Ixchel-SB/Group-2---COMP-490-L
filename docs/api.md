docs/api.md
API Documentation: PickupItem

Name and Short Description:
PickupItem is a MonoBehaviour component on an item that collects the item from the world when the player tagged “Player” collides with its trigger collider. When this is triggered, the item is moved from the world to the player’s ItemPickupHandler. 
Signature:
void OnCollisionEnter (Collision collision)
void OnTriggerEnter (Collider other)
Parameters:
void OnCollisionEnter (Collision collision)
Collision (collision) - an object provided by Unity that contains information about the physical collision and the GameObject that made contact.

void OnTriggerEnter (Collider other)
Other (Collider) - An object provided by Unity that represents the collider that entered the object’s trigger volume

Item - The item’s data associated with its world object
Return Values:
OnCollisionEnter - returns void (but adds item to inventory) 
OnTriggerEnter - returns void (but destroys world object)
Errors or Exceptions:
Tag - Function relies on a GameObject (the player) having a “Player” tag
Collider - One of the GameObjects from the collision must have a collider with IsTrigger applied
Item - For an item to be picked up, it must have a ScriptableObject assigned to it
Example How to Call:
Attach PickupItem to a WorldObject as a component
Assign ScriptableObject to item in PickupItem component
WorldObject must have a collider with isTrigger
Player must be tagged “Player” and have ItemPickupHandler assigned 
When the Player and the object collide, the object will be added to the inventory and removed from the scene.

Notes and Limitations:
Currently, items will be picked up upon collision, no key interaction needed.
Items will be displayed on the inventory only if they have an item ScriptableObject assigned, as this holds the item Id, name, sprite and description and that is displayed on the inventory UI. If an item is missing this, it will still disappear from the scene but it will not appear anywhere else.
There is currently no drop item functionality implemented.
