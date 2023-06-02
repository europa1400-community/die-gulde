"""Module for decoding and converting the models of the game."""

import tkinter as tk
from pathlib import Path
from tkinter import filedialog

from gilde_decoder.animations_decoder import BafFile
from gilde_decoder.const import (
    BGF_DIR,
    BGF_EXCLUDE,
    BGF_EXTENSION,
    GLTF_DIR,
    OBJ_DIR,
    OBJECTS_BIN,
    RESOURCES_DIR,
    TEX_DIR,
    TEXTURES_BIN,
)
from gilde_decoder.data.bgf.bgf_file import BgfFile
from gilde_decoder.data.gltf.gltf_file import GltfFile
from gilde_decoder.data.models_argument_parser import ModelsArgumentParser
from gilde_decoder.data.wavefront.wavefront_object import WavefrontObject
from gilde_decoder.helpers import extract_zipfile, get_files, rebase_path
from gilde_decoder.logger import logger


class ModelsDecoder:
    """Class for decoding and converting the models of the game."""

    game_path: Path
    output_path: Path
    bgf_dir: Path | None
    bgf_file_path: Path | None
    baf_file_path: Path | None
    obj_dir: Path
    tex_dir: Path

    def __init__(
        self,
        game_path: Path,
        output_path: Path,
        bgf_file_path: Path | None = None,
        baf_file_path: Path | None = None,
    ):
        """Initializes the ModelsDecoder class."""

        if not game_path.exists():
            raise ValueError(f"Input path {game_path} does not exist.")

        self.game_path = game_path
        self.output_path = output_path

        resources_dir = self.game_path / RESOURCES_DIR

        if not resources_dir.exists():
            raise FileNotFoundError(
                f"Input path {self.game_path} does not "
                "contain a valid resources directory."
            )

        textures_bin_path = resources_dir / TEXTURES_BIN

        if not textures_bin_path.exists():
            raise FileNotFoundError("textures.bin file not found.")

        self.obj_dir = output_path / OBJ_DIR
        self.tex_dir = output_path / TEX_DIR
        self.gltf_dir = output_path / GLTF_DIR

        if not self.obj_dir.exists():
            self.obj_dir.mkdir(parents=True)

        if not self.tex_dir.exists():
            self.tex_dir.mkdir(parents=True)

        if not self.gltf_dir.exists():
            self.gltf_dir.mkdir(parents=True)

        extract_zipfile(textures_bin_path, self.tex_dir)

        self.baf_file_path = baf_file_path

        if bgf_file_path:
            self.bgf_file_path = bgf_file_path
            self.bgf_dir = None
            return

        objects_bin_path = resources_dir / OBJECTS_BIN

        if not objects_bin_path.exists():
            raise FileNotFoundError("objects.bin file not found.")

        self.bgf_dir = output_path / BGF_DIR
        self.bgf_file_path = None

        if not self.bgf_dir.exists():
            self.bgf_dir.mkdir(parents=True)

        extract_zipfile(objects_bin_path, self.bgf_dir)

    def decode(self) -> None:
        """Decodes the models of the game."""

        if self.bgf_file_path:
            bgf_file = BgfFile.from_file(self.bgf_file_path)
            baf_file: BafFile | None = None

            if self.baf_file_path:
                baf_file = BafFile.from_file(self.baf_file_path)

            wavefront_file = WavefrontObject.from_bgf_file(bgf_file)
            wavefront_file.write(self.obj_dir, self.tex_dir)

            gltf_object = GltfFile.from_bgf_file(bgf_file, self.tex_dir, baf_file)
            gltf_object.write(self.gltf_dir, self.tex_dir)

        elif self.bgf_dir:
            bgf_paths = get_files(
                self.bgf_dir, extension=BGF_EXTENSION, exclude=BGF_EXCLUDE
            )

            for bgf_path in bgf_paths:
                bgf_file = BgfFile.from_file(bgf_path)
                wavefront_file = WavefrontObject.from_bgf_file(bgf_file)

                gltf_object = GltfFile.from_bgf_file(bgf_file, self.tex_dir)

                obj_path = (
                    rebase_path(bgf_file.path.parent, self.bgf_dir, self.obj_dir)
                    / bgf_file.path.stem
                )

                gltf_path = rebase_path(
                    bgf_file.path.parent, self.bgf_dir, self.gltf_dir
                )

                if not obj_path.parent.exists():
                    obj_path.parent.mkdir(parents=True)

                if not gltf_path.parent.exists():
                    gltf_path.parent.mkdir(parents=True)

                wavefront_file.write(obj_path, self.tex_dir)
                gltf_object.write(gltf_path, self.tex_dir)

        else:
            raise ValueError("No bgf file or directory specified.")


def main() -> None:
    """Main function of the models_decoder module."""

    logger.info("Starting models_decoder...")

    parser = ModelsArgumentParser()
    args = parser.parse_args()

    if not args.game_path:
        root = tk.Tk()
        root.withdraw()

        args.game_path = Path(
            filedialog.askdirectory(
                title="Select the game directory",
            )
        )

        root.destroy()

    logger.info(f"Game path: {args.game_path}")
    logger.info(f"Output path: {args.output_path}")

    if args.bgf_file:
        logger.info(f"Decoding single bgf file: {args.bgf_file}")

    if args.baf_file:
        logger.info(f"Using animation file: {args.baf_file}")

    models_decoder = ModelsDecoder(
        game_path=args.game_path,
        output_path=args.output_path,
        bgf_file_path=args.bgf_file,
        baf_file_path=args.baf_file,
    )
    models_decoder.decode()


if __name__ == "__main__":
    main()
