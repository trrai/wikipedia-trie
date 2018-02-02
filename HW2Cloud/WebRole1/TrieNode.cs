using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class TrieNode
    {
        public List<TrieNode> Children { get; set; }
        public char NodeValue { get; set; }
        public TrieNode Parent { get; set; }
        public int NodeDepth { get; set; }

        public TrieNode(TrieNode parent, char value, int depth)
        {
            Children = new List<TrieNode>();
            NodeValue = value;
            Parent = parent;
            NodeDepth = depth;

        }

        public TrieNode SearchChildren(char searchChar)
        {
            foreach (var child in Children)
            {
                if (child.NodeValue == searchChar)
                {
                    return child;
                }

            }

            return null;
        }

        public bool isLeafNode()
        {
            if (Children.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}