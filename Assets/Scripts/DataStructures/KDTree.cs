using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.DataStructures
{
    /// <summary>
    /// K-d tree implementation for fast search Unity Objects in 2d space (x and z axes are used).
    /// Search: Average O(logn) - Worst O(n).
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    internal class KDTree<T> : IEnumerable<T>, IRealtimeUpdatedCollection
        where T : Component
    {
        /// <summary>
        /// Represents a node of a KD-Tree.
        /// </summary>
        private class Node
        {
            public T Value;
            public int Level;
            public Node Left;
            public Node Right;
            public Node Next;
            public Node OldNext;

            /// <summary>
            /// Initializes a new instance of the <see cref="Node"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Node(T value) => Value = value;
        }

        // Used to traverse the tree when searching.
        private readonly Queue<Node> nodesQueue = new Queue<Node>();
        private Node root;
        private Node last;
        private float lastUpdate;
        private Node[] openedNodes;

        /// <summary>
        /// Gets or sets the current items count.
        /// </summary>
        /// <value>The items count.</value>
        public int Count { get; protected set; }

        /// <summary>
        /// Gets the <see cref="T"/> at the specified index.
        /// </summary>
        /// <value>The <see cref="T"/>.</value>
        /// <param name="index">The index.</param>
        /// <returns>The item.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index</exception>
        public T this[int index]
        {
            get
            {
                if (index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var current = root;

                for (var i = 0; i < index; i++)
                    current = current.Next;

                return current.Value;
            }
        }

        /// <summary>
        /// Add item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item) => AddNode(new Node(item));

        /// <summary>
        /// Batch add items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        /// <summary>
        /// Find all items that matches the given predicate.
        /// </summary>
        /// <param name="match">The predicate.</param>
        public KDTree<T> FindAll(Predicate<T> match)
        {
            var tree = new KDTree<T>();

            foreach (var node in this)
                if (match(node))
                    tree.Add(node);

            return tree;
        }

        /// <summary>
        /// Find first item that matches the given predicate.
        /// </summary>
        /// <param name="match">The predicate.</param>
        public T Find(Predicate<T> match)
        {
            var current = root;

            while (current != null)
            {
                if (match(current.Value))
                    return current.Value;

                current = current.Next;
            }

            return null;
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            var list = new List<Node>(GetAllNodes());
            list.RemoveAt(index);

            ClearTree();

            foreach (var node in list)
            {
                node.OldNext = null;
                node.Next = null;
            }

            foreach (var node in list)
                AddNode(node);
        }

        /// <summary>
        /// Remove all items that matches the given predicate.
        /// </summary>
        /// <param name="match">The predicate.</param>
        public void RemoveAll(Predicate<T> match)
        {
            var list = new List<Node>(GetAllNodes());
            list.RemoveAll(node => match(node.Value));

            ClearTree();

            foreach (var node in list)
            {
                node.OldNext = null;
                node.Next = null;
            }

            foreach (var node in list)
                AddNode(node);
        }

        /// <summary>
        /// Count all items that matches the given predicate.
        /// </summary>
        /// <param name="match">The predicate.</param>
        /// <returns>Matching object count.</returns>
        public int CountAll(Predicate<T> match)
        {
            return this.Count(node => match(node));
        }

        /// <summary>
        /// Clear the tree.
        /// </summary>
        public void ClearTree()
        {
            root = null;
            last = null;
            Count = 0;
        }

        /// <summary>
        /// Update item positions with rebuilding tree a specified number of times per second.
        /// </summary>
        /// <param name="rate">Updates per second.</param>
        public void Update(float rate)
        {
            if (rate <= 0)
                throw new ArgumentOutOfRangeException(nameof(rate), "Must be greater than 0.");

            if (Time.timeSinceLevelLoad - lastUpdate < 1f / rate)
                return;

            lastUpdate = Time.timeSinceLevelLoad;
            UpdatePositions();
        }

        /// <summary>
        /// Update item positions with rebuilding tree.
        /// </summary>
        public void UpdatePositions()
        {
            var current = root;

            while (current != null)
            {
                current.OldNext = current.Next;
                current = current.Next;
            }

            current = root;
            ClearTree();

            while (current != null)
            {
                AddNode(current);
                current = current.OldNext;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The yield instruction.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            var current = root;

            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Convert to list.
        /// </summary>
        /// <returns>Item list.</returns>
        public List<T> ToList()
        {
            var list = new List<T>();

            foreach (var node in this)
                list.Add(node);

            return list;
        }

        /// <summary>Gets the quick distance (without sqrt).</summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The distance.</returns>
        protected float GetDistance(Vector3 a, Vector3 b)
        {
            return (a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z);
        }

        /// <summary>
        /// Gets the split value.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="position">The position.</param>
        /// <returns>The splitted by level position value.</returns>
        protected float GetSplitValue(int level, Vector3 position)
        {
            return (level % 2 == 0) ? position.x : position.z;
        }

        /// <summary>
        /// Adds the node.
        /// </summary>
        /// <param name="newNode">The new node.</param>
        private void AddNode(Node newNode)
        {
            Count++;
            newNode.Left = null;
            newNode.Right = null;
            newNode.Level = 0;
            var parent = FindParent(newNode.Value.transform.position);

            if (last != null)
                last.Next = newNode;

            last = newNode;

            if (parent == null)
            {
                root = newNode;
                return;
            }

            newNode.Level = parent.Level + 1;

            var splitParent = GetSplitValue(parent.Level, parent.Value.transform.position);
            var splitNew = GetSplitValue(parent.Level, newNode.Value.transform.position);

            if (splitNew < splitParent)
                parent.Left = newNode; //go left
            else
                parent.Right = newNode; //go right
        }

        /// <summary>
        /// Finds the parent.
        /// </summary>
        /// <param name="position">The point.</param>
        /// <returns>The parent node.</returns>
        private Node FindParent(Vector3 position)
        {
            var current = root;
            var parent = root;

            while (current != null)
            {
                parent = current;

                var splitCurrent = GetSplitValue(current.Level, current.Value.transform.position);
                var splitSearch = GetSplitValue(current.Level, position);

                if (splitSearch < splitCurrent)
                    current = current.Left; //go left
                else
                    current = current.Right; //go right
            }

            return parent;
        }

        /// <summary>
        /// Find closest item to given position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The closest object.</returns>
        public T FindClosest(Vector3 position)
        {
            if (root == null)
                return null;

            var nearestDist = float.MaxValue;
            Node nearest = null;

            if (openedNodes == null || openedNodes.Length < Count)
                openedNodes = new Node[Count];

            for (int i = 0; i < openedNodes.Length; i++)
                openedNodes[i] = null;

            var openAdd = 0;
            var openCur = 0;

            if (root != null)
                openedNodes[openAdd++] = root;

            while (openCur < openedNodes.Length && openedNodes[openCur] != null)
            {
                var current = openedNodes[openCur++];
                var nodeDist = GetDistance(position, current.Value.transform.position);

                if (nodeDist < nearestDist)
                {
                    nearestDist = nodeDist;
                    nearest = current;
                }

                var splitCurrent = GetSplitValue(current.Level, current.Value.transform.position);
                var splitSearch = GetSplitValue(current.Level, position);

                if (splitSearch < splitCurrent)
                {
                    if (current.Left != null)
                        openedNodes[openAdd++] = current.Left; //go left
                    if (current.Right != null && Mathf.Abs(splitCurrent - splitSearch) * Mathf.Abs(splitCurrent - splitSearch) < nearestDist)
                        openedNodes[openAdd++] = current.Right; //go right
                }
                else
                {
                    if (current.Right != null)
                        openedNodes[openAdd++] = current.Right; //go right
                    if (current.Left != null && Mathf.Abs(splitCurrent - splitSearch) * Mathf.Abs(splitCurrent - splitSearch) < nearestDist)
                        openedNodes[openAdd++] = current.Left; //go left
                }
            }

            return nearest?.Value;
        }

        /// <summary>
        /// Gets all nodes.
        /// </summary>
        /// <returns>The nodes.</returns>
        private IEnumerable<Node> GetAllNodes()
        {
            var current = root;
            while (current != null)
            {
                yield return current;
                current = current.Next;
            }
        }

        /// <summary>
        /// Find all items in a given region.
        /// </summary>
        /// <param name="min">Lower coordinate.</param>
        /// <param name="max">Higher coordinate.</param>
        /// <returns>The finded items.</returns>
        public IEnumerable<T> GetByRegion(Vector3 min, Vector3 max)
        {
            var output = new List<T>();
            var pointChecker = new PointCheckerInRectangle(min, max);
            RangeSearch(min, max, pointChecker, output);

            return output;
        }

        /// <summary>
        /// Find all items in a given circle area.
        /// </summary>
        /// <param name="center">Circle center point.</param>
        /// <param name="radius">Circle radius.</param>
        /// <returns>The finded items.</returns>
        public IEnumerable<T> GetByCircleArea(Vector3 center, float radius)
        {
            var output = new List<T>();
            var pointChecker = new PointCheckerInCircle(center, radius);
            var min = new Vector3(center.x - radius, center.y, center.z - radius);
            var max = new Vector3(center.x + radius, center.y, center.z + radius);
            RangeSearch(min, max, pointChecker, output);

            return output;
        }

        /// <summary>
        /// The range search.
        /// </summary>
        /// <typeparam name="TChecker">The type of the checker.</typeparam>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// /// <param name="pointChecker">The point checker.</param>
        /// <param name="output">The finded items.</param>
        private void RangeSearch<TChecker>(Vector3 min, Vector3 max, TChecker pointChecker, List<T> output)
            where TChecker : IPointChecker
        {
            if (root == null)
                return;

            nodesQueue.Clear();
            nodesQueue.Enqueue(root);

            while (nodesQueue.Count > 0)
            {
                var node = nodesQueue.Dequeue();

                var splitMin = GetSplitValue(node.Level, min);
                var splitMax = GetSplitValue(node.Level, max);
                var splitNode = GetSplitValue(node.Level, node.Value.transform.position);

                var point = node.Value.transform.position;

                if (pointChecker.IsInArea(point))
                    output.Add(node.Value);

                if (splitMin < splitNode && node.Left != null)
                    nodesQueue.Enqueue(node.Left);

                if (splitMax >= splitNode && node.Right != null)
                    nodesQueue.Enqueue(node.Right);
            }

            nodesQueue.Clear();
        }
    }
}
