"""Module containing the BgfGameObject class."""

import os
from dataclasses import dataclass
from typing import BinaryIO, Optional

from gilde_decoder.data.bgf.bgf_anim_data import BgfAnimData
from gilde_decoder.data.bgf.bgf_model import BgfModel
from gilde_decoder.helpers import is_value, read_string, skip_optional, skip_zero


@dataclass
class BgfGameObject:
    """Class representing a game object in a bgf file."""

    name: str
    anim_count: int
    bgf_anim_datas: list[BgfAnimData]
    bgf_model: Optional[BgfModel] = None

    @classmethod
    def is_game_object(cls, file: BinaryIO) -> bool:
        """Checks if the current position in the file is a game object."""

        initial_pos = file.tell()

        is_obj = True

        if is_value(file, 1, 0x28, reset=True):
            file.seek(1, os.SEEK_CUR)

        if not is_value(file, 2, 0x1514, reset=False):
            is_obj = False

        file.seek(initial_pos, os.SEEK_SET)

        return is_obj

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfGameObject":
        """Reads a game object from a bgf file."""

        bgf_game_object = cls.__new__(cls)

        skip_optional(file, b"\x28", 1)
        skip_optional(file, b"\x14\x15", 2)

        bgf_game_object.name = read_string(file)

        skip_optional(file, b"\x16\x01", 5)
        has_model = skip_optional(file, b"\x17\x18", 6)

        if has_model:
            bgf_game_object.bgf_model = BgfModel.from_file(file)

        skip_optional(file, b"\x28", 1)
        skip_optional(file, b"\x28", 1)
        skip_optional(file, b"\x28", 1)
        has_anim_data = skip_optional(file, b"\x37", 1)

        bgf_game_object.anim_count = 0

        bgf_game_object.bgf_anim_datas = []
        if has_anim_data:
            bgf_game_object.anim_count = int.from_bytes(
                file.read(2), byteorder="little", signed=False
            )

            skip_zero(file, 2)

            for _ in range(bgf_game_object.anim_count):
                bgf_anim_data = BgfAnimData.from_file(file)
                bgf_game_object.bgf_anim_datas.append(bgf_anim_data)

        return bgf_game_object
