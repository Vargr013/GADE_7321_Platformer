using UnityEngine;

public class HashMapEntry
{
    public string key;
    public AudioClip value;
    public HashMapEntry next;

    public HashMapEntry(string key, AudioClip value)
    {
        this.key = key;
        this.value = value;
        next = null;
    }
}