using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Collection
{
    // AVL Binary Search Tree Implementation - https://en.wikipedia.org/wiki/AVL_tree
    //
    // - O(log n) Insert, Delete, Search
    // - Self balancing
    // 

    /// <summary>
    /// AVL Binary Search Tree implementation that gets the key from the value node by use of an 
    /// indexer delegate. Also, uses a user supplied comparer to compare two node keys. The keys are
    /// supplied by the indexer from the user.
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="T">Node type</typeparam>
    public class BinarySearchTree<K, T> where T : class where K : IComparable<K>
    {
        ///// <summary>
        ///// Delegate used to compare two node keys of the binary search tree. MUST RETURN -1, +1, or 0
        ///// </summary>
        //public delegate int BinarySearchTreeComparer(K key1, K key2);

        //readonly BinarySearchTreeComparer _comparer;

        // Track values to help visualize when debugging
        Dictionary<K, T> _valueDict;

        // Root node
        BinarySearchTreeNode _root;

        public int Count
        {
            get { return _valueDict.Count; }
        }

        protected class BinarySearchTreeNode
        {
            public BinarySearchTreeNode Parent { get; set; }
            public BinarySearchTreeNode LeftChild { get; set; }
            public BinarySearchTreeNode RightChild { get; set; }
            public K Key { get; set; }
            public T Value { get; set; }
            
            public int BalanceFactor
            {
                get { return GetDepth(this.RightChild) - GetDepth(this.LeftChild); }
            }

            private int GetDepth(BinarySearchTreeNode node)
            {
                if (node == null)
                    return 0;

                else if (node.LeftChild == null &&
                         node.RightChild == null)
                         return 1;
                else
                    return System.Math.Max(GetDepth(node.LeftChild), GetDepth(node.RightChild));
            }

            /// <summary>
            /// Sets the depth based on the parent node
            /// </summary>
            public BinarySearchTreeNode(BinarySearchTreeNode parent, K key, T value)
            {
                this.Parent = parent;
                this.Key = key;
                this.Value = value;
            }
        }

        public BinarySearchTree()
        {
            _valueDict = new Dictionary<K, T>();
        }

        public void Insert(K key, T value)
        {
            if (_root == null)
                _root = new BinarySearchTreeNode(null, key, value);

            else
            {
                // Perform insertion
                var node = InsertImpl(_root, key, value);

                // Use node to traverse the tree to re-balance it
                var heavyNode = Retrace(node);

                if (heavyNode != null)
                    Balance(heavyNode);
            }

            // Track the values for debugging
            _valueDict.Add(key, value);
        }

        public void Remove(K key)
        {
            if (_root == null)
                throw new Exception("Node not found in the tree BinarySearchTree");

            else
            {
                // Perform search
                var node = SearchImpl(_root, key);

                // Perform removal
                if (node != null)
                {
                    // Remove node and return node for retracing and balancing
                    var lowestAffectedNode = RemovalImpl(node);

                    // Retrace
                    var heavyNode = Retrace(lowestAffectedNode);

                    // Balance
                    if (heavyNode != null)
                        Balance(heavyNode);
                }
                else
                    throw new Exception("Node not found in the tree BinarySearchTree");
            }

            // Track the values for debugging
            _valueDict.Remove(key);
        }

        public T Search(K key)
        {
            // Utilize dictionary for O(1) lookup
            if (_valueDict.ContainsKey(key))
                return _valueDict[key];

            else
                return null;
        }

        public T Min()
        {
            return MinImpl(_root)?.Value ?? null;
        }
        public K MinKey()
        {
            var minNode = MinImpl(_root);

            if (minNode == null)
                throw new Exception("Trying to resolve min key from an empty Binary Search Tree");

            else
                return minNode.Key;
        }

        public T Max()
        {
            return MaxImpl(_root)?.Value ?? null;
        }
        public K MaxKey()
        {
            var maxNode = MaxImpl(_root);

            if (maxNode == null)
                throw new Exception("Trying to resolve max key from an empty Binary Search Tree");

            else
                return maxNode.Key;
        }

        /// <summary>
        /// Performs insertion of the node recursively and returns the node that was inserted. Returns null if insertion fails.
        /// </summary>
        private BinarySearchTreeNode InsertImpl(BinarySearchTreeNode node, K key, T value)
        {
            var comparison = key.CompareTo(node.Key);

            // Insert Left
            if (comparison < 0)
            {
                // Recurse Left
                if (node.LeftChild != null)
                    return InsertImpl(node.LeftChild, key, value);

                // Create Left Child Node
                else
                {
                    node.LeftChild = new BinarySearchTreeNode(node, key, value);
                    return node.LeftChild;
                }
            }

            // Insert Right
            else if (comparison > 0)
            {
                // Recurse Right
                if (node.RightChild != null)
                    return InsertImpl(node.RightChild, key, value);

                // Create Right Child Node
                else
                {
                    node.RightChild = new BinarySearchTreeNode(node, key, value);
                    return node.RightChild;
                }
            }

            else
                throw new Exception("Duplicate key insertion BinarySearchTree");

            return null;
        }

        /// <summary>
        /// Performs a search on the tree for the given value. Returns null if search fails
        /// </summary>
        private BinarySearchTreeNode SearchImpl(BinarySearchTreeNode node, K key)
        {
            if (node == null)
                return null;

            var comparison = key.CompareTo(node.Key);

            // Search Left
            if (comparison < 0)
                return SearchImpl(node.LeftChild, key);

            // Search Right
            else if (comparison > 0)
                return SearchImpl(node.RightChild, key);

            // Node found
            else
                return node;
        }

        /// <summary>
        /// Gets the next successor from the tree - used during removal. Start with the node being removed.
        /// </summary>
        private BinarySearchTreeNode SearchSuccessorImpl(BinarySearchTreeNode node, K key)
        {
            if (node == null)
                return null;

            var comparison = key.CompareTo(node.Key);

            // Key is smaller -> check to see if there are any smaller nodes still greater than the key
            if (comparison < 0)
            {
                if (node.LeftChild != null)
                    return SearchSuccessorImpl(node.LeftChild, key) ?? node;
            }
            else if (comparison > 0)
                throw new Exception("Improper use of SearchSuccessorImpl starting node");

            // The nodes are equal - so start search with the right child
            else
                return SearchSuccessorImpl(node.RightChild, key);

            return null;
        }

        /// <summary>
        /// Returns the first node up the tree that has a balance factor of -2 or +2
        /// </summary>
        private BinarySearchTreeNode Retrace(BinarySearchTreeNode node)
        {
            // No heavy node found - return null
            if (node == null)
                return null;

            var balanceFactor = node.BalanceFactor;

            // Heavy node found - return it for balancing
            if (System.Math.Abs(balanceFactor) == 2)
                return node;

            else if (System.Math.Abs(balanceFactor) > 1)
                throw new Exception("Invalid Balance Factor Binary Search Tree");

            // Node not heavy - trace up the tree
            else
                return Retrace(node.Parent);
        }

        /// <summary>
        /// Removes node from the tree using next-successor replacement (see procedure)
        /// </summary>
        private BinarySearchTreeNode RemovalImpl(BinarySearchTreeNode node)
        {
            // Procedure
            //
            // Case 1:  Node has zero children -> Remove the node
            // Case 2:  Node has one child -> Make the child node this node
            // Case 3:  Node has two children -> Search for immediate successor
            //          
            //          Case 3a:  Successor has no children -> Copy successor to this node -> Remove successor
            //          Case 3b:  Successor has one child -> Copy successor to this node ->  Make the successor's child 
            //                    the successor and remove the successor's reference
            //          Case 3c:  (Not possible)
            //
            // After this, return the lowest level affected node to retrace and balance the tree
            //

            // Case 1
            if (node.LeftChild == null &&
                node.RightChild == null)
            {
                // Node is the root
                if (node == _root)
                {
                    _root = null;
                    return null;
                }

                if (node.Parent.LeftChild == node)
                    node.Parent.LeftChild = null;

                else
                    node.Parent.RightChild = null;

                return node.Parent;
            }

            // Case 3
            else if (node.RightChild != null &&
                     node.LeftChild != null)
            {
                // Get the immediate successor
                var successor = SearchSuccessorImpl(node, node.Key);

                // Copy successor value to node
                node.Value = successor.Value;
                node.Key = successor.Key;

                // Case 3a
                if (successor.LeftChild == null &&
                    successor.RightChild == null)
                {
                    // Remove successor
                    if (successor.Parent.LeftChild == successor)
                        successor.Parent.LeftChild = null;

                    else
                        successor.Parent.RightChild = null;
                }

                // Case 3b
                else
                {
                    // Successor can NOT have a left child because it would itself be the successor
                    if (successor.LeftChild != null)
                        throw new Exception("Improper use of successor removal");

                    // Successor's right child now becomes the successor
                    else
                    {
                        // Copy successor's child's reference to successor's parent
                        if (successor.Parent.LeftChild == successor)
                            successor.Parent.LeftChild = successor.RightChild;

                        else
                            successor.Parent.RightChild = successor.RightChild;

                        // Set up successor's child with new parent reference
                        successor.RightChild.Parent = successor.Parent;
                    }
                }

                // Return the successor's parent for re-balancing
                return successor.Parent;
            }

            // Case 2
            else if (node.RightChild != null ||
                     node.LeftChild != null)
            {
                var newChildNode = node.RightChild ?? node.LeftChild;

                // Node is the root
                if (node == _root)
                {
                    // Set up new root
                    _root = newChildNode;

                    // Set up the parent node
                    newChildNode.Parent = null;

                    // Return root for balancing check
                    return newChildNode;
                }

                // Node is not the root
                else
                {
                    if (node.Parent.LeftChild == node)
                        node.Parent.LeftChild = newChildNode;

                    if (node.Parent.RightChild == node)
                        node.Parent.RightChild = newChildNode;

                    // Set the new child node's parent
                    newChildNode.Parent = node.Parent;

                    return newChildNode.Parent;
                }
            }

            else
                throw new Exception("Improperly handled Removal implementation BinarySearchTree");
        }

        /// <summary>
        /// Returns the lowest value node in the tree
        /// </summary>
        private BinarySearchTreeNode MinImpl(BinarySearchTreeNode node)
        {
            if (node == null)
                return null;

            return MinImpl(node.LeftChild) ?? node;
        }

        /// <summary>
        /// Returns the greatest value node in the tree
        /// </summary>
        private BinarySearchTreeNode MaxImpl(BinarySearchTreeNode node)
        {
            if (node == null)
                return null;

            return MaxImpl(node.RightChild) ?? node;
        }

        /// <summary>
        /// Performs tree balancing using the lowest ancestor with a balancing factor of +2 or -2
        /// </summary>
        /// <param name="heavyNode">Tree node with + or - 2 balancing factor</param>
        private void Balance(BinarySearchTreeNode heavyNode)
        {
            // Procedure - https://en.wikipedia.org/wiki/AVL_tree
            //
            // Examine situation with heavy node and direct child:
            //
            // Let: heavyNode = X, and nextNode = Z. (Next node would be left or right depending on BalanceFactor(Z))

            /* 
 	            Right Right  => Z is a right child of its parent X and Z is  not left-heavy 	(i.e. BalanceFactor(Z) ≥ 0)  (Rotate Left)
	            Left  Left 	 => Z is a left  child of its parent X and Z is  not right-heavy    (i.e. BalanceFactor(Z) ≤ 0)  (Rotate Right)
	            Right Left 	 => Z is a right child of its parent X and Z is  left-heavy 	    (i.e. BalanceFactor(Z) = −1) (Double Rotate RightLeft)
	            Left  Right  => Z is a left  child of its parent X and Z is  right-heavy 	    (i.e. BalanceFactor(Z) = +1) (Double Rotate LeftRight)
            */

            if (heavyNode.BalanceFactor == 2)
            {
                // Right Right
                if (heavyNode.RightChild.BalanceFactor >= 0)
                {
                    RotateLeft(heavyNode.RightChild);
                }

                // Right Left
                else
                {
                    // See diagram (https://en.wikipedia.org/wiki/AVL_tree#/media/File:AVL-double-rl_K.svg)
                    RotateRight(heavyNode.RightChild.LeftChild);
                    RotateLeft(heavyNode.RightChild);
                }
            }
            else if (heavyNode.BalanceFactor == -2)
            {
                // Left Left
                if (heavyNode.LeftChild.BalanceFactor <= 0)
                {
                    RotateRight(heavyNode.LeftChild);
                }

                // Left Right
                else
                {
                    RotateLeft(heavyNode.LeftChild.RightChild);
                    RotateRight(heavyNode.LeftChild);
                }
            }
            else
                throw new Exception("Improper use of tree balancing BinarySearchTree");
        }

        /// <summary>
        /// Performs a left rotation on the sub-tree starting with node
        /// </summary>
        private void RotateLeft(BinarySearchTreeNode node)
        {
            // Procedure - https://en.wikipedia.org/wiki/AVL_tree
            //
            // - Pre-condition:  Node must have an un-balanced parent; Node is the right-child of the parent
            // - Node's left child becomes node's parent's right child
            // - Node's parent becomes the left child of node
            // - Node's parent's parent becomes node's parent
            //

            if (node.Parent == null)
                throw new Exception("Invalid RotateLeft pre-condition BinarySearchTree");

            if (node.Parent.RightChild != node)
                throw new Exception("Invalid RotateLeft pre-condition BinarySearchTree");

            // Refering to variables from Wikipedia entry
            var G = node.Parent.Parent ?? null;
            var X = node.Parent;
            var Z = node;
            var T = node.LeftChild ?? null;

            // Node's left child becomes node's parent's right child:  X -> T
            X.RightChild = T;
            T.Parent = X;

            // Node's parent becomes the left child of node:  X <- Z
            Z.LeftChild = X;
            X.Parent = Z;

            // Node's parent's parent becomes node's parent:  -> Z
            Z.Parent = G;

            // Setup grand-parent reference
            if (G != null)
            {
                if (G.LeftChild == X)
                    G.LeftChild = Z;

                else
                    G.RightChild = Z;
            }
        }

        /// <summary>
        /// Performs a left rotation on the sub-tree starting with node
        /// </summary>
        private void RotateRight(BinarySearchTreeNode node)
        {
            // Procedure - https://en.wikipedia.org/wiki/AVL_tree
            //
            // - Pre-condition:  Node must have an un-balanced parent; Node is the left-child of the parent
            // - Node's right child becomes node's parent's left child
            // - Node's parent becomes the right child of node
            // - Node's parent's parent becomes node's parent
            //

            if (node.Parent == null)
                throw new Exception("Invalid RotateRight pre-condition BinarySearchTree");

            if (node.Parent.LeftChild != node)
                throw new Exception("Invalid RotateRight pre-condition BinarySearchTree");

            // Refering to variables from Wikipedia entry
            var G = node.Parent.Parent ?? null;
            var X = node.Parent;
            var Z = node;
            var T = node.RightChild ?? null;

            // Node's right child becomes node's parent's left child:  T <- X
            X.LeftChild = T;
            T.Parent = X;

            // Node's parent becomes the right child of node:  Z -> X
            Z.RightChild = X;
            X.Parent = Z;

            // Node's parent's parent becomes node's parent:  -> Z
            Z.Parent = G;

            // Setup grand-parent reference
            if (G != null)
            {
                if (G.LeftChild == X)
                    G.LeftChild = Z;

                else
                    G.RightChild = Z;
            }
        }
    }
}
