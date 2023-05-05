"""Module containing the BgfTexture class."""


import os
from dataclasses import dataclass
from typing import BinaryIO

from gilde_decoder.helpers import (
    is_value,
    read_string,
    skip_optional,
    skip_required,
    skip_until,
    skip_zero,
)


@dataclass
class BgfTexture:
    """Class representing a texture in a bgf file."""

    id: int
    name: str
    appendix: str = ""
    appendix_type: int = 0

    @property
    def material_name(self) -> str:
        """Returns the material name of the texture."""

        return self.name.split(".")[0]

    @classmethod
    def is_texture(cls, file: BinaryIO) -> bool:
        """Checks if the current position in the file is a texture."""

        initial_pos = file.tell()

        is_texture = True

        if not is_value(file, 2, 0x0605, reset=False):
            is_texture = False

        file.seek(initial_pos, os.SEEK_SET)

        return is_texture

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfTexture":
        """Reads a texture from a bgf file."""

        bgf_texture = cls.__new__(cls)

        skip_required(file, b"\x05\x06", 2)
        bgf_texture.id = int.from_bytes(file.read(2), byteorder="little", signed=False)
        skip_zero(file, 2)

        skip_optional(file, b"\x07", 1)
        skip_optional(file, b"\x08", 1)
        bgf_texture.name = read_string(file)

        has_appendix_1 = skip_optional(file, b"\x08", 1)
        has_appendix_2 = skip_optional(file, b"\x09", 1)

        if has_appendix_1 or has_appendix_2:
            bgf_texture.appendix = read_string(file)
            bgf_texture.appendix_type = 1 if has_appendix_1 else 2

        skip_until(file, 0x28, 1)

        return bgf_texture
