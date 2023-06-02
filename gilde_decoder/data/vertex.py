"""Module containing the Vertex class."""

import struct
from dataclasses import dataclass
from typing import BinaryIO


@dataclass
class Vertex:
    """Class representing a vertex in a bgf file."""

    x: float
    y: float
    z: float

    @classmethod
    def from_file(cls, file: BinaryIO) -> "Vertex":
        """Reads a vertex from a bgf file."""

        vertex = cls.__new__(cls)

        vertex.x = struct.unpack("<f", file.read(4))[0]
        vertex.y = struct.unpack("<f", file.read(4))[0]
        vertex.z = struct.unpack("<f", file.read(4))[0]

        return vertex

    def __repr__(self) -> str:
        return f"Vertex({self.x}, {self.y}, {self.z})"

    def __str__(self) -> str:
        return f"Vertex({self.x}, {self.y}, {self.z})"

    def __eq__(self, other: object) -> bool:
        if not isinstance(other, Vertex):
            return NotImplemented
        return self.x == other.x and self.y == other.y and self.z == other.z

    def __ne__(self, other: object) -> bool:
        if not isinstance(other, Vertex):
            return NotImplemented
        return self.x != other.x or self.y != other.y or self.z != other.z
