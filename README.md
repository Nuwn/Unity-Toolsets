This project contains tools frequenly used in my own projects.

Some still being upgraded and improved over time.

## EventPlanner
EventPlanner allowes me to make event's more global.<br>
It has proven to be a quite valuable ally.<br>
Only downside is key being string makes it harder to find references in VS.<br>
But so far, benefits outweights the cost.


Example usage:

```
EventPlanner.Invoke("OnEventHappened", true);
  
EventPlanner.AddListner("OnEventHappened", OnEventHappened);
EventPlanner.RemoveListner("OnEventHappened", OnEventHappened);
    
public void OnEventHappened(object data) => res = (bool) data;
```

## SaveSystem
This homemade save system saves encrypted Byte64 files to APPDATA instead of PlayPrefs or databases.<br>

Tested for PC, Android.

Save system makes use of [Lazy's Global Coroutine](https://github.com/Lazy-Solutions/Unity.CoroutineUtility).<br>
It allowes the use of coroutine during saves to avoid spikes, to save 1 file every frame.<br>
Stutter can still happen if one is to save a massive dump.

Features:
- Saves to individual files for smaller load and saves. I do not save everything to one file.
- Surpricingly easy to use, as simple as PlayPrefs, but without the limitations.
- Using Newtonsoft, way more powerful than Unity's, I've had no issues so far serializing about anything.

How to use:
```
List<InventoryItem> inventory;

var saveAddress = "Player/Inventory";

inventory = SaveSystem.Load<List<ItemRef>>(SaveAddress);

SaveSystem.Save(SaveAddress, inventory);
``` 

Future plans?:
- If the time comes to develop for other platforms, I may upgrade the file location.
- If i ever is in need of cloud save, i might hook it up to services like Playfabs or Mongo database.

But this is not on the roadmap for now.

## Extra
This folder contains small extra things I use.
