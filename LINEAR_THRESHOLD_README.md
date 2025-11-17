# Linear Threshold Model for Influence Maximization

This implementation provides a complete solution for the influence maximization problem using the Linear Threshold (LT) model, a fundamental propagation model in social network analysis.

## Overview

The Linear Threshold model is a diffusion process that models how influence spreads through a network:

- **Nodes**: Represent individuals in a social network
- **Edges**: Represent influence relationships with associated weights
- **Thresholds**: Each node has a threshold value (0 to 1) that determines when it becomes active
- **Activation**: A node becomes active when the weighted sum of influence from active neighbors exceeds its threshold

### Mathematical Definition

For a directed graph G = (V, E) with edge weights b_{u,v}:

1. Each node v has a threshold θ_v ∼ Uniform(0, 1)
2. Edge weights satisfy: Σ_{u→v} b_{u,v} ≤ 1 for all v
3. Node v becomes active when: Σ_{u∈active neighbors} b_{u,v} ≥ θ_v

## Features

### Core Functionality

1. **Linear Threshold Simulation**
   - Single simulation with specified thresholds
   - Monte Carlo estimation with random thresholds
   - Support for multiple seed nodes

2. **Influence Maximization Algorithms**
   - **Greedy Algorithm**: Provides (1-1/e) ≈ 63% approximation guarantee
   - **CELF (Cost-Effective Lazy Forward)**: Optimized greedy with lazy evaluation

3. **Graph Generation Utilities**
   - Random graph generation
   - Small-world network generation (Watts-Strogatz model)

## Installation

No external dependencies required! The implementation uses only Python standard library.

```bash
# Clone or download the files
# No pip install needed
```

## Usage

### Basic Example

```python
from linear_threshold import LinearThresholdModel

# Create a graph with edges (source, target, weight)
edges = [
    (0, 1, 0.5),  # Node 0 influences node 1 with weight 0.5
    (0, 2, 0.4),
    (1, 2, 0.3),
    (1, 3, 0.5),
    (2, 3, 0.4)
]

model = LinearThresholdModel(edges)

# Simulate influence propagation from seed node 0
seed_set = {0}
activated_nodes = model.simulate(seed_set)
print(f"Activated nodes: {activated_nodes}")
```

### Influence Estimation

```python
# Estimate expected influence using Monte Carlo simulation
expected_influence = model.estimate_influence({0}, num_simulations=1000)
print(f"Expected influence: {expected_influence:.2f} nodes")
```

### Finding Optimal Seed Set

```python
# Find k seed nodes that maximize influence
k = 3  # Number of seeds to select
seed_nodes = model.greedy_influence_maximization(k, num_simulations=500)
print(f"Optimal seed set: {seed_nodes}")

# Estimate influence of selected seeds
influence = model.estimate_influence(set(seed_nodes), num_simulations=1000)
print(f"Expected influence: {influence:.2f} nodes")
```

### Using CELF Optimization

```python
# CELF is faster for large graphs
seed_nodes = model.celf_influence_maximization(k=5, num_simulations=500)
print(f"Seeds selected with CELF: {seed_nodes}")
```

### Creating Test Graphs

```python
from linear_threshold import create_random_graph, create_small_world_graph

# Random graph
random_model = create_random_graph(
    num_nodes=50, 
    edge_probability=0.1, 
    weighted=True
)

# Small-world network
sw_model = create_small_world_graph(
    num_nodes=100, 
    k=6,        # Each node connects to k nearest neighbors
    p=0.1       # Rewiring probability
)
```

## API Reference

### LinearThresholdModel Class

#### Methods

**`__init__(edges=None)`**
- Initialize the model
- `edges`: Optional list of (source, target, weight) tuples

**`add_edge(source, target, weight)`**
- Add a single weighted edge
- `source`: Source node ID
- `target`: Target node ID
- `weight`: Edge weight (0 to 1)

**`add_edges(edges)`**
- Add multiple edges at once
- `edges`: List of (source, target, weight) tuples

**`set_thresholds(thresholds=None)`**
- Set node activation thresholds
- `thresholds`: Dict mapping nodes to thresholds, or None for random

**`simulate(seed_set, thresholds=None)`**
- Simulate influence propagation
- `seed_set`: Set of initial active nodes
- `thresholds`: Optional threshold dict
- Returns: Set of all activated nodes

**`estimate_influence(seed_set, num_simulations=1000)`**
- Estimate expected influence via Monte Carlo
- `seed_set`: Set of seed nodes
- `num_simulations`: Number of simulations to run
- Returns: Average number of activated nodes

**`greedy_influence_maximization(k, num_simulations=1000)`**
- Find k seeds using greedy algorithm
- `k`: Number of seeds to select
- `num_simulations`: Simulations per evaluation
- Returns: List of k seed nodes

