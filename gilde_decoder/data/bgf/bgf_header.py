"""Module containing the BgfHeader class."""

from dataclasses import dataclass
from typing import BinaryIO

from gilde_decoder.helpers import read_string, skip_optional, skip_required, skip_zero


@dataclass
class BgfHeader:
    """Class representing the header of a bgf file."""

    name: str
    mapping_address: int
    num1: int
    num2: int
    anim_count: int
    texture_count: int

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfHeader":
        """Reads the header from a bgf file."""

        bgf_header = cls.__new__(cls)

        bgf_header.name = read_string(file)

        skip_required(file, b"\x2E", 1)
        bgf_header.mapping_address = int.from_bytes(
            file.read(4), byteorder="little", signed=False
        )

        skip_required(file, b"\x01\x01", 2)
        bgf_header.num1 = int.from_bytes(file.read(1), byteorder="little", signed=False)

        skip_required(file, b"\xCD\xAB\x02", 3)
        bgf_header.num2 = int.from_bytes(file.read(1), byteorder="little", signed=False)

        has_anim = skip_optional(file, b"\x37", 1)
        if has_anim:
            bgf_header.anim_count = int.from_bytes(
                file.read(2), byteorder="little", signed=False
            )
            skip_zero(file, 2)

        skip_required(file, b"\x03\x04", 2)
        bgf_header.texture_count = int.from_bytes(
            file.read(2), byteorder="little", signed=False
        )
        skip_zero(file, 2)

        return bgf_header
