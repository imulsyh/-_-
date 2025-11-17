"""
Example usage of Linear Threshold Model for Influence Maximization.

This script demonstrates various use cases of the Linear Threshold model.
"""

from linear_threshold import (
    LinearThresholdModel,
    create_random_graph,
    create_small_world_graph
)


def example_1_basic_simulation():
    """Example 1: Basic influence simulation with a simple graph."""
    print("\n" + "="*70)
    print("Example 1: Basic Influence Simulation")
    print("="*70)
    
    # Create a simple social network graph
    # Edges represent influence relationships (source, target, weight)
    edges = [
        (0, 1, 0.6),  # Person 0 influences person 1 with strength 0.6
        (0, 2, 0.5),
        (1, 2, 0.4),
        (1, 3, 0.7),
        (2, 3, 0.5),
        (2, 4, 0.6),
        (3, 4, 0.8)
    ]
    
    model = LinearThresholdModel(edges)
    print(f"Created network with {len(model.nodes)} people")
    print(f"Number of influence relationships: {sum(len(v) for v in model.graph.values())}")
    
    # Set specific thresholds for demonstration
    thresholds = {
        0: 0.0,  # Already active (seed)
        1: 0.5,  # Medium threshold
        2: 0.4,  # Lower threshold (easier to influence)
        3: 0.6,  # Higher threshold (harder to influence)
        4: 0.7   # Very high threshold
    }
    model.set_thresholds(thresholds)
    
    # Simulate starting with person 0
    seed_set = {0}
    activated = model.simulate(seed_set, thresholds)
    
    print(f"\nStarting with seed: {seed_set}")
    print(f"Final activated people: {sorted(activated)}")
    print(f"Total influenced: {len(activated)} out of {len(model.nodes)} people")
    print(f"Influence spread: {len(activated) / len(model.nodes) * 100:.1f}%")


def example_2_monte_carlo_estimation():
    """Example 2: Estimate expected influence with random thresholds."""
    print("\n" + "="*70)
    print("Example 2: Monte Carlo Influence Estimation")
    print("="*70)
    
    # Create a larger network
    edges = []
    for i in range(10):
        for j in range(i+1, min(i+4, 10)):
            edges.append((i, j, 0.5))  # Each person influences 3 others
    
    model = LinearThresholdModel(edges)
    print(f"Created network with {len(model.nodes)} people")
    
    # Test different seed sets
    seed_sets = [
        {0},
        {5},
        {0, 5},
        {0, 5, 9}
    ]
    
    print("\nComparing different seed selections:")
    for seeds in seed_sets:
        influence = model.estimate_influence(seeds, num_simulations=500)
        seeds_str = str(sorted(seeds))
        print(f"  Seeds {seeds_str:15} → Expected influence: {influence:.2f} people")


def example_3_find_optimal_seeds():
    """Example 3: Find optimal seed set for maximum influence."""
    print("\n" + "="*70)
    print("Example 3: Finding Optimal Seeds")
    print("="*70)
    
    # Create a network with clear influencers
    edges = [
        # Hub 1: Person 0 (well-connected)
        (0, 1, 0.7), (0, 2, 0.6), (0, 3, 0.8),
        # Hub 2: Person 4 (well-connected)
        (4, 5, 0.7), (4, 6, 0.6), (4, 7, 0.8),
        # Connections between hubs
        (1, 4, 0.5), (3, 5, 0.5),
        # Additional connections
        (2, 8, 0.6), (6, 9, 0.6),
        (8, 9, 0.5)
    ]
    
    model = LinearThresholdModel(edges)
    print(f"Created network with {len(model.nodes)} people")
    
    # Find optimal seeds
    k_values = [1, 2, 3]
    
    print("\nFinding optimal influencers:")
    for k in k_values:
        print(f"\n  Finding top {k} influencer(s)...")
        seeds = model.greedy_influence_maximization(k, num_simulations=200)
        influence = model.estimate_influence(set(seeds), num_simulations=500)
        
        print(f"    Selected people: {seeds}")
        print(f"    Expected influence: {influence:.2f} out of {len(model.nodes)} people")
        print(f"    Coverage: {influence / len(model.nodes) * 100:.1f}%")


