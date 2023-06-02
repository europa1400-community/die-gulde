"""Module containing the Face class."""

from dataclasses import dataclass
from typing import BinaryIO


@dataclass
class Face:
    """Class representing a face in a bgf file."""

    a: int
    b: int
    c: int

    @classmethod
    def from_file(cls, file: BinaryIO) -> "Face":
        """Reads a face from a bgf file."""

        face = cls.__new__(cls)

        face.a = int.from_bytes(file.read(4), byteorder="little", signed=False)
        face.b = int.from_bytes(file.read(4), byteorder="little", signed=False)
        face.c = int.from_bytes(file.read(4), byteorder="little", signed=False)

        return face
