using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elang 
{
    /// <summary>
    /// <br>Unused.</br> 
    /// <br>Not yet "deprecated" perse, but will proably be replaced with a c++/cli dll.</br>
    /// <br>In any case it is functional for the time being; just unused.</br>
    /// </summary>
    public class Heap {
        IHeapNode[] pile;
        public int Count { get; private set; }

        public Heap(int nodeNum) {
            pile = new IHeapNode[nodeNum];
        }

        public bool Contains(IHeapNode node) {
            if (pile[node.index] == node)
                return true;
            else return false;
        }

        public bool UpdateCost(IHeapNode node, float cost) {
            if (!Contains(node))
                return false;

            node.cost = cost;
            CompareAncestorsOf(node);
            CompareOffspringsOf(node);
            return true;
        }

        //Warning: Optimized for Performance. Only use it to reduce cost of existing nodes.
        public bool ReduceCost(IHeapNode node, float cost) {
            if (!Contains(node))
                return false;
            node.cost = cost;
            CompareAncestorsOf(node);
            return true;
        }

        public void Enqueue(IHeapNode node) {
            Count++;
            node.index = Count;
            pile[Count] = node;
            CompareAncestorsOf(node);
        }

        public IHeapNode Dequeue() {
            if (Count == 0)
                return null;

            IHeapNode head = pile[1];

            if (Count == 1) {
                Count--;
                pile[1] = null;
                return head;
            }
            
            pile[1] = pile[Count];
            pile[1].index = 1;
            pile[Count] = null;
            Count--;
            
            CompareOffspringsOf(pile[1]);
            return head;
        }

        void CompareAncestorsOf(IHeapNode node) {
            while (true) {
                IHeapNode parent = pile[node.index / 2];
                if (parent != null && parent.cost > node.cost)
                    Swap(parent, node);
                else break;
            }
        }

        void CompareOffspringsOf(IHeapNode node) {
            while (true) {
                IHeapNode child = node;
                int left = node.index * 2;
                int right = node.index * 2 + 1;

                if (Count >= left) {
                    if (child.cost > pile[left].cost)
                        child = pile[left];
                }

                if (Count >= right) {
                    if (child.cost > pile[right].cost)
                        child = pile[right];
                }

                if (child == node)
                    break;

                Swap(node, child);
            }
        }
        
        void Swap(IHeapNode ancestor, IHeapNode offspring) {
            pile[ancestor.index] = offspring;
            pile[offspring.index] = ancestor;

            int index = offspring.index;
            offspring.index = ancestor.index;
            ancestor.index = index;
        }
    }
}
