"""Module containing the BgfMappingObject class."""

import struct
from dataclasses import dataclass
from typing import BinaryIO

import numpy as np

from gilde_decoder.data.bgf.bgf_polygon_mapping import BgfPolygonMapping
from gilde_decoder.data.bgf.bgf_vertex_mapping import BgfVertexMapping
from gilde_decoder.helpers import skip_required


@dataclass
class BgfMappingObject:
    """Class representing a mapping object in a bgf file."""

    num1: int
    num2: int
    num3: int
    num4: int
    vertex_mapping_count: int
    polygon_mapping_count: int
    vertex_mappings: list[BgfVertexMapping]
    box_vertex_mappings: list[BgfVertexMapping]
    some_float: float
    polygon_mappings: list[BgfPolygonMapping]

    @property
    def vertices_ndarray(self) -> np.ndarray:
        """Returns a numpy array of the vertices."""

        return np.array(
            [
                [
                    vertex_mapping.vertex1.x,
                    vertex_mapping.vertex1.y,
                    vertex_mapping.vertex1.z,
                ]
                for vertex_mapping in self.vertex_mappings
            ],
            dtype=np.float32,
        )

    @property
    def vertex_normals_ndarray(self) -> np.ndarray:
        """Returns a numpy array of the vertex normals."""

        return np.array(
            [
                [
                    vertex_mapping.vertex2.x,
                    vertex_mapping.vertex2.y,
                    vertex_mapping.vertex2.z,
                ]
                for vertex_mapping in self.vertex_mappings
            ],
            dtype=np.float32,
        )

    def get_faces_ndarray(self, texture_index: int | None = None) -> np.ndarray:
        """Returns a numpy array of the indices."""

        return np.array(
            [
                [
                    polygon_mapping.face.a,
                    polygon_mapping.face.b,
                    polygon_mapping.face.c,
                ]
                for polygon_mapping in self.polygon_mappings
                if texture_index is None
                or polygon_mapping.texture_index == texture_index
            ],
            dtype=np.uint16,
        )

    def get_uv_coordinates_per_triangle_vertex_ndarray(
        self, texture_index: int | None = None
    ) -> np.ndarray:
        """Returns a numpy array of the uv coordinates."""

        return np.array(
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
                for polygon_mapping in self.polygon_mappings
                if texture_index is None
                or polygon_mapping.texture_index == texture_index
            ],
            dtype=np.float32,
        )

    @property
    def get_primitive_datas(
        self,
    ) -> list[tuple[np.ndarray, np.ndarray, np.ndarray, np.ndarray, int]]:
        """Returns a tuple of numpy arrays of the vertices,
        vertex normals, indices, uv coordinates and material index per material used."""

        texture_indices = [
            polygon_mapping.texture_index for polygon_mapping in self.polygon_mappings
        ]
        unique_texture_indices = set(texture_indices)

        primitive_datas: list[
            tuple[np.ndarray, np.ndarray, np.ndarray, np.ndarray]
        ] = []

        for texture_index in unique_texture_indices:
            vertices = self.vertices_ndarray
            vertex_normals = self.vertex_normals_ndarray
            faces = self.get_faces_ndarray(texture_index=texture_index)
            uv_coordinates_per_triangle_vertex = (
                self.get_uv_coordinates_per_triangle_vertex_ndarray(texture_index)
            )

            new_vertices = []
            new_vertex_normals = []
            new_indices = []
            new_uv_coordinates = []

            vertex_dict = {}

            for face, uv_coords in zip(faces, uv_coordinates_per_triangle_vertex):
                for vertex_index, uv_coord in zip(face, uv_coords):
                    key = (vertex_index, uv_coord[0], uv_coord[1])

                    if key not in vertex_dict:
                        vertex_dict[key] = len(new_vertices)
                        new_vertices.append(vertices[vertex_index])
                        new_vertex_normals.append(vertex_normals[vertex_index])
                        new_uv_coordinates.append(uv_coord)

                    index = vertex_dict[key]
                    new_indices.append(index)

            new_vertices = np.array(new_vertices, dtype=np.float32)
            new_vertex_normals = np.array(new_vertex_normals, dtype=np.float32)
            new_indices = np.array(new_indices, dtype=np.uint16)
            new_uv_coordinates = np.array(new_uv_coordinates, dtype=np.float32)

            primitive_datas.append(
                (
                    new_vertices,
                    new_vertex_normals,
                    new_indices,
                    new_uv_coordinates,
                    texture_index,
                )
            )

        return primitive_datas

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfMappingObject":
        """Reads a mapping object from a bgf file."""

        bgf_mapping_object = cls.__new__(cls)

        skip_required(file, b"\x2F\x2D", 2)

        bgf_mapping_object.num1 = int.from_bytes(
            file.read(1), byteorder="little", signed=False
        )

        bgf_mapping_object.num2 = int.from_bytes(
            file.read(2), byteorder="little", signed=False
        )

        _ = int.from_bytes(file.read(1), byteorder="little", signed=False)

        bgf_mapping_object.num3 = int.from_bytes(
            file.read(2), byteorder="little", signed=False
        )

        skip_required(file, b"\xB5\xFA", 2)

        bgf_mapping_object.num4 = int.from_bytes(
            file.read(4), byteorder="little", signed=False
        )

        bgf_mapping_object.vertex_mapping_count = int.from_bytes(
            file.read(4), byteorder="little", signed=False
        )

        bgf_mapping_object.polygon_mapping_count = int.from_bytes(
            file.read(4), byteorder="little", signed=False
        )

        bgf_mapping_object.vertex_mappings = []
        for _ in range(bgf_mapping_object.vertex_mapping_count):
            vertex_mapping = BgfVertexMapping.from_file(file)
            bgf_mapping_object.vertex_mappings.append(vertex_mapping)

        bgf_mapping_object.box_vertex_mappings = []
        for _ in range(8):
            box_vertex_mapping = BgfVertexMapping.from_file(file)
            bgf_mapping_object.box_vertex_mappings.append(box_vertex_mapping)

        bgf_mapping_object.some_float = struct.unpack("<f", file.read(4))[0]

        bgf_mapping_object.polygon_mappings = []
        for _ in range(bgf_mapping_object.polygon_mapping_count):
            polygon_mapping = BgfPolygonMapping.from_file(file)
            bgf_mapping_object.polygon_mappings.append(polygon_mapping)

        return bgf_mapping_object
