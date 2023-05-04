import json
import os
import struct


def decode_buildings(input_path: str, output_path: str):
    if input_path is None or output_path is None:
        return
    
    with open(input_path, 'rb') as file:
        data = file.read(589)

        buildings = []

        while data:
            building = {}

            building_type = struct.unpack('<B', data[:1])[0]
            building_name = data[1:33].decode('ascii').rstrip('\x00')
            building_time = struct.unpack('<I', data[-10:-6])[0]
            building_level = struct.unpack('<B', data[583:584])[0]
            building_price = struct.unpack('<I', data[584:588])[0]
            
            if building_type == 0:
                data = file.read(589)
                continue

            building['building_type'] = str(building_type)
            building['building_name'] = str(building_name)
            building['building_time'] = str(building_time)
            building['building_level'] = str(building_level)
            building['building_price'] = str(building_price)
            
            data = file.read(589)

            buildings.append(building)

    buildings_json_path = os.path.join(output_path, 'buildings.json')

    with open(buildings_json_path, 'w', encoding='utf-8') as jsonfile:
        json.dump(buildings, jsonfile, ensure_ascii=False, indent=4)
