"""Module for the animations_decoder argument parser."""

import os
from pathlib import Path
from typing import Optional

from tap import Tap


class AnimationsArgumentsParser(Tap):
    """Argument parser for the animations_decoder."""

    game_path: Optional[Path] = None
    output_path: Path = Path(os.path.join(os.getcwd(), "output"))
    file: Optional[Path] = None

    def configure(self) -> None:
        """Configure the argument parser."""

        self.add_argument(
            "-g",
            "--game_path",
            help="Path to the game directory.",
            type=self._path_converter,
        )
        self.add_argument(
            "-o",
            "--output_path",
            help="Output path where the extracted files will be stored. "
            "If not specified, the output folder will be created "
            "in the current working directory.",
            type=self._path_converter,
        )
        self.add_argument(
            "-f",
            "--file",
            help="Path to a single baf file to decode.",
            type=self._path_converter,
        )

    @staticmethod
    def _path_converter(path_str: str) -> Path:
        path = Path(path_str)

        if not path.exists():
            raise ValueError(f"Path {path} does not exist.")

        path = path.resolve()
        return path
