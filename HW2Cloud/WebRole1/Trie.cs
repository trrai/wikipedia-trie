using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace WebRole1
{
    public class Trie
    {
        private TrieNode rootNode;

        public Trie()
        {
            rootNode = new TrieNode(null, '^', 0);
        }

        public TrieNode PrefixNode(string input)
        {
            var currentNode = rootNode;
            var returnNode = currentNode;

            foreach (var currentChar in input)
            {
                currentNode = currentNode.SearchChildren(currentChar);
                if (currentNode == null)
                {
                    break;
                }

                returnNode = currentNode;
            }

            return returnNode;
        }

        public void InsertString(string input)
        {
            var preNode = PrefixNode(input);
            var currentNode = preNode;

            for (var i = currentNode.NodeDepth; i < input.Length; i++)
            {
                var newNode = new TrieNode(currentNode, input[i], currentNode.NodeDepth + 1);
                currentNode.Children.Add(newNode);
                currentNode = newNode;

            }

            var endingNode = new TrieNode(currentNode, '$', currentNode.NodeDepth + 1);
            currentNode.Children.Add(endingNode);

        }

        public List<string> Search(string input)
        {

            //this might be the same thing as prefix yo 
            var currentNode = rootNode;
            //System.Diagnostics.Debug.WriteLine(input);
            foreach (var c in input)
            {
                //System.Diagnostics.Debug.WriteLine("Character:" + c);
                var newNode = currentNode.SearchChildren(c);
                //System.Diagnostics.Debug.WriteLine("Node: " + newNode);
                if (newNode == null)
                {
                    return null;
                }
                currentNode = newNode;
            }

            List<string> returnList = new List<string>();


            dfs(returnList, currentNode, input.Remove(input.Length - 1));

            /*
            var returnString = "";
            foreach(var word in returnList)
            {
                returnString = returnString + word + " | ";
            }
            */


            return returnList;
        }

        public void dfs(List<string> returnList, TrieNode node, string input)
        {
            //System.Diagnostics.Debug.WriteLine(input);
            if (returnList.Count() <= 10)
            {
                if (node.NodeValue == '$')
                {
                    //System.Diagnostics.Debug.WriteLine("------" + input + "------");
                    if (!returnList.Contains(input))
                    {
                        returnList.Add(input);
                    }
                }
                else
                {
                    input = input + node.NodeValue;
                }

                foreach (var child in node.Children)
                {
                    if (child != null)
                    {
                        dfs(returnList, child, input);
                    }

                }
            }

        }

    }
}