"""Module containing the BgfGameObject class."""

import os
from dataclasses import dataclass
from typing import BinaryIO, Optional

import numpy as np

from gilde_decoder.data.bgf.bgf_anim_data import BgfAnimData
from gilde_decoder.data.bgf.bgf_model import BgfModel
from gilde_decoder.data.gltf.gltf_mesh import GltfMesh
from gilde_decoder.data.gltf.gltf_primitive import GltfPrimitive
from gilde_decoder.helpers import is_value, read_string, skip_optional, skip_zero


@dataclass
class BgfGameObject:
    """Class representing a game object in a bgf file."""

    name: str
    anim_count: int
    bgf_anim_datas: list[BgfAnimData]
    bgf_model: Optional[BgfModel] = None

    def get_gltf_mesh(
        self,
    ) -> GltfMesh | None:
        """Returns the model data in gLTF compatible format
        or None if there is no model."""

        if self.bgf_model is None:
            return None

        texture_indices = [polygon.texture_index for polygon in self.bgf_model.polygons]
        unique_texture_indices = set(texture_indices)

        bgf_vertices = np.array(
            [
                [
                    vertex.x,
                    vertex.z,
                    -vertex.y,
                ]
                for vertex in self.bgf_model.vertices
            ],
            dtype=np.float32,
        )
        bgf_polygon_normals = np.array(
            [
                [
                    polygon.normal.x,
                    polygon.normal.z,
                    -polygon.normal.y,
                ]
                for polygon in self.bgf_model.polygons
            ],
            dtype=np.float32,
        )
        bgf_uv_coordinates = np.array(
            [
                [
                    [
                        polygon.texture_mapping.a.u,
                        polygon.texture_mapping.a.v,
                    ],
                    [
                        polygon.texture_mapping.b.u,
                        polygon.texture_mapping.b.v,
                    ],
                    [
                        polygon.texture_mapping.c.u,
                        polygon.texture_mapping.c.v,
                    ],
                ]
                for polygon in self.bgf_model.polygons
            ],
            dtype=np.float32,
        )

        gltf_primitives = []
        for texture_index in unique_texture_indices:
            faces = np.array(
                [
                    [
                        polygon.face.a,
                        polygon.face.b,
                        polygon.face.c,
                    ]
                    for polygon in self.bgf_model.polygons
                    if polygon.texture_index == texture_index
                ],
                dtype=np.uint32,
            )

            indices = []
            vertices = []
            normals = []
            uv_coordinates = []

            for i, face in enumerate(faces):
                for j, vertex_index in enumerate(face):
                    vertex = bgf_vertices[vertex_index]
                    vertices.append(vertex)

                    vertex_normal = bgf_polygon_normals[i]
                    normals.append(vertex_normal)

                    uv_coordinate = bgf_uv_coordinates[i][j]
                    uv_coordinates.append(uv_coordinate)

                    indices.append(i * 3 + j)

            indices = np.array(indices, dtype=np.uint32)
            vertices = np.array(vertices, dtype=np.float32)
            normals = np.array(normals, dtype=np.float32)
            uv_coordinates = np.array(uv_coordinates, dtype=np.float32)

            gltf_primitive = GltfPrimitive(
                indices=indices,
                vertices=vertices,
                vertex_normals=normals,
                uv_coordinates=uv_coordinates,
                texture_index=texture_index,
            )
            gltf_primitives.append(gltf_primitive)

        gltf_mesh = GltfMesh(
            name=self.name,
            primitives=gltf_primitives,
        )

        return gltf_mesh

    @classmethod
    def is_game_object(cls, file: BinaryIO) -> bool:
        """Checks if the current position in the file is a game object."""

        initial_pos = file.tell()

        is_obj = True

        if is_value(file, 1, 0x28, reset=True):
            file.seek(1, os.SEEK_CUR)

        if not is_value(file, 2, 0x1514, reset=False):
            is_obj = False

        file.seek(initial_pos, os.SEEK_SET)

        return is_obj

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfGameObject":
        """Reads a game object from a bgf file."""

        bgf_game_object = cls.__new__(cls)

        skip_optional(file, b"\x28", 1)
        skip_optional(file, b"\x14\x15", 2)

        bgf_game_object.name = read_string(file)

        skip_optional(file, b"\x16\x01", 5)
        has_model = skip_optional(file, b"\x17\x18", 6)

        if has_model:
            bgf_game_object.bgf_model = BgfModel.from_file(file)

        skip_optional(file, b"\x28", 1)
        skip_optional(file, b"\x28", 1)
        skip_optional(file, b"\x28", 1)
        has_anim_data = skip_optional(file, b"\x37", 1)

        bgf_game_object.anim_count = 0

        bgf_game_object.bgf_anim_datas = []
        if has_anim_data:
            bgf_game_object.anim_count = int.from_bytes(
                file.read(2), byteorder="little", signed=False
            )

            skip_zero(file, 2)

            for _ in range(bgf_game_object.anim_count):
                bgf_anim_data = BgfAnimData.from_file(file)
                bgf_game_object.bgf_anim_datas.append(bgf_anim_data)

        return bgf_game_object
