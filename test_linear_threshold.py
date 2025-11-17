"""
Unit tests for Linear Threshold Model implementation.
"""

import unittest
import random
from linear_threshold import (
    LinearThresholdModel, 
    create_random_graph, 
    create_small_world_graph
)


class TestLinearThresholdModel(unittest.TestCase):
    """Test cases for Linear Threshold Model."""
    
    def setUp(self):
        """Set up test fixtures."""
        random.seed(42)  # For reproducibility
    
    def test_initialization(self):
        """Test model initialization."""
        model = LinearThresholdModel()
        self.assertEqual(len(model.nodes), 0)
        self.assertEqual(len(model.graph), 0)
    
    def test_add_edge(self):
        """Test adding edges to the graph."""
        model = LinearThresholdModel()
        model.add_edge(0, 1, 0.5)
        
        self.assertIn(0, model.nodes)
        self.assertIn(1, model.nodes)
        self.assertEqual(len(model.nodes), 2)
        self.assertIn((1, 0.5), model.graph[0])
    
    def test_add_edges_bulk(self):
        """Test adding multiple edges at once."""
        edges = [
            (0, 1, 0.5),
            (1, 2, 0.3),
            (0, 2, 0.4)
        ]
        model = LinearThresholdModel(edges)
        
        self.assertEqual(len(model.nodes), 3)
        self.assertEqual(len(model.graph[0]), 2)
        self.assertEqual(len(model.graph[1]), 1)
    
    def test_set_thresholds_manual(self):
        """Test manually setting thresholds."""
        model = LinearThresholdModel([(0, 1, 0.5)])
        thresholds = {0: 0.3, 1: 0.7}
        model.set_thresholds(thresholds)
        
        self.assertEqual(model.thresholds[0], 0.3)
        self.assertEqual(model.thresholds[1], 0.7)
    
    def test_set_thresholds_random(self):
        """Test random threshold assignment."""
        model = LinearThresholdModel([(0, 1, 0.5), (1, 2, 0.3)])
        model.set_thresholds()
        
        self.assertEqual(len(model.thresholds), 3)
        for threshold in model.thresholds.values():
            self.assertGreaterEqual(threshold, 0)
            self.assertLessEqual(threshold, 1)
    
    def test_simulate_single_cascade(self):
        """Test influence propagation simulation."""
        # Create a simple chain: 0 -> 1 -> 2
        edges = [(0, 1, 0.8), (1, 2, 0.8)]
        model = LinearThresholdModel(edges)
        
        # Low thresholds - should activate all
        thresholds = {0: 0.0, 1: 0.5, 2: 0.5}
        activated = model.simulate({0}, thresholds)
        
        self.assertIn(0, activated)
        self.assertIn(1, activated)
        self.assertIn(2, activated)
        self.assertEqual(len(activated), 3)
    
    def test_simulate_no_cascade(self):
        """Test when influence doesn't propagate."""
        edges = [(0, 1, 0.3), (1, 2, 0.3)]
        model = LinearThresholdModel(edges)
        
        # High thresholds - only seed should be active
        thresholds = {0: 0.0, 1: 0.9, 2: 0.9}
        activated = model.simulate({0}, thresholds)
        
        self.assertEqual(activated, {0})
    
    def test_simulate_partial_cascade(self):
        """Test partial influence propagation."""
        edges = [(0, 1, 0.6), (0, 2, 0.3), (1, 2, 0.5)]
        model = LinearThresholdModel(edges)
        
        # Node 1 gets activated (0.6 > 0.5), then node 2 gets activated (0.3 + 0.5 = 0.8 > 0.7)
        thresholds = {0: 0.0, 1: 0.5, 2: 0.7}
        activated = model.simulate({0}, thresholds)
        
        self.assertIn(0, activated)
        self.assertIn(1, activated)
        # Node 2 also gets activated due to combined influence from 0 and 1
        self.assertEqual(len(activated), 3)
    
    def test_simulate_multiple_seeds(self):
        """Test simulation with multiple seed nodes."""
        edges = [(0, 2, 0.4), (1, 2, 0.4)]
        model = LinearThresholdModel(edges)
        
        # Node 2 gets activated when both seeds are active (0.4 + 0.4 = 0.8 > 0.7)
        thresholds = {0: 0.0, 1: 0.0, 2: 0.7}
        activated = model.simulate({0, 1}, thresholds)
        
        self.assertEqual(len(activated), 3)
    
    def test_estimate_influence(self):
        """Test influence estimation with Monte Carlo."""
        edges = [(0, 1, 0.8), (1, 2, 0.8)]
        model = LinearThresholdModel(edges)
        
        # With high edge weights, should activate most nodes
        influence = model.estimate_influence({0}, num_simulations=100)
        
        self.assertGreater(influence, 1.0)  # Should activate more than just seed
        self.assertLessEqual(influence, 3.0)  # Can't activate more than exist
    
    def test_greedy_influence_maximization_simple(self):
        """Test greedy algorithm on simple graph."""
        # Star graph: node 0 connects to all others
        edges = [(0, i, 0.6) for i in range(1, 5)]
        model = LinearThresholdModel(edges)
        
        seeds = model.greedy_influence_maximization(k=1, num_simulations=50)
        
        self.assertEqual(len(seeds), 1)
        # Node 0 should be selected as it has most influence
        self.assertEqual(seeds[0], 0)
    
    def test_greedy_influence_maximization_k_nodes(self):
        """Test selecting multiple seed nodes."""
        edges = [
            (0, 1, 0.5), (0, 2, 0.5),
            (3, 4, 0.5), (3, 5, 0.5)
        ]
        model = LinearThresholdModel(edges)
        
        seeds = model.greedy_influence_maximization(k=2, num_simulations=50)
        
        self.assertEqual(len(seeds), 2)
    
    def test_celf_influence_maximization(self):
        """Test CELF optimization algorithm."""
        edges = [
            (0, 1, 0.5), (0, 2, 0.5),
            (1, 2, 0.4), (2, 3, 0.5)
        ]
        model = LinearThresholdModel(edges)
        
        seeds = model.celf_influence_maximization(k=2, num_simulations=50)
        
        self.assertEqual(len(seeds), 2)
        # Both algorithms should select reasonable seeds
        influence = model.estimate_influence(set(seeds), num_simulations=100)
        self.assertGreater(influence, 2.0)
    
    def test_empty_graph_simulation(self):
        """Test simulation on graph with isolated nodes."""
        model = LinearThresholdModel()
        model.nodes = {0, 1, 2}
        
        activated = model.simulate({0})
        self.assertEqual(activated, {0})
    
    def test_influence_submodularity(self):
        """Test that influence function is submodular (diminishing returns)."""
        edges = [
            (0, 2, 0.5), (1, 2, 0.5),
            (2, 3, 0.8), (3, 4, 0.8)
        ]
        model = LinearThresholdModel(edges)
        
        # Marginal gain of adding node 1 to empty set
        empty_influence = model.estimate_influence(set(), num_simulations=100)
        with_1_influence = model.estimate_influence({1}, num_simulations=100)
        marginal_1 = with_1_influence - empty_influence
        
        # Marginal gain of adding node 1 to set containing node 0
        with_0_influence = model.estimate_influence({0}, num_simulations=100)
        with_0_1_influence = model.estimate_influence({0, 1}, num_simulations=100)
        marginal_1_given_0 = with_0_1_influence - with_0_influence
        
        # Due to submodularity, marginal gain should decrease
        # (allowing some margin for randomness)
        self.assertLessEqual(marginal_1_given_0, marginal_1 + 0.5)


