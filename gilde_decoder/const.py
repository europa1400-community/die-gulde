"""Constants for the gilde_decoder package."""

from pathlib import Path

RESOURCES_DIR = "Resources"
OBJECTS_BIN = "objects.bin"
TEXTURES_BIN = "textures.bin"
ANIMATIONS_BIN = "animations.bin"
BGF_DIR = "bgf"
OBJ_DIR = "obj"
TEX_DIR = "tex"
GLTF_DIR = "gltf"
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
GLTF_EXTENSION = ".gltf"
GLB_EXTENSION = ".glb"
PNG_EXTENSION = ".png"
INI_EXTENSION = ".ini"

WAVEFRONT_ENCODING = "utf-8"

BAF_INI_FILE_SECTION = "4HEAD Studios Animation-Settings"
BAF_INI_FILE_NUM_KEYS = "NumKeys"
BAF_INI_FILE_KEYS = "Keys"
BAF_INI_FILE_LOOP_IN = "LoopIn"
BAF_INI_FILE_LOOP_OUT = "LoopOut"
