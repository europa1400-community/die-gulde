from dataclasses import dataclass

from gilde_decoder.data.gltf.gltf_primitive import GltfPrimitive


@dataclass
class GltfMesh:
    """Class representing a mesh in a gLTF file."""

    name: str
    primitives: list[GltfPrimitive]
