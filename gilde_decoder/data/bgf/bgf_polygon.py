"""Module containing the Polygon class."""

from dataclasses import dataclass
from typing import BinaryIO

from gilde_decoder.data.bgf.bgf_texture_mapping import BgfTextureMapping
from gilde_decoder.data.face import Face
from gilde_decoder.data.vertex import Vertex
from gilde_decoder.helpers import skip_optional


@dataclass
class BgfPolygon:
    """Class representing a polygon in a bgf file."""

    face: Face
    texture_mapping: BgfTextureMapping
    normal: Vertex
    texture_index: int

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfPolygon":
        """Reads a polygon from a bgf file."""

        polygon = cls.__new__(cls)

        polygon.face = Face.from_file(file)

        skip_optional(file, b"\x1E", 1)

        polygon.texture_mapping = BgfTextureMapping.from_file(file)

        has_normal = skip_optional(file, b"\x1F", 1)

        if has_normal:
            polygon.normal = Vertex.from_file(file)

            has_texture = skip_optional(file, b"\x20", 1)

            if has_texture:
                polygon.texture_index = int.from_bytes(
                    file.read(1), byteorder="little", signed=False
                )

            skip_optional(file, b"\x1D", 1)

        return polygon
