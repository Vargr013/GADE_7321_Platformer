using UnityEngine;

public class DialogueQueue 
{
    private DialogueEntry[] items;
    private int front = 0;
    private int rear = 0;
    private int count = 0;

    public DialogueQueue(int capacity)
    {
        items = new DialogueEntry[capacity];
    }

    public void Enqueue(DialogueEntry item)
    {
        if (count == items.Length) return; // Full
        items[rear] = item;
        rear = (rear + 1) % items.Length;
        count++;
    }

    public DialogueEntry Dequeue()
    {
        if (count == 0) return null;
        DialogueEntry item = items[front];
        front = (front + 1) % items.Length;
        count--;
        return item;
    }

    public int Count => count;

    public void Clear()
    {
        front = 0;
        rear = 0;
        count = 0;
    }
}
