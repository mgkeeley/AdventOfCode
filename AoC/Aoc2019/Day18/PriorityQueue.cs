using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC {
    public class PriorityQueue<T> where T: struct {
        struct PriorityQueueItem : IEquatable<PriorityQueueItem> {
            public T item;
            public int priority;

            public override bool Equals(object obj) {
                return obj is PriorityQueueItem item && Equals(item);
            }

            public bool Equals(PriorityQueueItem other) {
                return EqualityComparer<T>.Default.Equals(item, other.item) &&
                       priority == other.priority;
            }

            public override int GetHashCode() {
                return HashCode.Combine(item, priority);
            }
        }

        class PriorityQueueItemComparer : IComparer<PriorityQueueItem> {
            private static readonly Comparer<int> comparer = Comparer<int>.Default;

            public int Compare(PriorityQueueItem x, PriorityQueueItem y) {
                return comparer.Compare(x.priority, y.priority);
            }
        }

        private readonly BinaryHeap<PriorityQueueItem> heap;

        public PriorityQueue() {
            heap = new BinaryHeap<PriorityQueueItem>(new PriorityQueueItemComparer());
        }

        public int Count => heap.Count;

        public void Clear() {
            heap.Clear();
        }

        public void Add(T newItem, int priority) {
            heap.Insert(new PriorityQueueItem { item = newItem, priority = priority });
        }

        public T Peek() {
            return heap.Peek().item;
        }

        public bool TryRemoveRoot(out T root, out int priority) {
            if (heap.Count == 0) {
                priority = 0;
                root = default;
                return false;
            }
            var item = heap.RemoveRoot();
            root = item.item;
            priority = item.priority;
            return true;
        }
    }
}
