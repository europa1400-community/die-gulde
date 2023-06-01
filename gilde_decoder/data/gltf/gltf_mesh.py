from dataclasses import dataclass

import numpy as np
import pygltflib

from gilde_decoder.data.bgf.bgf_model import BgfModel


@dataclass
class GltfMesh:
    mesh: pygltflib.Mesh
    buffer: pygltflib.Buffer
    vertex_buffer_view: pygltflib.BufferView
    triangle_buffer_view: pygltflib.BufferView
    vertex_accessor: pygltflib.Accessor
    triangle_accessor: pygltflib.Accessor
    vertices_bytes: bytes
    triangles_bytes: bytes

    @property
    def gltf_accessors(self) -> list[pygltflib.Accessor]:
        return [self.vertex_accessor, self.triangle_accessor]

    @property
    def gltf_buffer_views(self) -> list[pygltflib.BufferView]:
        return [self.vertex_buffer_view, self.triangle_buffer_view]

    @classmethod
    def from_bgf_model(cls, bgf_model: BgfModel) -> "GltfMesh":
        if (
            len(bgf_model.vertices)
            != len(bgf_model.faces)
            != len(bgf_model.normals)
            != len(bgf_model.texture_mappings)
        ):
            raise ValueError("Amount of model elements do not match.")

        vertices = np.array(
            [[vertex.x, vertex.z, -vertex.y] for vertex in bgf_model.vertices],
            dtype=np.float32,
        )
        triangles = np.array(
            [[face.a, face.b, face.c] for face in bgf_model.faces],
            dtype=np.uint16,
        ).flatten()

        gltf_mesh = cls.__new__(cls)

        gltf_mesh.vertices_bytes = vertices.tobytes()
        gltf_mesh.triangles_bytes = triangles.tobytes()

        gltf_mesh.mesh = pygltflib.Mesh(
            primitives=[
                pygltflib.Primitive(
                    attributes=pygltflib.Attributes(POSITION=1), indices=0
                )
            ]
        )

        gltf_mesh.buffer = pygltflib.Buffer(
            byteLength=len(gltf_mesh.triangles_bytes) + len(gltf_mesh.vertices_bytes)
        )

        gltf_mesh.vertex_buffer_view = (
            pygltflib.BufferView(
                buffer=0,
                byteLength=len(gltf_mesh.vertices_bytes),
                byteOffset=len(gltf_mesh.triangles_bytes),
                target=pygltflib.ARRAY_BUFFER,
            ),
        )

        gltf_mesh.triangle_buffer_view = (
            pygltflib.BufferView(
                buffer=0,
                byteLength=len(gltf_mesh.triangles_bytes),
                target=pygltflib.ELEMENT_ARRAY_BUFFER,
            ),
        )

        gltf_mesh.vertex_accessor = pygltflib.Accessor(
            bufferView=1,
            componentType=pygltflib.FLOAT,
            count=len(vertices),
            type=pygltflib.VEC3,
            max=vertices.max(axis=0).tolist(),
            min=vertices.min(axis=0).tolist(),
        )

        gltf_mesh.triangle_accessor = pygltflib.Accessor(
            bufferView=0,
            componentType=pygltflib.UNSIGNED_BYTE,
            count=triangles.size,
            type=pygltflib.SCALAR,
            max=[int(triangles.max())],
            min=[int(triangles.min())],
        )

        return gltf_mesh
