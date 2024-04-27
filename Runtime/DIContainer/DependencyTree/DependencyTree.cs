using System.Collections.Generic;
using System.Linq;
using UJect.Exceptions;
using UJect.Injection;

namespace UJect
{
    internal class DependencyTree
    {
        private readonly Dictionary<InjectionKey, DependencyNode> nodeLookup = new();
        private readonly HashSet<DependencyNode>                  roots      = new();
        private IEnumerable<DependencyNode> OrderedRoots => roots.OrderBy(n => n.InjectionKey);

        internal IEnumerable<InjectionKey> RootKeys => OrderedRoots.Select(dn=>dn.InjectionKey);

        private List<InjectionKey> cachedSortedList;

        private DependencyNode GetOrCreateNode(InjectionKey key, out bool created)
        {
            created = false;
            if (!nodeLookup.TryGetValue(key, out var node))
            {
                node            = new DependencyNode(key);
                nodeLookup[key] = node;
                created         = true;
                AddInjectorDependencies(key);
            }

            return node;
        }
        
        private void AddInjectorDependencies(InjectionKey injected)
        {
            var injector = InjectorCache.GetOrCreateInjector(injected.InjectedResourceType);
            foreach (var dependsOn in injector.DependsOn)
            {
                AddDependency(injected, dependsOn, false);
            }
        }

        /// <summary>
        /// Add a dependency.
        ///
        /// dependsOn will depend on dependency. Any subsequent dependencies will subsequently be added.
        /// </summary>
        /// <param name="dependsOn"></param>
        /// <param name="dependency"></param>
        internal void AddDependency(InjectionKey dependsOn, InjectionKey dependency) => AddDependency(dependsOn, dependency, true);
        
        private void AddDependency(InjectionKey dependsOn, InjectionKey dependency, bool isRoot)
        {
            var sourceNode = GetOrCreateNode(dependsOn);
            //If a concrete class is bound to an implementation of itself, these will be equal. No need to track a self-referencing dependency
            if (!dependsOn.Equals(dependency))
            {
                sourceNode.AddDependsOn(GetOrCreateNode(dependency));
                cachedSortedList = null;
            }

            if (isRoot)
            {
                roots.Add(sourceNode);
                cachedSortedList = null;
            }
        }

        private DependencyNode GetOrCreateNode(InjectionKey key) => GetOrCreateNode(key, out _);

        internal bool HasCycle(out InjectionKey onDependency)
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

            foreach (var root in OrderedRoots)
            {
                stack.Push(root);
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
                        else if (GetNodeState(dependsOn) == NodeState.Open)
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

        internal IEnumerable<InjectionKey> Sorted()
        {
            if (cachedSortedList == null)
            {
                cachedSortedList = TopologicSort(OrderedRoots)
                    .Select(n => n.InjectionKey)
                    .ToList();
            }

            return cachedSortedList;
        }


        private static List<DependencyNode> TopologicSort(IEnumerable<DependencyNode> orderedRoots)
        {
            var stack = new Stack<StackEntry>();
            var visited = new HashSet<DependencyNode>();
            var sortedList = new List<DependencyNode>();

            foreach (var rootNode in orderedRoots)
            {
                stack.Push(new StackEntry {Node = rootNode, IsParent = false});
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
                    stack.Push(new StackEntry {Node = next, IsParent = true});
                }

                foreach (var dOn in next.DependsOn.OrderBy(d => d.InjectionKey))
                {
                    if (visited.Contains(dOn))
                    {
                        continue;
                    }

                    stack.Push(new StackEntry { Node = dOn, IsParent = false });
                }
            }

            return sortedList;
        }

        internal IEnumerable<InjectionKey> GetDependenciesFor(InjectionKey key)
        {
            return GetOrCreateNode(key).DependsOn.Select(d => d.InjectionKey);
        }

        private class DependencyNode
        {
            public readonly InjectionKey InjectionKey;

            public DependencyNode(InjectionKey injectionKey)
            {
                InjectionKey = injectionKey;
            }

            internal HashSet<DependencyNode> DependsOn { get; } = new();

            public void AddDependsOn(DependencyNode node)
            {
                DependsOn.Add(node);
            }

            public override string ToString()
            {
                return $"Node [{InjectionKey}]";
            }
        }

        private struct StackEntry
        {
            public DependencyNode Node;
            public bool           IsParent;
        }

        private enum NodeState : byte
        {
            Unvisited = 0,
            Open      = 1,
            Closed    = 2
        }
    }
}