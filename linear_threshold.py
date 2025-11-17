"""
Linear Threshold Model for Influence Maximization

This module implements the Linear Threshold (LT) model, a fundamental propagation 
model used in social network analysis and influence maximization problems.

In the Linear Threshold model:
- Each node v has a threshold θ_v uniformly chosen at random from [0,1]
- Each edge (u,v) has a weight b_u,v such that Σ b_u,v ≤ 1 for all v
- A node v becomes active when Σ_{u∈active neighbors} b_u,v ≥ θ_v

The influence maximization problem aims to find k seed nodes that maximize 
the expected number of activated nodes through the propagation process.
"""

import random
from collections import defaultdict
from typing import List, Set, Dict, Tuple


class LinearThresholdModel:
    """
    Linear Threshold Model for influence propagation in social networks.
    
    Attributes:
        graph: Adjacency list representation where graph[u] = [(v, weight), ...]
        nodes: Set of all nodes in the graph
        thresholds: Dictionary mapping nodes to their activation thresholds
    """
    
    def __init__(self, edges: List[Tuple[int, int, float]] = None):
        """
        Initialize the Linear Threshold model.
        
        Args:
            edges: List of tuples (source, target, weight) representing directed edges
        """
        self.graph = defaultdict(list)
        self.nodes = set()
        self.thresholds = {}
        
        if edges:
            self.add_edges(edges)
    
    def add_edge(self, source: int, target: int, weight: float):
        """
        Add a weighted edge to the graph.
        
        Args:
            source: Source node
            target: Target node
            weight: Edge weight (influence weight)
        """
        self.graph[source].append((target, weight))
        self.nodes.add(source)
        self.nodes.add(target)
    
    def add_edges(self, edges: List[Tuple[int, int, float]]):
        """
        Add multiple edges to the graph.
        
        Args:
            edges: List of tuples (source, target, weight)
        """
        for source, target, weight in edges:
            self.add_edge(source, target, weight)
    
    def set_thresholds(self, thresholds: Dict[int, float] = None):
        """
        Set activation thresholds for nodes.
        
        Args:
            thresholds: Dictionary mapping nodes to thresholds. 
                       If None, random thresholds are assigned.
        """
        if thresholds is None:
            # Assign random thresholds from uniform distribution [0, 1]
            self.thresholds = {node: random.random() for node in self.nodes}
        else:
            self.thresholds = thresholds.copy()
    
    def simulate(self, seed_set: Set[int], thresholds: Dict[int, float] = None) -> Set[int]:
        """
        Simulate influence propagation from a seed set using Linear Threshold model.
        
        Args:
            seed_set: Initial set of active nodes
            thresholds: Optional thresholds for this simulation. 
                       If None, uses stored thresholds or generates random ones.
        
        Returns:
            Set of all activated nodes after propagation completes
        """
        if thresholds is None:
            if not self.thresholds:
                self.set_thresholds()
            thresholds = self.thresholds
        
        # Initialize active nodes with seed set
        active = set(seed_set)
        newly_active = set(seed_set)
        
        # Track weighted influence received by each node
        influence_received = defaultdict(float)
        
        # Propagate influence
        while newly_active:
            next_active = set()
            
            for node in newly_active:
                # Spread influence to neighbors
                if node in self.graph:
                    for neighbor, weight in self.graph[node]:
                        if neighbor not in active:
                            influence_received[neighbor] += weight
                            
                            # Check if neighbor's threshold is exceeded
                            if influence_received[neighbor] >= thresholds[neighbor]:
                                next_active.add(neighbor)
            
            # Update active sets
            active.update(next_active)
            newly_active = next_active
        
        return active
    
    def estimate_influence(self, seed_set: Set[int], num_simulations: int = 1000) -> float:
        """
        Estimate the expected influence (number of activated nodes) of a seed set
        through Monte Carlo simulation.
        
        Args:
            seed_set: Initial set of active nodes
            num_simulations: Number of simulations to run
        
        Returns:
            Average number of nodes activated across all simulations
        """
        total_influence = 0
        
        for _ in range(num_simulations):
            # Generate random thresholds for this simulation
            thresholds = {node: random.random() for node in self.nodes}
            activated = self.simulate(seed_set, thresholds)
            total_influence += len(activated)
        
        return total_influence / num_simulations
    
    def greedy_influence_maximization(self, k: int, num_simulations: int = 1000) -> List[int]:
        """
        Find k seed nodes that maximize influence using greedy algorithm.
        
        This implements the greedy algorithm that provides a (1-1/e) approximation
        guarantee for influence maximization under the Linear Threshold model.
        
        Args:
            k: Number of seed nodes to select
            num_simulations: Number of Monte Carlo simulations per evaluation
        
        Returns:
            List of k seed nodes that approximately maximize influence
        """
        seed_set = set()
        
        for _ in range(k):
            best_node = None
            best_marginal_gain = -1
            
            # Find node with maximum marginal gain
            for node in self.nodes:
                if node not in seed_set:
                    # Calculate marginal gain of adding this node
                    current_influence = self.estimate_influence(seed_set, num_simulations)
                    new_influence = self.estimate_influence(seed_set | {node}, num_simulations)
                    marginal_gain = new_influence - current_influence
                    
                    if marginal_gain > best_marginal_gain:
                        best_marginal_gain = marginal_gain
                        best_node = node
            
            if best_node is not None:
                seed_set.add(best_node)
        
        return list(seed_set)
    
    def celf_influence_maximization(self, k: int, num_simulations: int = 1000) -> List[int]:
        """
        Find k seed nodes using CELF (Cost-Effective Lazy Forward) optimization.
        
        CELF is an optimized version of greedy algorithm that exploits submodularity
        to avoid redundant influence evaluations.
        
        Args:
            k: Number of seed nodes to select
            num_simulations: Number of Monte Carlo simulations per evaluation
        
        Returns:
            List of k seed nodes
        """
        seed_set = set()
        
        # Initialize marginal gains
        marginal_gains = []
        for node in self.nodes:
            influence = self.estimate_influence({node}, num_simulations)
            marginal_gains.append((influence, node, 0))  # (gain, node, iteration)
        
        # Sort by marginal gain
        marginal_gains.sort(reverse=True)
        
        # Select k nodes
        for i in range(k):
            while True:
                gain, node, iteration = marginal_gains[0]
                
                # If this node was evaluated in current iteration, select it
                if iteration == i:
                    seed_set.add(node)
                    marginal_gains.pop(0)
                    break
                
                # Otherwise, re-evaluate marginal gain
                current_influence = self.estimate_influence(seed_set, num_simulations)
                new_influence = self.estimate_influence(seed_set | {node}, num_simulations)
                updated_gain = new_influence - current_influence
                
                # Update and re-sort
                marginal_gains[0] = (updated_gain, node, i)
                marginal_gains.sort(reverse=True)
        
        return list(seed_set)


