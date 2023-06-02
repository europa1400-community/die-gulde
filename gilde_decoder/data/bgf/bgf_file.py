"""Module containing the Bgf class."""

from dataclasses import dataclass
from pathlib import Path

import numpy as np

from gilde_decoder.animations_decoder import BafFile
from gilde_decoder.data.bgf.bgf_footer import BgfFooter
from gilde_decoder.data.bgf.bgf_game_object import BgfGameObject
from gilde_decoder.data.bgf.bgf_header import BgfHeader
from gilde_decoder.data.bgf.bgf_mapping_object import BgfMappingObject
from gilde_decoder.data.bgf.bgf_texture import BgfTexture
from gilde_decoder.data.gltf.gltf_mesh import GltfMesh
from gilde_decoder.data.gltf.gltf_primitive import GltfPrimitive
from gilde_decoder.logger import logger


@dataclass
class BgfFile:
    """Class representing a bgf file."""

    path: Path
    bgf_header: BgfHeader
    bgf_textures: list[BgfTexture]
    bgf_game_objects: list[BgfGameObject]
    bgf_mapping_object: BgfMappingObject
    bgf_footer: BgfFooter

    def get_gltf_mesh(self, baf_file: BafFile | None = None) -> GltfMesh:
        gltf_primitives = []
        gltf_mesh = GltfMesh(
            name=self.path.stem,
            primitives=gltf_primitives,
        )

        texture_indices = set(
            polygon_mapping.texture_index
            for polygon_mapping in self.bgf_mapping_object.polygon_mappings
        )

        bgf_vertices = np.array(
            [
                [
                    vertex_mapping.vertex1.x,
                    vertex_mapping.vertex1.y,
                    vertex_mapping.vertex1.z,
                ]
                for vertex_mapping in self.bgf_mapping_object.vertex_mappings
            ],
            dtype=np.float32,
        )

        bgf_normals = np.array(
            [
                [
                    vertex_mapping.vertex2.x,
                    vertex_mapping.vertex2.y,
                    vertex_mapping.vertex2.z,
                ]
                for vertex_mapping in self.bgf_mapping_object.vertex_mappings
            ],
            dtype=np.float32,
        )

        bgf_vertices_per_key = None
        if baf_file is not None:
            bgf_vertices_per_key = baf_file.get_vertices_per_key()

        for texture_index in texture_indices:
            # skip indices of missing textures
            if texture_index >= len(self.bgf_footer.texture_names):
                continue

            indices = []
            vertices = []
            normals = []
            uvs = []

            vertices_per_key = None
            if bgf_vertices_per_key is not None:
                # create a new empty array of the same shape as bgf_vertices_per_key,
                # but the axis=1 will be variable and be appended to
                vertices_per_key = [[] for _ in range(bgf_vertices_per_key.shape[0])]

            polygon_mappings = [
                polygon_mapping
                for polygon_mapping in self.bgf_mapping_object.polygon_mappings
                if polygon_mapping.texture_index == texture_index
            ]
            indices_per_polygon = np.array(
                [
                    [
                        polygon_mapping.face.a,
                        polygon_mapping.face.b,
                        polygon_mapping.face.c,
                    ]
                    for polygon_mapping in polygon_mappings
                ]
            )
            uvs_per_polygon = np.array(
                [
                    [
                        [
                            polygon_mapping.texture_mapping.a.u,
                            polygon_mapping.texture_mapping.a.v,
                        ],
                        [
                            polygon_mapping.texture_mapping.b.u,
                            polygon_mapping.texture_mapping.b.v,
                        ],
                        [
                            polygon_mapping.texture_mapping.c.u,
                            polygon_mapping.texture_mapping.c.v,
                        ],
                    ]
                    for polygon_mapping in polygon_mappings
                ]
            )

            vertex_dict = {}

            for face, uvs_per_face in zip(indices_per_polygon, uvs_per_polygon):
                for vertex_index, uv in zip(face, uvs_per_face):
                    key = (vertex_index, uv[0], uv[1])
                    if key not in vertex_dict:
                        vertex_dict[key] = len(vertices)
                        vertex = bgf_vertices[vertex_index]
                        vertices.append(vertex)
                        normals.append(bgf_normals[vertex_index])
                        uvs.append(uv)

                        if vertices_per_key is not None:
                            for keyframe in range(bgf_vertices_per_key.shape[0]):
                                vertex_per_key = bgf_vertices_per_key[keyframe][
                                    vertex_index
                                ]
                                vertices_per_key[keyframe].append(vertex_per_key)

                    index = vertex_dict[key]
                    indices.append(index)

            indices = np.array(indices, dtype=np.uint32)
            vertices = np.array(vertices, dtype=np.float32)
            normals = np.array(normals, dtype=np.float32)
            uvs = np.array(uvs, dtype=np.float32)

            if vertices_per_key is not None:
                vertices_per_key = np.array(vertices_per_key, dtype=np.float32)

            gltf_primitive = GltfPrimitive(
                indices=indices,
                vertices=vertices,
                vertices_per_key=vertices_per_key,
                normals=normals,
                uvs=uvs,
                texture_index=texture_index,
            )
            gltf_primitives.append(gltf_primitive)

        return gltf_mesh

    @classmethod
    def from_file(cls, bgf_path: Path) -> "BgfFile":
        """Reads the bgf file."""

        logger.info(f"Decoding {bgf_path}")

        bgf = cls.__new__(cls)
        bgf.path = bgf_path

        if not bgf.path.exists():
            raise FileNotFoundError(f"{bgf.path} does not exist")

        with open(bgf.path, "rb") as file:
            bgf.bgf_header = BgfHeader.from_file(file)

            bgf.bgf_textures = []
            while BgfTexture.is_texture(file):
                bgf_texture = BgfTexture.from_file(file)
                bgf.bgf_textures.append(bgf_texture)

            bgf.bgf_game_objects = []
            while BgfGameObject.is_game_object(file):
                bgf_game_object = BgfGameObject.from_file(file)
                bgf.bgf_game_objects.append(bgf_game_object)

            bgf.bgf_mapping_object = BgfMappingObject.from_file(file)

            bgf.bgf_footer = BgfFooter.from_file(
                file, bgf_path, bgf.bgf_textures, bgf.bgf_game_objects
            )

        return bgf
