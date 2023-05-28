"""Constants for the gilde_decoder package."""

from pathlib import Path

RESOURCES_DIR = "Resources"
OBJECTS_BIN = "objects.bin"
TEXTURES_BIN = "textures.bin"
ANIMATIONS_BIN = "animations.bin"
BGF_DIR = "bgf"
OBJ_DIR = "obj"
TEX_DIR = "tex"
BAF_DIR = "baf"

MODELS_STRING_ENCODING = "latin-1"

MODELS_REDUCED_FOOTER_FILES = [
    "ob_DREIFACHGALGEN.bgf",
    "ob_DREIFACKREUZ.bgf",
    "ob_EXEKUTIONSKANONESTOPFER.bgf",
]

BGF_EXCLUDE: list[Path] = []
BAF_EXCLUDE: list[Path] = []

BGF_EXTENSION = ".bgf"
OBJ_EXTENSION = ".obj"
MTL_EXTENSION = ".mtl"
BAF_EXTENSION = ".baf"

WAVEFRONT_ENCODING = "utf-8"
