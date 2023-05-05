"""Module containing the BgfAnimFooter class."""

import os
from dataclasses import dataclass
from typing import BinaryIO

from gilde_decoder.helpers import read_string


@dataclass
class BgfAnimFooter:
    """Class representing a bgf anim footer."""

    name: str

    @classmethod
    def from_file(cls, file: BinaryIO) -> "BgfAnimFooter":
        """Reads the anim footer from a bgf file."""

        bgf_anim_footer = cls.__new__(cls)

        bgf_anim_footer.name = read_string(file)

        file.seek(27, os.SEEK_CUR)

        return bgf_anim_footer