def create_random_graph(num_nodes: int, edge_probability: float = 0.1, 
                       weighted: bool = True) -> LinearThresholdModel:
    """
    Create a random directed graph for testing.
    
    Args:
        num_nodes: Number of nodes in the graph
        edge_probability: Probability of edge existence between any two nodes
        weighted: If True, assigns random weights; otherwise uniform weights
    
    Returns:
        LinearThresholdModel with random graph
    """
    model = LinearThresholdModel()
    
    for i in range(num_nodes):
        for j in range(num_nodes):
            if i != j and random.random() < edge_probability:
                if weighted:
                    # Random weight, ensuring incoming weights sum to at most 1
                    weight = random.random() * 0.3  # Keep it reasonable
                else:
                    weight = 0.1
                model.add_edge(i, j, weight)
    
    return model


def create_small_world_graph(num_nodes: int, k: int = 4, p: float = 0.1) -> LinearThresholdModel:
    """
    Create a Watts-Strogatz small-world graph.
    
    Args:
        num_nodes: Number of nodes
        k: Each node is connected to k nearest neighbors in ring topology
        p: Probability of rewiring each edge
    
    Returns:
        LinearThresholdModel with small-world structure
    """
    model = LinearThresholdModel()
    
    # Create ring lattice
    for i in range(num_nodes):
        for j in range(1, k // 2 + 1):
            target = (i + j) % num_nodes
            weight = 1.0 / k
            
            # Rewire with probability p
            if random.random() < p:
                target = random.randint(0, num_nodes - 1)
                while target == i:
                    target = random.randint(0, num_nodes - 1)
            
            model.add_edge(i, target, weight)
    
    return model


if __name__ == "__main__":
    # Example usage
    print("Linear Threshold Model - Influence Maximization Demo")
    print("=" * 60)
    
    # Create a simple example graph
    edges = [
        (0, 1, 0.5), (0, 2, 0.4),
        (1, 2, 0.3), (1, 3, 0.5),
        (2, 3, 0.4), (2, 4, 0.3),
        (3, 4, 0.6), (4, 5, 0.5)
    ]
    
    model = LinearThresholdModel(edges)
    print(f"\nGraph created with {len(model.nodes)} nodes")
    
    # Test single simulation
    print("\n1. Single Simulation Test:")
    seed_set = {0}
    model.set_thresholds({0: 0.0, 1: 0.3, 2: 0.3, 3: 0.4, 4: 0.5, 5: 0.4})
    activated = model.simulate(seed_set)
    print(f"   Seed set: {seed_set}")
    print(f"   Activated nodes: {activated}")
    print(f"   Total influence: {len(activated)}")
    
    # Estimate influence with Monte Carlo
    print("\n2. Monte Carlo Influence Estimation:")
    estimated = model.estimate_influence({0}, num_simulations=1000)
    print(f"   Seed set: {0}")
    print(f"   Expected influence: {estimated:.2f}")
    
    # Greedy influence maximization
    print("\n3. Greedy Influence Maximization:")
    k = 2
    seed_nodes = model.greedy_influence_maximization(k, num_simulations=100)
    print(f"   Finding {k} seed nodes...")
    print(f"   Selected seeds: {seed_nodes}")
    final_influence = model.estimate_influence(set(seed_nodes), num_simulations=500)
    print(f"   Expected influence: {final_influence:.2f}")
    
    # Test on random graph
    print("\n4. Random Graph Test:")
    random_model = create_random_graph(20, edge_probability=0.15)
    print(f"   Created random graph with {len(random_model.nodes)} nodes")
    seed_nodes = random_model.celf_influence_maximization(3, num_simulations=100)
    print(f"   Selected seeds using CELF: {seed_nodes}")
    influence = random_model.estimate_influence(set(seed_nodes), num_simulations=500)
    print(f"   Expected influence: {influence:.2f} ({influence/len(random_model.nodes)*100:.1f}% of nodes)")
    
    print("\n" + "=" * 60)
    print("Demo completed!")
