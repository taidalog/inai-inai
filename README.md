# inai-inai

A privacy tool that blurs faces in images.

The name "inai-inai" comes from "inai inai baa!", the Japanese version of peek-a-boo.

## Features

- Detects faces in an image and blurs them.

## Requirements

- Python 3.13

## Getting Started

Set up the environment:

```
py -3.13 -m venv .venv
.venv\Scripts\activate
py -m pip install --upgrade pip
py -m pip install -r requirements.txt
```

Then run the script:

```
python main.py
```

## Usage

1. Set up the environment.
1. Run the script (a white window saying "Drag & drop images here." will appear).
1. Drag and drop your image files onto the window.

## Known Issue

- The application stops when an input image file contains whitespace or Japanese characters in its filename.

## Release Notes

[Releases on GitHub](https://github.com/taidalog/inai-inai/releases)

## License

This application is licensed under [Apache License Version 2.0](https://github.com/taidalog/inai-inai/blob/main/LICENSE).

## Copyright

Copyright 2026 taidalog
