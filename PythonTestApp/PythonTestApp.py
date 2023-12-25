# Define the network as an adjacency list
network = {
    "jqt": ["rhn", "xhk", "nvd"],
    "rsh": ["frs", "pzl", "lsr"],
    "xhk": ["hfx", "rhn", "bvb"],
    "cmg": ["qnr", "nvd", "lhk", "bvb"],
    "rhn": ["xhk", "bvb", "hfx", "jqt"],
    "bvb": ["xhk", "hfx", "rhn", "cmg"],
    "pzl": ["lsr", "hfx", "nvd", "rsh"],
    "qnr": ["nvd", "cmg", "frs"],
    "ntq": ["jqt", "hfx", "bvb", "xhk"],
    "nvd": ["lhk", "jqt", "cmg", "pzl", "qnr"],
    "lsr": ["lhk", "pzl", "rsh", "frs", "rzs"],
    "rzs": ["qnr", "cmg", "lsr", "rsh"],
    "frs": ["qnr", "lhk", "lsr", "rsh"],
    "hfx": ["xhk", "rhn", "bvb", "pzl", "ntq"],
    "lhk": ["nvd", "cmg", "lsr", "frs"]
}

# Define a function that finds the most tightly connected pair of nodes
def find_pair(network):
    # Initialize the set of active nodes and the weights of each node
    active = set(network.keys())
    weight = {node: 0 for node in network}
    # Pick an arbitrary node as the source
    source = next(iter(active))
    # Perform a modified breadth-first search from the source
    queue = [source]
    while queue:
        # Dequeue the first node in the queue
        node = queue.pop(0)
        # Update the weights of its neighbors
        for neighbor in network[node]:
            if neighbor in active:
                weight[neighbor] += 1
                queue.append(neighbor)
        # Remove the node from the active set
        active.remove(node)
    # Return the source and the node with the highest weight
    return source, max(weight, key=weight.get)

# Define a function that merges two nodes into one
def merge_nodes(network, node1, node2):
    # Create a new node that represents the merged node
    new_node = node1 + node2
    # Update the network with the new node and its neighbors
    network[new_node] = list(set(network[node1] + network[node2]) - {node1, node2})
    for neighbor in network[new_node]:
        network[neighbor] = [new_node if node in {node1, node2} else node for node in network[neighbor]]
    # Remove the old nodes from the network
    del network[node1]
    del network[node2]
    # Return the new node
    return new_node

# Define a function that finds the minimum cut of the network
def find_min_cut(network):
    # Initialize the minimum cut and its size
    min_cut = []
    min_size = float("inf")
    # Repeat until only two nodes remain in the network
    while len(network) > 2:
        # Find the most tightly connected pair of nodes 
        node1, node2 = find_pair(network)
        # Merge the pair into one node
        new_node = merge_nodes(network, node1, node2)
        # Update the minimum cut and its size if necessary
        cut = network[new_node]
        size = len(cut)
        if size < min_size:
            min_cut = cut
            min_size = size 
    # Return the minimum cut and its size
    return min_cut, min_size

# Find the minimum cut of the network
min_cut, min_size = find_min_cut(network)

# Print the result
print(f"The minimum cut is {min_cut} and its size is {min_size}.")
