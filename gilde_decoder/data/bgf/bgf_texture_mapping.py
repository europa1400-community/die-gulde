"""Module containing the BgfTextureMapping class."""


from dataclasses import dataclass
from typing import BinaryIO

from gilde_decoder.data.texture_coordinate import TextureCoordinate
from gilde_decoder.data.vertex import Vertex


@dataclass
class BgfTextureMapping:
    """Class representing a texture mapping in a bgf file."""

    a: TextureCoordinate
    b: TextureCoordinate
    c: TextureCoordinate

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfTextureMapping":
        """Reads a texture mapping from a bgf file."""

        bgf_texture_mapping = cls.__new__(cls)

        vertex_u = Vertex.from_file(file)
        vertex_v = Vertex.from_file(file)
        vertex_w = Vertex.from_file(file)

        bgf_texture_mapping.a = TextureCoordinate(vertex_u.x, vertex_v.x, vertex_w.x)
        bgf_texture_mapping.b = TextureCoordinate(vertex_u.y, vertex_v.y, vertex_w.y)
        bgf_texture_mapping.c = TextureCoordinate(vertex_u.z, vertex_v.z, vertex_w.z)

        return bgf_texture_mapping
