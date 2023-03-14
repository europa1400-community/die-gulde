import os
from PIL import Image


def decode_graphics(input_path: str, output_path: str):
    if input_path is None or output_path is None:
        raise ValueError("Input and output paths must be specified")

    if not os.path.exists(input_path):
        raise FileNotFoundError("Input file not found")
    
    output_graphics_path = os.path.join(output_path, "graphics")

    if not os.path.exists(output_graphics_path):
        os.mkdir(output_graphics_path)

    with open(input_path, "rb") as file:
        header_data = file.read(4)

        graphics_count = int.from_bytes(header_data, byteorder="little", signed=False)

        print(f"Found {graphics_count} graphics")

        graphics = []

        for i in range(graphics_count):
            graphics_header_data = file.read(84)
            graphics_name = graphics_header_data[:32].decode("ascii").strip("\x00")
            graphics_relative_start_address = int.from_bytes(graphics_header_data[48:52], byteorder="little", signed=False)
            graphics_width = int.from_bytes(graphics_header_data[80:82], byteorder="little", signed=False)
            graphics_height = int.from_bytes(graphics_header_data[82:84], byteorder="little", signed=False)

            print(f"Decoding {graphics_name} ({graphics_width}x{graphics_height}) at +{graphics_relative_start_address}")

            graphics.append({
                "name": graphics_name,
                "relative_start_address": graphics_relative_start_address,
                "width": graphics_width,
                "height": graphics_height
            })

        for graphic in graphics:
            file.seek(graphic["relative_start_address"], os.SEEK_CUR)

            graphic_data = file.read(graphic["width"] * graphic["height"] * 3)

            img = Image.frombytes(mode="RGB", size=(graphic["width"], graphic["height"]), data=graphic_data)

            img.show()




if __name__ == "__main__":
    decode_graphics("C:\\Users\\lbeer\\Games\\Die Gilde Gold-Edition\\gfx\\Gilde_add_on_german.gfx", "output")
