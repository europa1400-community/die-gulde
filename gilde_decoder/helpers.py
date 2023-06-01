import base64
import os
import re
import struct
from pathlib import Path
from typing import BinaryIO
from zipfile import ZipFile

import construct as cs

from gilde_decoder.const import MODELS_STRING_ENCODING


def bytes_to_gltf_uri(data: bytes) -> str:
    encoded_data = base64.b64encode(data).decode("utf-8")
    return f"data:application/octet-stream;base64,{encoded_data}"


def HexConst(hex_str: str) -> cs.Const:
    if len(hex_str) % 2 != 0:
        raise RuntimeError("Hex string must be of even size")
    return cs.Const(bytes.fromhex(hex_str))


def OptionalTaggedValue(tag: str, value_type=cs.Int32ul) -> cs.Struct:
    return cs.Struct(
        "has_value" / cs.Optional(HexConst(tag)),
        "value" / cs.If(cs.this.has_value, value_type),
    )


def read_string(file: BinaryIO) -> str:
    value = ""
    buffer = file.read(1)
    while buffer != b"\x00":
        value += buffer.decode(MODELS_STRING_ENCODING)
        buffer = file.read(1)
    return value


def is_value(file: BinaryIO, length: int, value: int | float, reset: bool) -> bool:
    if length not in (1, 2, 3, 4):
        raise ValueError("Length must be between 1 and 4")

    if isinstance(value, float) and length != 4:
        raise ValueError("Length must be 4 for float values")

    fmt: str | None = None

    if isinstance(value, float):
        fmt = "<f"
    elif length == 1:
        fmt = "<B"
    elif length == 2:
        fmt = "<H"
    elif length == 3:
        fmt = None
    else:
        fmt = "<I"

    initial_pos = file.tell()

    data = file.read(length)

    if len(data) < length:
        if reset:
            file.seek(initial_pos)
        return False

    if length == 3:
        unpacked_value = int.from_bytes(data, byteorder="little", signed=False)
    elif fmt is None:
        unpacked_value = struct.unpack("<I", data + b"\x00")[0]
    else:
        unpacked_value = struct.unpack(fmt, data)[0]

    is_equal = unpacked_value == value

    if reset:
        file.seek(initial_pos)

    return is_equal


def skip_optional(file: BinaryIO, value: bytes, padding: int) -> bool:
    read_value = file.read(len(value))
    file.seek(-len(value), os.SEEK_CUR)

    if read_value != value:
        return False

    file.seek(padding, os.SEEK_CUR)
    return True


def skip_required(file: BinaryIO, value: bytes, padding: int) -> None:
    read_value = file.read(len(value))
    file.seek(-len(value), os.SEEK_CUR)

    assert read_value == value, f"Expected {str(value)}, got {str(read_value)}"

    file.seek(padding, os.SEEK_CUR)


def skip_zero(file: BinaryIO, length: int) -> None:
    for _ in range(length):
        value = int.from_bytes(file.read(1), byteorder="little", signed=False)
        if value != 0:
            raise ValueError(f"Expected 0, got {value}")


def skip_until(file: BinaryIO, value: int, length: int) -> None:
    while not is_value(file, length, value, reset=True):
        file.seek(1, os.SEEK_CUR)
    file.seek(1, os.SEEK_CUR)


def is_latin_1(value: int) -> bool:
    return 0x20 <= value <= 0x7E


def find_address_of_byte_pattern(pattern: bytes, data: bytes) -> list[int]:
    if not pattern or not data:
        return []

    pattern_length = len(pattern)
    data_length = len(data)
    occurrences = []

    for i in range(data_length - pattern_length + 1):
        if (
            (i == 0 or not is_latin_1(data[i - 1]))
            and data[i : i + pattern_length] == pattern
            and data[i + pattern_length] == 0
        ):
            occurrences.append(i)

    return occurrences


def rebase_path(path: Path, base_path: Path, target_path: Path) -> Path:
    """Rebases the specified path to the specified target path."""

    path = path.resolve()
    base_path = base_path.resolve()
    target_path = target_path.resolve()

    if not path.is_relative_to(base_path):
        raise ValueError("Path must be a subpath of the base path.")

    relative_path = path.relative_to(base_path)
    return target_path / relative_path


def sanitize_filename(path, replacement="_"):
    illegal_characters = r'<>:"/\|?*'
    if os.name == "nt":
        illegal_characters += r"\\"

    sanitized_path = re.sub(f"[{re.escape(illegal_characters)}]", replacement, path)
    return sanitized_path


def extract_zipfile(input_path: Path, output_path: Path) -> None:
    """Extracts the contents of a zip file to the specified output path."""

    if not input_path.is_file():
        raise FileNotFoundError(f"File not found: {input_path}")

    if not output_path.is_dir():
        raise FileNotFoundError(f"Directory not found: {output_path}")

    output_subdir = output_path / input_path.stem

    if not output_subdir.is_dir():
        output_subdir.mkdir()

    with ZipFile(input_path, "r") as zip_file:
        zip_file.extractall(output_path)


def get_files(
    path: Path, extension: str | None = None, exclude: list[Path] = []
) -> list[Path]:
    """Returns a list of files in the specified directory and its subdirectories."""

    file_paths: list[Path] = []

    for root, _, files in os.walk(path):
        for file in files:
            file_path = Path(root) / file

            if exclude is not None and file_path in exclude:
                continue

            if extension is not None and file_path.suffix != extension:
                continue

            file_paths.append(file_path)

    return file_paths