def example_4_compare_algorithms():
    """Example 4: Compare Greedy vs CELF algorithms."""
    print("\n" + "="*70)
    print("Example 4: Algorithm Comparison (Greedy vs CELF)")
    print("="*70)
    
    # Create a random network
    import time
    model = create_random_graph(30, edge_probability=0.15)
    print(f"Created random network with {len(model.nodes)} people")
    
    k = 5
    num_sims = 100
    
    # Greedy algorithm
    print(f"\nRunning Greedy algorithm (k={k})...")
    start_time = time.time()
    greedy_seeds = model.greedy_influence_maximization(k, num_simulations=num_sims)
    greedy_time = time.time() - start_time
    greedy_influence = model.estimate_influence(set(greedy_seeds), num_simulations=500)
    
    print(f"  Time: {greedy_time:.2f} seconds")
    print(f"  Seeds: {greedy_seeds}")
    print(f"  Influence: {greedy_influence:.2f}")
    
    # CELF algorithm
    print(f"\nRunning CELF algorithm (k={k})...")
    start_time = time.time()
    celf_seeds = model.celf_influence_maximization(k, num_simulations=num_sims)
    celf_time = time.time() - start_time
    celf_influence = model.estimate_influence(set(celf_seeds), num_simulations=500)
    
    print(f"  Time: {celf_time:.2f} seconds")
    print(f"  Seeds: {celf_seeds}")
    print(f"  Influence: {celf_influence:.2f}")
    
    print(f"\nSpeedup: {greedy_time / celf_time:.2f}x faster with CELF")


def example_5_small_world_network():
    """Example 5: Influence maximization on small-world network."""
    print("\n" + "="*70)
    print("Example 5: Small-World Network (Facebook/Twitter-like)")
    print("="*70)
    
    # Create a small-world network (models real social networks well)
    model = create_small_world_graph(50, k=6, p=0.1)
    print(f"Created small-world network with {len(model.nodes)} people")
    print("(Each person initially connected to 6 nearest neighbors)")
    
    # Find influential nodes
    k = 5
    print(f"\nFinding top {k} influencers in the network...")
    seeds = model.celf_influence_maximization(k, num_simulations=200)
    influence = model.estimate_influence(set(seeds), num_simulations=500)
    
    print(f"\nResults:")
    print(f"  Top {k} influencers: {seeds}")
    print(f"  Expected reach: {influence:.2f} people")
    print(f"  Network coverage: {influence / len(model.nodes) * 100:.1f}%")
    
    # Compare with random selection
    import random
    random_seeds = random.sample(list(model.nodes), k)
    random_influence = model.estimate_influence(set(random_seeds), num_simulations=500)
    
    print(f"\nComparison with random selection:")
    print(f"  Random {k} people: {random_seeds}")
    print(f"  Their reach: {random_influence:.2f} people")
    print(f"  Coverage: {random_influence / len(model.nodes) * 100:.1f}%")
    print(f"\nOptimal seeds achieve {(influence / random_influence - 1) * 100:.1f}% more reach!")


def main():
    """Run all examples."""
    print("\n" + "="*70)
    print(" LINEAR THRESHOLD MODEL - COMPREHENSIVE EXAMPLES")
    print("="*70)
    print("\nThis demonstrates influence maximization for applications like:")
    print("  • Viral marketing campaigns")
    print("  • Information dissemination")
    print("  • Disease outbreak prevention")
    print("  • Social network analysis")
    
    example_1_basic_simulation()
    example_2_monte_carlo_estimation()
    example_3_find_optimal_seeds()
    example_4_compare_algorithms()
    example_5_small_world_network()
    
    print("\n" + "="*70)
    print(" All examples completed successfully!")
    print("="*70)
    print("\nFor more information, see LINEAR_THRESHOLD_README.md")
    print()


if __name__ == "__main__":
    main()
