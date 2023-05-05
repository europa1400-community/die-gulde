"""Constants for the gilde_decoder package."""

from pathlib import Path

RESOURCES_DIR = "Resources"
OBJECTS_BIN = "objects.bin"
TEXTURES_BIN = "textures.bin"
BGF_DIR = "bgf"
OBJ_DIR = "obj"
TEX_DIR = "tex"

MODELS_STRING_ENCODING = "latin-1"

MODELS_REDUCED_FOOTER_FILES = [
    "ob_DREIFACHGALGEN.bgf",
    "ob_DREIFACKREUZ.bgf",
    "ob_EXEKUTIONSKANONESTOPFER.bgf",
]

MODELS_EXCLUDE_PATHS: list[Path] = []

BGF_EXTENSION = ".bgf"
OBJ_EXTENSION = ".obj"
MTL_EXTENSION = ".mtl"

WAVEFRONT_ENCODING = "utf-8"
