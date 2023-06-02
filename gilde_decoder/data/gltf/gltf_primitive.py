from dataclasses import dataclass

import numpy as np


@dataclass
class GltfPrimitive:
    vertices: np.ndarray
    vertices_per_key: np.ndarray | None
    normals: np.ndarray
    uvs: np.ndarray
    indices: np.ndarray
    texture_index: int
