# Dekoder für SBF (Sound Bank File)-Dateien

# %sep
# %print SBF Header:
# *name : string[304]
# %skip 4
# snd_count : u32
# %skip 16

# %sep
# %rep snd_count
# 	%print Sound:

# 	name : string[48]
# 	%skip 2
# 	variants : u16
# 	%skip 8
# 	file_start : u32
# %endrep

# Hier folgen dann die Daten ein einem Format ähnlich
# foreach(sound in sounds)
# {
#   Sound-Header
#   for ( i = 0; i < sound.variants; i++)
#   {
#     Variant-Header
#     RIFF- oder MP3-Daten
#   }
# }

# Open the binary file in read mode
# get the filename from the command line

import os
import struct


def get_riff_header_positions(filename):
    positions = []
    buffer_size = 1024  # adjust this value based on your needs
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

            # save the last buffer_size bytes for the next iteration
            buffer = buffer[-buffer_size:]

        # handle any remaining partial matches
        pos = buffer.find(b'RIFF')
        while pos != -1:
            positions.append(file.tell() - len(buffer) + pos)
            pos = buffer.find(b'RIFF', pos + 1)

    # remove duplicates
    positions = list(dict.fromkeys(positions))

    return positions
        


def decode_sbf(filename):

    riff_header_positions = get_riff_header_positions(filename)

    with open(filename, 'rb') as file:

        header_data = file.read(324)
        header = {}

        header['name'] = header_data[:304].decode('ascii').rstrip('\x00')
        header['snd_count'] = struct.unpack('<I', header_data[308:312])[0]

        print(header)

        sounds = []

        for i in range(header['snd_count']):
            sound = {}
            sound_data = file.read(64)

            sound['file_start'] = struct.unpack('<L', sound_data[0:4])[0]
            sound['name'] = sound_data[4:52].decode('ascii').rstrip('\x00')
            sound['variants'] = struct.unpack('<H', sound_data[54:56])[0]

            print(sound)

            sounds.append(sound)
        

        for i in range(len(riff_header_positions)):
            file.seek(riff_header_positions[i])
            # read the data until the next RIFF header or the end of the file
            wav_data = file.read(riff_header_positions[i + 1] - riff_header_positions[i] if i < header['snd_count'] - 1 else -1)

            # wav_header_data = file.read(44)
            # wav_header = {}

            # wav_header['chunk_id'] = wav_header_data[:4].decode('ascii')
            # wav_header['chunk_size'] = struct.unpack('<I', wav_header_data[4:8])[0]
            # wav_header['format'] = wav_header_data[8:12].decode('ascii')
            # wav_header['subchunk_id'] = wav_header_data[12:16].decode('ascii')
            # wav_header['subchunk_size'] = struct.unpack('<I', wav_header_data[16:20])[0]
            # wav_header['audio_format'] = struct.unpack('<H', wav_header_data[20:22])[0]
            # wav_header['num_channels'] = struct.unpack('<H', wav_header_data[22:24])[0]
            # wav_header['sample_rate'] = struct.unpack('<I', wav_header_data[24:28])[0]
            # wav_header['byte_rate'] = struct.unpack('<I', wav_header_data[28:32])[0]
            # wav_header['block_align'] = struct.unpack('<H', wav_header_data[32:34])[0]
            # wav_header['bits_per_sample'] = struct.unpack('<H', wav_header_data[34:36])[0]
            # # wav_header['data_id'] = wav_header_data[36:40].decode('ascii')
            # wav_header['data_size'] = struct.unpack('<I', wav_header_data[40:44])[0]

            # print(wav_header)

            # wav_data = file.read(wav_header['chunk_size'])
            # create output folder if it doesn't exist
            if i >= len(sounds):
                sound_name = sounds[-1]['name']
            else:
                sound_name = sounds[i]['name']

            os.makedirs('output', exist_ok=True)
            filename = f'output/{header["name"]}_{i}_{sound_name}.wav'

            with open(filename, 'wb') as wav_file:
                # wav_file.write(wav_header_data)
                wav_file.write(wav_data)


if __name__ == '__main__':
    import sys
    filename = sys.argv[1]
    decode_sbf(filename)