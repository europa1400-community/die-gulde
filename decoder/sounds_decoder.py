import fnmatch
import os
import struct


def decode_sounds(input_path: str, output_path: str):
    if input_path is None or output_path is None:
        return
    
    file_paths = []
    for root, directories, files in os.walk(input_path):
        for filename in fnmatch.filter(files, '*.sbf'):
            filepath = os.path.join(root, filename)
            file_paths.append(filepath)


    for sbf_path in file_paths:
        decode_sound(sbf_path, output_path)


def get_riff_header_positions(filename):
    positions = []
    buffer_size = 1024
    buffer = b''

    with open(filename, 'rb') as file:
        while True:
            data = file.read(buffer_size)
            if not data:
                break

            buffer += data
            pos = buffer.find(b'RIFF')
            while pos != -1:
                positions.append(file.tell() - len(buffer) + pos)
                pos = buffer.find(b'RIFF', pos + 1)

            buffer = buffer[-buffer_size:]

        pos = buffer.find(b'RIFF')
        while pos != -1:
            positions.append(file.tell() - len(buffer) + pos)
            pos = buffer.find(b'RIFF', pos + 1)

    positions = list(dict.fromkeys(positions))

    return positions
        

def decode_sound(input_path: str, output_path: str):
    if input_path is None or output_path is None:
        return
    
    if not os.path.exists(input_path):
        raise Exception("Input file doesn't exist")
    
    output_sounds_path = os.path.join(output_path, 'sounds')

    if not os.path.exists(output_sounds_path):
        os.mkdir(output_sounds_path)

    riff_header_positions = get_riff_header_positions(input_path)

    with open(input_path, 'rb') as file:
        header_data = file.read(324)
        header = {}

        header['name'] = header_data[:304].decode('ascii').rstrip('\x00')
        header['snd_count'] = struct.unpack('<I', header_data[308:312])[0]

        sounds = []

        for i in range(header['snd_count']):
            sound = {}
            sound_data = file.read(64)

            sound['file_start'] = struct.unpack('<L', sound_data[0:4])[0]
            sound['name'] = sound_data[4:52].decode('ascii').rstrip('\x00')
            sound['variants'] = struct.unpack('<H', sound_data[54:56])[0]

            sounds.append(sound)

        if len(riff_header_positions) != 0:
            for i in range(len(riff_header_positions)):
                file.seek(riff_header_positions[i])
                
                wav_data = file.read(riff_header_positions[i + 1] - riff_header_positions[i] if i < header['snd_count'] - 1 else -1)

                if i >= len(sounds):
                    sound_name = sounds[-1]['name']
                else:
                    sound_name = sounds[i]['name']

                wav_path = os.path.join(output_sounds_path, f'{header["name"]}_{i}_{sound_name}.wav')

                with open(wav_path, 'wb') as wav_file:
                    wav_file.write(wav_data)
        else:
            MP3_HEADER_OFFSET = 11

            for i in range(len(sounds)):
                if i >= len(sounds):
                    sound_name = sounds[-1]['name']
                else:
                    sound_name = sounds[i]['name']

                mp3_path = os.path.join(output_sounds_path, f'{header["name"]}_{i}_{sound_name}.mp3')

                file.seek(sounds[i]['file_start'] + MP3_HEADER_OFFSET)
                mp3_data = file.read(sounds[i+1]['file_start'] - sounds[i]['file_start'] - MP3_HEADER_OFFSET if i <  header['snd_count'] - 1 else -1)
                
                with open(mp3_path, 'wb') as mp3_file:
                    mp3_file.write(mp3_data)
