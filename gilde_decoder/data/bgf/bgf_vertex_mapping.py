"""Module containing the VertexMapping class."""

from dataclasses import dataclass
from typing import BinaryIO

from gilde_decoder.data.vertex import Vertex


@dataclass
class BgfVertexMapping:
    """Class representing a vertex mapping in a bgf file."""

    vertex1: Vertex
    vertex2: Vertex

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfVertexMapping":
        """Reads a vertex mapping from a bgf file."""

        vertex_mapping = cls.__new__(cls)

        vertex_mapping.vertex1 = Vertex.from_file(file)
        vertex_mapping.vertex2 = Vertex.from_file(file)

        return vertex_mapping
