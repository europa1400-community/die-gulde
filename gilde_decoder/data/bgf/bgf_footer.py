import os
from dataclasses import dataclass
from pathlib import Path
from typing import BinaryIO

from gilde_decoder.const import MODELS_REDUCED_FOOTER_FILES, MODELS_STRING_ENCODING
from gilde_decoder.data.bgf.bgf_game_object import BgfGameObject
from gilde_decoder.data.bgf.bgf_texture import BgfTexture
from gilde_decoder.helpers import find_address_of_byte_pattern, read_string
from gilde_decoder.logger import logger


@dataclass
class BgfFooter:
    texture_names: list[str]

    @staticmethod
    def validate_footer(
        file: BinaryIO,
        bgf_path: Path,
        bgf_textures: list[BgfTexture],
        bgf_game_objects: list[BgfGameObject],
    ) -> bool:
        """Validates the footer of a bgf file."""

        initial_position = file.tell()
        footer = file.read()

        literal_count = 0
        footer_texture_count = 0
        footer_anim_count = 0

        texture_bytes_found = []
        for bgf_texture in bgf_textures:
            texture_name = bgf_texture.name.split(".")[0]

            texture_name_bytes = texture_name.encode(MODELS_STRING_ENCODING)

            if bgf_texture.appendix:
                appendix = bgf_texture.appendix.split(".")[0]
                appendix_bytes = appendix.encode(MODELS_STRING_ENCODING)

                if bgf_texture.appendix_type == 1:
                    texture_name_bytes += b"\x00" + appendix_bytes
                elif bgf_texture.appendix_type == 2:
                    texture_name_bytes += b"\x00\x00" + appendix_bytes
                else:
                    raise ValueError("Unknown appendix type")

                texture_name += appendix

            if texture_name_bytes in texture_bytes_found:
                continue

            relative_positions = find_address_of_byte_pattern(
                texture_name_bytes, footer
            )

            if len(relative_positions) == 0:
                continue

            texture_bytes_found.append(texture_name_bytes)
            literal_count += len(texture_name * len(relative_positions))
            footer_texture_count += len(relative_positions)

        file.seek(initial_position + footer_texture_count * 9 + literal_count + 4)

        game_objects_with_anim_data = [
            bgf_game_object
            for bgf_game_object in bgf_game_objects
            if len(bgf_game_object.bgf_anim_datas) > 0
        ]
        anim_literal_count = 0
        footer_anim_count = 0
        for bgf_game_object in game_objects_with_anim_data:
            for _ in bgf_game_object.bgf_anim_datas:
                value = read_string(file)
                file.seek(24, os.SEEK_CUR)
                anim_literal_count += len(value)
                footer_anim_count += 1

        expected_non_literal_count = (
            footer_texture_count * 9 + footer_anim_count * 25 + 5
        )

        is_reduced_footer_length = any(
            bgf_path.name == reduced_footer_file
            for reduced_footer_file in MODELS_REDUCED_FOOTER_FILES
        )
        if is_reduced_footer_length:
            expected_non_literal_count -= 4

        non_literal_count = len(footer) - literal_count - anim_literal_count

        is_valid = non_literal_count == expected_non_literal_count

        if not is_valid:
            logger.error(
                f"Got {non_literal_count} non-literal bytes"
                + " instead of {expected_non_literal_count}"
            )

        file.seek(initial_position)

        return is_valid

    @classmethod
    def from_file(
        cls,
        file: BinaryIO,
        bgf_path: Path,
        bgf_textures: list[BgfTexture],
        bgf_game_objects: list[BgfGameObject],
    ) -> "BgfFooter":
        """Reads the footer from a bgf file."""

        bgf_footer = cls.__new__(cls)

        assert cls.validate_footer(file, bgf_path, bgf_textures, bgf_game_objects)

        footer_bytes = file.read()

        footer_texture_names_to_position = {}

        for bgf_texture in bgf_textures:
            bgf_texture_full_name = bgf_texture.name
            bgf_texture_name = bgf_texture.name.split(".")[0]
            bgf_texture_name_bytes = bgf_texture_name.encode(MODELS_STRING_ENCODING)

            if bgf_texture_full_name in footer_texture_names_to_position:
                continue

            position = footer_bytes.find(bgf_texture_name_bytes)

            if position == -1:
                continue

            footer_texture_names_to_position[bgf_texture_full_name] = position

        bgf_footer.texture_names = list(footer_texture_names_to_position.keys())
        bgf_footer.texture_names.sort(
            key=lambda texture_name: footer_texture_names_to_position[texture_name]
        )

        return bgf_footer
