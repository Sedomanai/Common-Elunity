namespace Elang
{
    /// <summary>
    /// Unused. See Heap.cs.
    /// </summary>
    public interface IHeapNode {
        int index { get; set; }
        float cost { get; set; }
        void Reset();
    }
}
