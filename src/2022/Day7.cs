using System.Text;
using Parsing.StringReading;

namespace advent_of_code.y2022 {
    class Day7 : AdventDay {
        public override void SolvePart1(string[] inputLines) {
            var treeReader = CreateTreeReader(inputLines);

            int counter = 0;
            long totalSize = 0;
            Console.WriteLine("tree reader initialized");
            const int maxSize = 100_000;
            var allEntries = treeReader.EnumerateEntries();

            counter = LogTree(allEntries);

            long sizeAggregator = 0;
            totalSize = allEntries.OfType<IFileTreeNode>()
                .Aggregate(sizeAggregator, (tSize, node) => {
                    var size = node.ComputeSize();
                    if (size <= maxSize) {
                        tSize += size;
                    }
                    return tSize;
                });

            Console.WriteLine("ended. {0} counted, {1} in tree; total size: {2}",
                counter, treeReader?.Count, totalSize);
        }

        public override void SolvePart2(string[] inputLines) {
            const long totalSpace = 70_000_000;
            const long requredSpace = 30_000_000;

            var tree = CreateTreeReader(inputLines);
            var totalUsedSpace = tree.Root.ComputeSize();

            var freeSpace = totalSpace - totalUsedSpace;
            var requiredToClean = requredSpace - freeSpace;

            var allDirs = tree.EnumerateEntries()
                .OfType<IFileTreeNode>()
                .Where(dir => dir.ComputeSize() >= requiredToClean)
                .ToList();

            allDirs.Sort((x, y) => x.ComputeSize().CompareTo(y.ComputeSize()));

            Console.WriteLine();
            LogTree(allDirs);
            Console.WriteLine("better delete folder {0}", allDirs.First());
        }

        private static FileTreeReader CreateTreeReader(string[] inputLines) {
            FileTreeReader? treeReader = null;
            foreach (string line in inputLines) {
                ReadLine(line, ref treeReader);
            }
            Console.WriteLine();
            return treeReader ?? throw new InvalidDataException();
        }

        static void ReadLine(string line, ref FileTreeReader? treeReader) {
            var wordReader = new WordReader(line);
            var firstWord = wordReader.ReadWord();

            if (firstWord == "$") {
                var command = wordReader.ReadWord();
                switch (command) {
                    case "cd":
                        var name = wordReader.ReadWord();
                        if (treeReader == null) {
                            treeReader = new FileTreeReader(new DirectoryNode(name));
                        } else if (name == "..") {
                            treeReader.MoveUp();
                        } else {
                            treeReader.MoveDown(name);
                        }
                        break;
                    case "ls":
                        // enumerate
                        break;
                }
            } else if (treeReader == null) {
                Console.WriteLine("ERROR");
            } else if (firstWord == "dir") {
                var dir = new DirectoryNode(wordReader.ReadWord());
                treeReader.IncludeContent(dir);
            } else if (firstWord.All(char.IsDigit)) {
                var size = int.Parse(firstWord);
                var name = wordReader.ReadWord();
                treeReader.IncludeContent(new FileNode(name, size));
            }
        }

        static int LogTree(IEnumerable<IFileTreeEntry> entries) {
            int count = 0;
            foreach (var entry in entries) {
                var indent = new string(' ', entry.Metadata.depth);
                Console.WriteLine("{0}- {1}  [tot size: {2}]", indent, entry, entry.ComputeSize());
                count++;
            }
            Console.WriteLine("displayed {0} elements\n", count);
            return count;
        }

        class FileTreeReader {
            readonly IFileTreeNode _root;
            readonly Stack<IFileTreeNode> _currentPath;
            int _count = 0;

            public FileTreeReader(IFileTreeNode root) {
                _root = root;
                _currentPath = new Stack<IFileTreeNode>();
                _count = 1;
            }

            public IFileTreeNode Root => _root;
            public int IndentDepth => _currentPath.Count;
            public int Count => _count;

            public void IncludeContent(IFileTreeEntry entry) {
                _count++;
                var currentNode = GetCurrentNode();
                var metadata = currentNode.Metadata;
                metadata.depth++;
                metadata.parentIndex = currentNode.Children.Count;
                entry.Metadata = metadata;
                currentNode.AddChild(entry);
                Console.WriteLine("added {0} to {1}", entry, currentNode);
            }

            public bool MoveDown(string targetNodeName) {
                var current = GetCurrentNode();
                var targetNode = current.Children
                    .OfType<IFileTreeNode>()
                    .FirstOrDefault(n => n.Name == targetNodeName);
                if (targetNode != null) {
                    Console.WriteLine("moved from {0} down to {1}", current, targetNode);
                    _currentPath.Push(targetNode);
                }
                return targetNode != null;
            }

            public bool MoveUp() {
                bool result = _currentPath.TryPop(out var from);
                if (result)
                    Console.WriteLine("moved from {0} up to {1}", from, GetCurrentNode());
                else
                    Console.WriteLine("failed to move up from root!");
                return result;
            }

