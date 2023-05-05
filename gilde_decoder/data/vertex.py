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