class TestGraphGenerators(unittest.TestCase):
    """Test graph generation utilities."""
    
    def setUp(self):
        """Set up test fixtures."""
        random.seed(42)
    
    def test_create_random_graph(self):
        """Test random graph generation."""
        model = create_random_graph(10, edge_probability=0.2)
        
        self.assertEqual(len(model.nodes), 10)
        # Should have some edges
        total_edges = sum(len(neighbors) for neighbors in model.graph.values())
        self.assertGreater(total_edges, 0)
    
    def test_create_random_graph_weights(self):
        """Test random graph has valid edge weights."""
        model = create_random_graph(5, edge_probability=0.5, weighted=True)
        
        for source in model.graph:
            for target, weight in model.graph[source]:
                self.assertGreater(weight, 0)
                self.assertLessEqual(weight, 1)
    
    def test_create_small_world_graph(self):
        """Test small-world graph generation."""
        model = create_small_world_graph(10, k=4, p=0.1)
        
        self.assertEqual(len(model.nodes), 10)
        # Each node should have approximately k edges
        avg_degree = sum(len(neighbors) for neighbors in model.graph.values()) / len(model.nodes)
        self.assertGreaterEqual(avg_degree, 2)
    
    def test_small_world_graph_connectivity(self):
        """Test that small-world graph is reasonably connected."""
        model = create_small_world_graph(20, k=4, p=0.0)
        
        # With p=0, it's a ring lattice, so should be connected
        self.assertGreater(len(model.graph), 0)


class TestEdgeCases(unittest.TestCase):
    """Test edge cases and error conditions."""
    
    def test_single_node_graph(self):
        """Test graph with single node."""
        model = LinearThresholdModel()
        model.nodes = {0}
        model.set_thresholds()
        
        activated = model.simulate({0})
        self.assertEqual(activated, {0})
    
    def test_disconnected_graph(self):
        """Test graph with disconnected components."""
        edges = [(0, 1, 0.5), (2, 3, 0.5)]
        model = LinearThresholdModel(edges)
        
        # Activating node 0 should only affect component containing 0
        thresholds = {0: 0.0, 1: 0.4, 2: 0.0, 3: 0.4}
        activated = model.simulate({0}, thresholds)
        
        self.assertIn(0, activated)
        self.assertIn(1, activated)
        self.assertNotIn(2, activated)
    
    def test_self_loop(self):
        """Test handling of self-loops."""
        model = LinearThresholdModel()
        model.add_edge(0, 0, 0.5)  # Self-loop
        
        activated = model.simulate({0})
        # Should still work, just self-loop doesn't do anything special
        self.assertIn(0, activated)


if __name__ == '__main__':
    unittest.main()
