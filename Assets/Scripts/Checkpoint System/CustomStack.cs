using System;

// This is a custom stack that follows the Last-In-First-Out data structure.
// We use <T> (Generic) so that this stack can hold any data type. (OneWheelStudio, 2020)
public class CustomStack<T>
{
    private T[] items;      // The internal array that stores our data
    private int top = -1;   // The index of the most recently added item (-1 means empty)
    private int capacity;   // The maximum size of the current array

    // Constructor to initialize the stack
    public CustomStack(int size = 10)
    {
        capacity = size;
        items = new T[capacity];
    }

    // Adds a new item to the top of the stack
    public void Push(T item)
    {
        // If the array is full, we double its size to prevent an index error
        if (top == capacity - 1)
        {
            Array.Resize(ref items, capacity * 2);
            capacity *= 2;
        }
        // Increment top and place the new item at that index
        items[++top] = item;
    }

    // Removes and returns the item at the top of the stack
    public T Pop()
    {
        // If the stack is empty, we throw an exception to prevent an index error
        if (IsEmpty()) throw new Exception("Stack Underflow: Cannot pop from an empty stack!");
        
        // Return the item and then decrement the top index
        return items[top--];
    }

    // Returns the item at the top of the stack without removing it
    public T Peek()
    {
        // If the stack is empty, we throw an exception to prevent an index error
        if (IsEmpty()) throw new Exception("Stack is empty: Nothing to peek at!");
        // Return the item at the top index without modifying it
        return items[top];
    }

    // IsEmpty is a Helper method to check if the stack has no items
    public bool IsEmpty() => top == -1;

    // Property to see how many items are currently in the stack
    public int Count => top + 1;
}