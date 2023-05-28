"""Module containing the BgfAnimData class."""

from dataclasses import dataclass
from typing import BinaryIO

from gilde_decoder.data.vertex import Vertex
from gilde_decoder.helpers import read_string, skip_required


@dataclass
class BgfAnimData:
    """Class representing an animation data in a bgf file."""

    name: str
    vertex1: Vertex
    val: int
    vertex2: Vertex

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfAnimData":
        """Reads an animation data from a bgf file."""

        bgf_anim_data = cls.__new__(cls)

        skip_required(file, b"\x38", 1)

        bgf_anim_data.name = read_string(file)

        skip_required(file, b"\x39", 1)

        bgf_anim_data.vertex1 = Vertex.from_file(file)
        bgf_anim_data.val = int.from_bytes(
            file.read(1), byteorder="little", signed=False
        )
        bgf_anim_data.vertex2 = Vertex.from_file(file)

        return bgf_anim_data
