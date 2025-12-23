// <copyright file="TreeNodeViewModel.cs" company="ZEISS">
//          Copyright (c) ZEISS. All rights reserved.
// </copyright>

namespace Framework.ViewModel
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// View model for an tree of variables (root node and child nodes).
    /// </summary>
    public class TreeNodeViewModel : BaseViewModel
    {
        #region nested classes

        /// <summary>
        /// This class retrieves the string until the first point and the remaining string after the point.
        /// </summary>
        internal class PointSeparatedSplitString
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PointSeparatedSplitString" /> class.
            /// </summary>
            /// <param name="fullString">The point separated full sting.</param>
            /// <param name="separator">The separator.</param>
            internal PointSeparatedSplitString(string fullString, char separator)
            {
                // Find the first point
                int pos = fullString.IndexOf(separator);

                // Check if a point is found
                if (pos > 0)
                {
                    // This string have an point.
                    IsPoint = true;

                    // Set the first part of the string with the name until the first point.
                    FirstPart = fullString.Substring(0, pos).Trim();

                    // Set the remaining string
                    RemainingString = fullString.Substring(pos + 1, fullString.Length - pos - 1).Trim();
                }
                else
                {
                    // If no point was found set the first part to the full string, the remaining string is null and is point is false.
                    FirstPart = fullString;
                }
            }

            /// <summary>
            /// The first part of the string until the first point (without point) if point was found, the fullstring if no point was found.
            /// </summary>
            internal string FirstPart { get; set; }

            /// <summary>
            /// The remaining string after the point, if point was found, null if no point was found.
            /// </summary>
            internal string RemainingString { get; set; }

            /// <summary>
            /// True if a point was found in the full string.
            /// </summary>
            internal bool IsPoint { get; set; }
        }

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeViewModel"/> class.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        public TreeNodeViewModel(string name)
        {
            Name = name;
            Separator = '.';
        }

        #endregion

        public event EventHandler<KeyValuePair<string, string>> ValueChanged;

        #region public properties
        /// <summary>
        /// 
        /// </summary>
        public char Separator { get; set; }

        /// <summary>
        /// The name of the node.
        /// </summary>
        public string Name
        {
            get => this.Get<string>();
            private set => this.Set(value);
        }

        /// <summary>
        /// The name of the node.
        /// </summary>
        public string Value
        {
            get => this.Get<string>();
            set
            {
                this.Set(value);
                this.ValueChanged?.Invoke(this, new KeyValuePair<string, string>(this.OriginalName, this.Value));
            }
        }

        /// <summary>
        /// Gets and sets the corresponding, original full name of the tree (with all points).
        /// </summary>
        public string OriginalName
        {
            get => this.Get<string>();
            set
            {
                this.Set<string>(value);
                this.HasOriginalName = !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// True if this node have the original full qualificated, point seprateted string.
        /// </summary>
        public bool HasOriginalName
        {
            get => this.Get<bool>();
            private set => this.Set(value);
        }

        /// <summary>
        /// True if this node have children.
        /// </summary>
        public bool HasChildren
        {
            get => this.Get<bool>();
            private set => this.Set(value);
        }

        /// <summary>
        /// Collection of child nodes.
        /// </summary>
        public DispatchedObservableCollection<TreeNodeViewModel> Children
        {
            get => this.Get<DispatchedObservableCollection<TreeNodeViewModel>>();
            set => this.Set(value);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Convert a tree from a list of point separated strings.
        /// </summary>
        /// <param name="list">List of point seararted strings.</param>
        public void ConvertFromPointSeparatedStringList(Dictionary<string, string> list)
        {
            // Test
            //list = new List<string>();
            //list.Add("test.test.test");
            //list.Add("test1");
            //list.Add("test2.test");
            // Go true all point strings 
            //Dictionary<string, string> last = this.RemoveUnusedPrefix(list);
            //last = this.RemoveUnusedPrefix(last);
            //last = this.RemoveUnusedPrefix(last);
            foreach (KeyValuePair<string, string> fullItemString in list)
            {
                // Trim the string
                string trimItemString = fullItemString.Key.Trim();

                // Insert the string recursive in the tree of nodes, and retreive the created node from the deepest level
                TreeNodeViewModel node = InsertNodeFromPointSeparatedString(this, new PointSeparatedSplitString(trimItemString, Separator));

                // Set the original full qualificated, point seprateted string
                if (node != null)
                {
                    node.OriginalName = trimItemString;
                    node.Value = fullItemString.Value;
                }
            }
        }

        /// <summary>
        /// Search the child node in the tree, that correspond to the full qualificated, point separated string, (from a deep level).
        /// </summary>
        /// <param name="fullString">Full qualificated point separated string.</param>
        /// <returns>The node in the tree, that ist represented from the full qualificated point separated string.</returns>
        public TreeNodeViewModel GetLastLevelChildNodeFromFullString(string fullString)
        {
            // Create the splitted string with the part until the first point and the part after the first point.
            PointSeparatedSplitString pointSeparatedSplitString = new PointSeparatedSplitString(fullString, Separator);

            // To begin set the next sub level node to this.
            TreeNodeViewModel? nextSubLevelNode = this;

            // Loop true the point separated string.
            while (true)
            {
                // Get the child form the next sub level.
                nextSubLevelNode = nextSubLevelNode.GetFirstLevelChildNode(pointSeparatedSplitString.FirstPart);

                // Check if the child not exist, or the element have no childs.
                if (nextSubLevelNode == null)
                {
                    // Return with null, if the child form the next sub level.
                    return null;
                }

                // Split the string, in the part until the first point and the part after the first point.
                pointSeparatedSplitString = new PointSeparatedSplitString(pointSeparatedSplitString.RemainingString, Separator);

                // Check if remaining string exists.
                if (string.IsNullOrEmpty(pointSeparatedSplitString.RemainingString))
                {
                    // If remaining string (part after the first point) not exist, the node is found
                    return nextSubLevelNode.GetFirstLevelChildNode(pointSeparatedSplitString.FirstPart);
                }
            }
        }


        /// <summary>
        /// Disposing the elements and subelements.
        /// </summary>
        public Dictionary<string, string> ReadAllChildren()
        {
            Dictionary<string, string> rv = new Dictionary<string, string>();
            if (Children != null)
            {
                if (Children.Count > 0)
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        if (this.Children[i].HasChildren)
                        {
                            Dictionary<string, string> add = Children[i].ReadAllChildren();
                            foreach (KeyValuePair<string, string> child in add)
                            {
                                rv.Add(child.Key, child.Value);
                            }
                        }
                        else
                        {
                            rv.Add(this.Children[i].OriginalName, Children[i].Value);
                        }
                    }
                }
            }
            return rv;
        }

        /// <summary>
        /// Disposing the elements and subelements.
        /// </summary>
        public void DisposeChildren()
        {
            if (Children != null)
            {
                if (Children.Count > 0)
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        Children[i].DisposeChildren();
                    }
                }
                Children.Clear();
                Children = null;
            }
        }


        #endregion

        #region private methods

        /// <summary>
        /// Gets a child node if a child with the given name exist in the children collection node, or create e new node and add to the child collection.
        /// </summary>
        /// <param name="name">The name of the child, to search for.</param>
        /// <returns>The existing or new created child node.</returns>
        private TreeNodeViewModel GetChildNodeOrCreate(string name)
        {
            // This node must have a children-collection.
            if (!HasChildren)
            {
                // Create the children-collection if not exist.
                Children = new DispatchedObservableCollection<TreeNodeViewModel>(this.Dispatcher);
                HasChildren = true;
            }

            // Get the child node with the name from the chrildren-collection if exist.
            TreeNodeViewModel? childNode = this.GetFirstLevelChildNode(name);

            // Check if the node exist.
            if (childNode == null)
            {
                // Create if not exist
                childNode = new TreeNodeViewModel(name);

                // Add the child to the children-collection.
                Children.Add(childNode);
            }
            return childNode;
        }

        /// <summary>
        /// Search for an child with given name in the children-collection (only first level), if no child with this name exist, return with null.
        /// </summary>
        /// <param name="name">The name of the child to find.</param>
        /// <returns>The find node or null.</returns>
        private TreeNodeViewModel? GetFirstLevelChildNode(string name)
        {
            if (!HasChildren)
            {
                return null;
            }
            return this.Children.FirstOrDefault(o => o.Name == name);
        }

        /// <summary>
        /// Recursive method, who generates a full tree, corrsponding to the point sepatarated string, in the given node.
        /// </summary>
        /// <param name="node">The given node of the tree, from that point the tree is to be created.</param>
        /// <param name="pointSeparatedSplitString">Point seprated split string.</param>
        private TreeNodeViewModel InsertNodeFromPointSeparatedString(TreeNodeViewModel node, PointSeparatedSplitString pointSeparatedSplitString)
        {
            // Get a existing node from the children of the tree, or ccreate a new child.
            TreeNodeViewModel nextSubLevelNode = node;

            // Create the split string of the next sublevel (get the part until the point and the remaining string).
            PointSeparatedSplitString nextSubLevelPointSeparatedSplitString = pointSeparatedSplitString;

            // Check if the remaining string exists.
            if (string.IsNullOrEmpty(nextSubLevelPointSeparatedSplitString.RemainingString))
            {
                // if no remaining string exists return with the created or found child node.
                return nextSubLevelNode.GetChildNodeOrCreate(nextSubLevelPointSeparatedSplitString.FirstPart);
            }
            else
            {
                nextSubLevelNode = nextSubLevelNode.GetChildNodeOrCreate(nextSubLevelPointSeparatedSplitString.FirstPart);

                // Create the split string of the next sublevel (get the part until the point and the remaining string).
                nextSubLevelPointSeparatedSplitString = new PointSeparatedSplitString(pointSeparatedSplitString.RemainingString, Separator);

                // Call this method recursive, to build the next sublevel
                return InsertNodeFromPointSeparatedString(nextSubLevelNode, nextSubLevelPointSeparatedSplitString);
            }
        }

        private Dictionary<string, string> RemoveUnusedPrefix(Dictionary<string, string> list)
        {
            Dictionary<string, string> rv = new Dictionary<string, string>();
            if (list.Count > 0)
            {
                string first = list.ToList()[0].Key;
                string[] split = first.Split('.', StringSplitOptions.RemoveEmptyEntries);
                string prefix = string.Empty;
                if (split.Count() > 0)
                {
                    prefix = split[0];
                    bool allPrefixesEqual = true;
                    foreach (KeyValuePair<string, string> keyValuePair in list)
                    {
                        if (!keyValuePair.Key.StartsWith(prefix))
                        {
                            allPrefixesEqual = false;
                        }
                    }
                    if (allPrefixesEqual)
                    {
                        foreach (KeyValuePair<string, string> keyValuePair in list)
                        {
                            rv.Add(keyValuePair.Key.Substring(prefix.Length + 1), keyValuePair.Value);
                        }
                    }
                    else
                    {
                        return list;
                    }
                }
            }
            return rv;
        }

        #endregion
    }
}
