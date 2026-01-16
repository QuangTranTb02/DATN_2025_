using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedObject : MonoBehaviour
{
    [Serializable]
   public class SaveSpawedObjectData
    {
        public int objectID;
        public Vector3 worldPosition;

        public SaveSpawedObjectData(int objectID, Vector3 worldPosition)
        {
            this.objectID = objectID;
            this.worldPosition = worldPosition;
        }
    }
    public int objectID;
    public void SpawnedObjectDestroyed()
    {
        transform.parent.GetComponent<ObjectSpawner>().SpawnedObjectDestroy(this);
    }
}
