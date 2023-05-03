import json
import os
import struct


def decode_objects(input_path: str, output_path: str):
    if input_path is None or output_path is None:
        return
    
    with open(input_path, 'rb') as file:

        data = file.read(65)

        objects = []

        while data:
            object = {}

            object['object_type'] = struct.unpack('<B', data[:1])[0]
            object['object_name'] = data[1:33].decode('ascii').rstrip('\x00')
            object['object_level'] = struct.unpack('<B', data[33:34])[0]
            object['object_time'] = struct.unpack('<I', data[34:38])[0]
            object['object_prop_idx'] = struct.unpack('<HHHH', data[38:46])
            object['object_prop_val'] = struct.unpack('<HHHH', data[46:54])
            object['object_thingy'] = struct.unpack('<H', data[54:56])[0]
            object['object_price'] = struct.unpack('<H', data[56:58])[0]
            object['object_value'] = struct.unpack('<H', data[60:62])[0]
            
            if object['object_name'] == "":
                data = file.read(65)
                continue
            
            data = file.read(65)

            objects.append(object)

    objects_json_path = os.path.join(output_path, 'objects.json')

    with open(objects_json_path, 'w', encoding='utf-8') as jsonfile:
        json.dump(objects, jsonfile, ensure_ascii=False, indent=4)