            public IEnumerable<IFileTreeEntry> EnumerateEntries() {
                var enumerator = new TreeEnumerator(_root);
                while (enumerator.MoveNext() && enumerator.Current != null) {
                    yield return enumerator.Current;
                }
            }

            public static IEnumerable<IFileTreeEntry> EnumerateNodes(IFileTreeNode root) {
                var enumerator = new TreeEnumerator(root);
                while (enumerator.MoveNext() && enumerator.Current != null) {
                    yield return enumerator.Current;
                }
            }

            private IFileTreeNode GetCurrentNode() {
                return _currentPath.TryPeek(out var result)
                    ? result : _root;
            }

            public struct TreeEnumerator {
                private readonly IFileTreeNode _rootNode;
                private readonly Stack<IFileTreeNode> _path;
                private bool _ended;

                public TreeEnumerator(IFileTreeNode rootNode) : this() {
                    _rootNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
                    _path = new Stack<IFileTreeNode>();
                }

                public IFileTreeEntry? Current { get; private set; }
                private IFileTreeNode GetCurrentNode() {
                    return _path.TryPeek(out var node) ? node : _rootNode;
                }

                public bool MoveNext() {
                    IFileTreeNode currentNode = GetCurrentNode();
                    if (_ended) return false;
                    if (Current == null) {
                        // not started
                        Current = _rootNode;
                    } else if (Current is IFileTreeNode asNode && asNode.Children.Count > 0) {
                        // current element is node, move down
                        if (asNode != _rootNode) _path.Push(asNode);
                        Current = asNode.Children[0];
                    } else if (!currentNode.IsLastChild(Current)) {
                        // continue visiting a node children
                        Current = currentNode.Children[currentNode.IndexOf(Current) + 1];
                    } else {
                        // visited all node elements
                        // move up til meet a node with unvisited children
                        IFileTreeNode? prevNode = null;
                        bool canceled = false;
                        while (!canceled && _path.TryPop(out prevNode)) {
                            currentNode = GetCurrentNode();
                            if (prevNode.Metadata.parentIndex + 1 < currentNode.Children.Count) {
                                canceled = true;
                            }
                        }

                        // in case of root
                        prevNode ??= currentNode;

                        if (prevNode != null && prevNode != currentNode) {
                            // found an unvisited node
                            Current = currentNode.Children[prevNode.Metadata.parentIndex + 1];
                        } else {
                            // we're visit all root children
                            _ended = true;
                            Current = null;
                        }
                    }
                    return Current != null;
                }
            }
        }

        struct TreeEntryMetadata {
            public int depth;
            public int parentIndex;
        }

        interface IFileTreeEntry {
            TreeEntryMetadata Metadata { get; set; }
            string Name { get; set; }
            long ComputeSize();
        }

        interface IFileTreeNode : IFileTreeEntry {
            IReadOnlyList<IFileTreeEntry> Children { get; }

            void AddChild(IFileTreeEntry child);

            bool HasNodesInside() {
                return Children.OfType<IFileTreeNode>().Any();
            }

            int CountAllChildEntries() {
                var result = Children.Count;
                result += Children.OfType<IFileTreeNode>()
                    .Sum(x => x.CountAllChildEntries());
                return result;
            }

            int IndexOf(IFileTreeEntry child);
            bool IsLastChild(IFileTreeEntry child);
        }

        class DirectoryNode : IFileTreeNode {
            private List<IFileTreeEntry> _children;
            private bool _changed = true;
            private long _cachedSize;

            public DirectoryNode(string name) {
                Name = name;
                _children = new List<IFileTreeEntry>();
            }

            public DirectoryNode(string name, IEnumerable<IFileTreeEntry> children) {
                Name = name;
                _children = new List<IFileTreeEntry>(children);
            }

            public string Name { get; set; }
            public IReadOnlyList<IFileTreeEntry> Children { get => _children; }
            public TreeEntryMetadata Metadata { get; set; }

            public void AddChild(IFileTreeEntry child) {
                _children.Add(child);
                _changed = true;
            }

            public int IndexOf(IFileTreeEntry child) {
                return _children.IndexOf(child);
            }

            public bool IsLastChild(IFileTreeEntry child) {
                return _children[^1] == child;
            }

            public long ComputeSize() {
                ActualizeCache();
                return _cachedSize;
            }

            public override string ToString() {
                return $"{Name} (dir)";
            }

            private void ActualizeCache() {
                if (_changed) {
                    _changed = false;
                    _cachedSize = Children.Sum(x => x.ComputeSize());
                }
            }
        }

        class FileNode : IFileTreeEntry {
            public FileNode(string name, long size) {
                Name = name;
                Size = size;
            }

            public string Name { get; set; }
            public long Size { get; set; }
            public TreeEntryMetadata Metadata { get; set; }

            public long ComputeSize() { return Size; }

            public override string ToString() {
                return $"{Name} (file, size={Size})";
            }
        }
    }
}
