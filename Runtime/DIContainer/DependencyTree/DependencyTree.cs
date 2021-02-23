using System.Collections.Generic;
using System.Linq;

namespace UJect
{
    public class DependencyTree
    {
        private readonly Dictionary<DiContainer.InjectionKey, DependencyNode> nodeLookup = new Dictionary<DiContainer.InjectionKey, DependencyNode>();
        private readonly HashSet<DiContainer.InjectionKey>                    roots      = new HashSet<DiContainer.InjectionKey>();

        private class DependencyNode
        {
            public readonly  DiContainer.InjectionKey InjectionKey;
            private readonly HashSet<DependencyNode>  dependsOn = new HashSet<DependencyNode>();

            public DependencyNode(DiContainer.InjectionKey injectionKey)
            {
                this.InjectionKey = injectionKey;
            }

            internal HashSet<DependencyNode> DependsOn => dependsOn;

            public void AddDependsOn(DependencyNode node)
            {
                this.dependsOn.Add(node);
            }

            public override string ToString()
            {
                return $"Node [{InjectionKey}]";
            }
        }

        internal void AddDependency(DiContainer.InjectionKey source, DiContainer.InjectionKey on, bool isRoot)
        {
            //If a concrete class is bound to an implementation of itself, these will be equal. No need to track a self-referencing dependency
            if (!source.Equals(on))
            {
                GetOrCreateNode(source).AddDependsOn(GetOrCreateNode(on));
                cachedSortedList = null;
            }

            if (isRoot)
            {
                roots.Add(source);
                cachedSortedList = null;
            }
        }

        private DependencyNode GetOrCreateNode(DiContainer.InjectionKey key)
        {
            if (!nodeLookup.TryGetValue(key, out var node))
            {
                node            = new DependencyNode(key);
                nodeLookup[key] = node;
            }

            return node;
        }

        private struct StackEntry
        {
            public DependencyNode Node;
            public bool           IsParent;
        }

        private enum NodeState:byte
        {
            Unvisited = 0,
            Open = 1,
            Closed = 2,
        }
        
        internal bool HasCycle(out DiContainer.InjectionKey onDependency)
        {
            onDependency = default;
            var stack = new Stack<DependencyNode>();
            var visited = new HashSet<DependencyNode>();
            var nodeStates = new Dictionary<DependencyNode, NodeState>();

            NodeState GetNodeState(DependencyNode node)
            {
                if (nodeStates.TryGetValue(node, out var v))
                {
                    return v;
                }

                return NodeState.Unvisited;
            }

            foreach (var root in roots)
            {
                stack.Push(GetOrCreateNode(root));
            }

            while (stack.Count != 0)
            {
                var current = stack.Peek();
                if (visited.Add(current))
                {
                    nodeStates[current] = NodeState.Open;
                    foreach (var dependsOn in current.DependsOn)
                    {
                        if (!visited.Contains(dependsOn))
                        {
                            stack.Push(dependsOn);
                        }
                        else if(GetNodeState(dependsOn) == NodeState.Open)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    nodeStates[current] = NodeState.Closed;
                    stack.Pop();
                }
            }

            return false;
        }


        private List<DiContainer.InjectionKey> cachedSortedList = null;
        internal IEnumerable<DiContainer.InjectionKey> Sorted()
        {
            if (cachedSortedList == null)
            {
                cachedSortedList = TopologicSort(roots.Select(GetOrCreateNode).ToArray())
                    .Select(n => n.InjectionKey)
                    .ToList();
            }
            return cachedSortedList;
        }


        private List<DependencyNode> TopologicSort(params DependencyNode[] roots)
        {
            var stack = new Stack<StackEntry>();
            var visited = new HashSet<DependencyNode>();
            var sortedList = new List<DependencyNode>();

            foreach (var rootNode in roots)
            {
                stack.Push(new StackEntry() {Node = rootNode, IsParent = false});
            }

            while (stack.Count > 0)
            {
                var nextEntry = stack.Pop();
                var next = nextEntry.Node;
                var isParent = nextEntry.IsParent;
                if (isParent)
                {
                    sortedList.Add(next);
                    continue;
                }

                if (visited.Add(next))
                {
                    stack.Push(new StackEntry() {Node = next, IsParent = true});
                }

                foreach (var dOn in next.DependsOn)
                {
                    if (visited.Contains(dOn))
                    {
                        continue;
                    }

                    stack.Push(new StackEntry() {Node = dOn, IsParent = false});
                }
            }

            return sortedList;
        }

        internal IEnumerable<DiContainer.InjectionKey> GetDependenciesFor(DiContainer.InjectionKey key)
        {
            return GetOrCreateNode(key).DependsOn.Select(d => d.InjectionKey);
        }


    }
}