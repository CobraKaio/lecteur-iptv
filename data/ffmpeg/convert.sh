#!/bin/bash
# Script de conversion de flux IPTV avec FFmpeg

# Vérification des arguments
if [ $# -lt 2 ]; then
    echo "Usage: $0 <input_url> <output_format>"
    echo "Formats supportés: hls, dash, mp4"
    exit 1
fi

INPUT_URL=$1
OUTPUT_FORMAT=$2
OUTPUT_DIR="output"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

# Création du répertoire de sortie s'il n'existe pas
mkdir -p $OUTPUT_DIR

case $OUTPUT_FORMAT in
    "hls")
        echo "Conversion en HLS..."
        ffmpeg -i "$INPUT_URL" \
            -c:v libx264 -preset fast -crf 22 \
            -c:a aac -b:a 128k \
            -f hls \
            -hls_time 4 \
            -hls_playlist_type vod \
            -hls_segment_filename "$OUTPUT_DIR/${TIMESTAMP}_segment_%03d.ts" \
            "$OUTPUT_DIR/${TIMESTAMP}_playlist.m3u8"
        ;;
    "dash")
        echo "Conversion en DASH..."
        ffmpeg -i "$INPUT_URL" \
            -c:v libx264 -preset fast -crf 22 \
            -c:a aac -b:a 128k \
            -f dash \
            -init_seg_name "$OUTPUT_DIR/${TIMESTAMP}_init_\$RepresentationID\$.m4s" \
            -media_seg_name "$OUTPUT_DIR/${TIMESTAMP}_chunk_\$RepresentationID\$_\$Number%05d\$.m4s" \
            "$OUTPUT_DIR/${TIMESTAMP}_manifest.mpd"
        ;;
    "mp4")
        echo "Conversion en MP4..."
        ffmpeg -i "$INPUT_URL" \
            -c:v libx264 -preset fast -crf 22 \
            -c:a aac -b:a 128k \
            "$OUTPUT_DIR/${TIMESTAMP}_video.mp4"
        ;;
    *)
        echo "Format non supporté: $OUTPUT_FORMAT"
        echo "Formats supportés: hls, dash, mp4"
        exit 1
        ;;
esac

echo "Conversion terminée. Fichiers disponibles dans le répertoire $OUTPUT_DIR"
