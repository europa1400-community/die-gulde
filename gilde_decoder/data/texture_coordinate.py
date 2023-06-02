"""Module containing the TextureCoordinate class."""

from dataclasses import dataclass


@dataclass
class TextureCoordinate:
    """Class representing a texture coordinate."""

    u: float
    v: float
    w: float
