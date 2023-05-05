"""Module containing the BgfMappingObject class."""

import struct
from dataclasses import dataclass
from typing import BinaryIO

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
