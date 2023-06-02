"""Module containing the PolygonMapping class."""

from dataclasses import dataclass
from typing import BinaryIO

from gilde_decoder.data.bgf.bgf_texture_mapping import BgfTextureMapping
from gilde_decoder.data.face import Face


@dataclass
class BgfPolygonMapping:
    """Class representing a polygon mapping in a bgf file."""

    face: Face
    texture_mapping: BgfTextureMapping
    texture_index: int

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfPolygonMapping":
        """Reads a polygon mapping from a bgf file."""

        polygon_mapping = cls.__new__(cls)

        polygon_mapping.face = Face.from_file(file)

        polygon_mapping.texture_mapping = BgfTextureMapping.from_file(file)
        polygon_mapping.texture_index = int.from_bytes(
            file.read(1), byteorder="little", signed=False
        )

        return polygon_mapping
