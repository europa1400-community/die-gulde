from dataclasses import dataclass

import numpy as np


@dataclass
class GltfPrimitive:
    vertices: np.ndarray
    vertex_normals: np.ndarray
    uv_coordinates: np.ndarray
    indices: np.ndarray
    texture_index: int
