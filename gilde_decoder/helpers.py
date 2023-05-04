import os
import re
import struct
from typing import BinaryIO, Optional

from gilde_decoder.const import MODELS_STRING_CODEC


def read_string(file: BinaryIO) -> str:
    value = ""
    buffer = file.read(1)
    while buffer != b"\x00":
        value += buffer.decode(MODELS_STRING_CODEC)
        buffer = file.read(1)
    return value


def is_value(file: BinaryIO, length: int, value: int | float, reset: bool) -> bool:
    if length not in (1, 2, 3, 4):
        raise ValueError("Length must be between 1 and 4")

    if isinstance(value, float) and length != 4:
        raise ValueError("Length must be 4 for float values")

    fmt: Optional[str] = None

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
        assert (
            int.from_bytes(file.read(1), byteorder="little", signed=False) == 0
        ), f"Expected 0, got {int.from_bytes(file.read(1), byteorder='little', signed=False)}"


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


def subtract_path(base_path, target_path) -> str:
    base_path = os.path.normpath(
        base_path
    )  # Normalize the path to remove redundant separators
    target_path = os.path.normpath(target_path)

    # Make sure target_path starts with base_path
    if not target_path.startswith(base_path):
        raise ValueError("Target path is not a subpath of the base path.")

    relative_path = target_path[
        len(base_path) :
    ]  # Remove the base_path from target_path

    if relative_path.startswith(os.path.sep):
        relative_path = relative_path[
            len(os.path.sep) :
        ]  # Remove the leading path separator, if any

    return relative_path


def sanitize_filename(path, replacement="_"):
    # Define a set of illegal characters for Windows and Unix-based systems
    illegal_characters = r'<>:"/\|?*'
    if os.name == "nt":  # Windows
        illegal_characters += r"\\"  # Add backslash as an illegal character on Windows

    # Replace illegal characters with the replacement character
    sanitized_path = re.sub(f"[{re.escape(illegal_characters)}]", replacement, path)

    return sanitized_path