**`celf_influence_maximization(k, num_simulations=1000)`**
- Find k seeds using CELF optimization
- `k`: Number of seeds to select
- `num_simulations`: Simulations per evaluation
- Returns: List of k seed nodes

### Helper Functions

**`create_random_graph(num_nodes, edge_probability, weighted)`**
- Generate random directed graph
- Returns: LinearThresholdModel instance

**`create_small_world_graph(num_nodes, k, p)`**
- Generate Watts-Strogatz small-world network
- Returns: LinearThresholdModel instance

## Running Tests

```bash
# Run all tests
python test_linear_threshold.py

# Run with verbose output
python test_linear_threshold.py -v

# Run specific test class
python -m unittest test_linear_threshold.TestLinearThresholdModel

# Run specific test method
python -m unittest test_linear_threshold.TestLinearThresholdModel.test_simulate_single_cascade
```

## Running the Demo

```bash
python linear_threshold.py
```

This will run a demonstration showing:
1. Single simulation with fixed thresholds
2. Monte Carlo influence estimation
3. Greedy seed selection
4. Random graph influence maximization

## Algorithm Complexity

### Time Complexity

- **Single Simulation**: O(|V| + |E|) per simulation
- **Influence Estimation**: O(S × (|V| + |E|)) where S is number of simulations
- **Greedy Algorithm**: O(k × |V| × S × (|V| + |E|))
- **CELF Algorithm**: O(k × |V| × S × (|V| + |E|)) average case (better than greedy in practice)

### Space Complexity

- O(|V| + |E|) for graph storage

## Theory and Background

### Influence Maximization Problem

Given a graph G and integer k, find k seed nodes that maximize the expected number of nodes influenced through cascade propagation.

This problem is:
- **NP-hard** (proven by Kempe et al., 2003)
- **Submodular**: The influence function exhibits diminishing returns
- **Monotone**: Adding more seeds never decreases influence

### Approximation Guarantee

The greedy algorithm provides a **(1 - 1/e) ≈ 0.63** approximation guarantee due to the submodular property of the influence function.

### Key Papers

1. Kempe, D., Kleinberg, J., & Tardos, É. (2003). "Maximizing the spread of influence through a social network." KDD 2003.

2. Leskovec, J., et al. (2007). "Cost-effective outbreak detection in networks." KDD 2007. (CELF algorithm)

## Practical Applications

- **Viral Marketing**: Select influential users to promote products
- **Public Health**: Identify key individuals for vaccination campaigns
- **Information Dissemination**: Optimize news/information spreading
- **Social Networks**: Detect influential users and communities

## Example Output

```
Linear Threshold Model - Influence Maximization Demo
============================================================

Graph created with 6 nodes

1. Single Simulation Test:
   Seed set: {0}
   Activated nodes: {0, 1, 2, 3, 4}
   Total influence: 5

2. Monte Carlo Influence Estimation:
   Seed set: 0
   Expected influence: 3.45

3. Greedy Influence Maximization:
   Finding 2 seed nodes...
   Selected seeds: [0, 3]
   Expected influence: 4.82

4. Random Graph Test:
   Created random graph with 20 nodes
   Selected seeds using CELF: [7, 12, 3]
   Expected influence: 8.34 (41.7% of nodes)

============================================================
Demo completed!
```

## Limitations and Considerations

1. **Computation Time**: Monte Carlo estimation requires many simulations for accuracy
2. **Scalability**: Greedy algorithm can be slow for very large graphs (>10,000 nodes)
3. **Threshold Distribution**: Assumes uniform random thresholds; real networks may differ
4. **Edge Weights**: Requires proper normalization (incoming weights ≤ 1 per node)

## Advanced Usage

### Custom Threshold Distribution

```python
# Use custom thresholds instead of uniform random
custom_thresholds = {
    0: 0.2,  # Low threshold - easily influenced
    1: 0.8,  # High threshold - hard to influence
    2: 0.5   # Medium threshold
}

model.set_thresholds(custom_thresholds)
activated = model.simulate({0}, custom_thresholds)
```

### Analyzing Influence Spread

```python
# Track influence over multiple simulations
influences = []
for _ in range(100):
    thresholds = {node: random.random() for node in model.nodes}
    activated = model.simulate({0}, thresholds)
    influences.append(len(activated))

import statistics
print(f"Mean: {statistics.mean(influences):.2f}")
print(f"Std Dev: {statistics.stdev(influences):.2f}")
```

## Contributing

Feel free to extend this implementation with:
- Independent Cascade model
- Weighted Cascade model
- Community-based influence analysis
- Network visualization
- Performance optimizations for large-scale graphs

## License

This implementation is provided as-is for educational and research purposes.
