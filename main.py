from pathlib import Path
import re
import cv2
from retinaface import RetinaFace
import numpy as np
import argparse
import time
import tkinter as tk
from tkinterdnd2 import DND_FILES, TkinterDnD
import threading

parser = argparse.ArgumentParser()
parser.add_argument(
    "-v", "--verbose", help="increase output verbosity", action="store_true"
)
parser.add_argument("-d", "--debug", help="enable debug mode", action="store_true")
parser.add_argument(
    "-w", "--wait", help="wait for user input before exiting", action="store_true"
)
parser.add_argument("-i", "--input-directory", help="specify the input directory")
parser.add_argument("-o", "--output-directory", help="specify the output directory")
args = parser.parse_args()

filename_pattern = r"^.+\.(jpe?g|png|bmp|tiff?|webp)$"


def maskf(height, width, center, radius):
    mask = np.zeros((height, width, 3), dtype=np.uint8)
    mask = cv2.ellipse(
        mask,
        center,
        radius,
        0,
        0,
        360,
        (255, 255, 255),
        -1,
    )
    return mask


def blur_faces(p: Path):
    if not p.exists():
        print(f"Skipped image:\t{p} (File does not exist)")
        return

    t = time.time()

    print(f"Detecting faces: {p}")
    img = cv2.imread(p)
    faces = RetinaFace.detect_faces(img)

    if len(list(faces.keys())) == 0:
        print(f"Skipped image:\t{p} (No faces detected)")
        return

    print(
        f"Detected faces:\t{p} ({len(list(faces.keys()))} faces, {time.time() - t} seconds)"
    )

    t = time.time()

    height, width, _ = img.shape
    if args.verbose:
        print(f"Image dimensions: {width} x {height}")

    for k in faces.keys():
        if args.verbose:
            print(f"Face {k}: {faces[k]['facial_area']}")

        x1, y1, x2, y2 = faces[k]["facial_area"]
        center = ((x1 + x2) // 2, (y1 + y2) // 2)
        radius = ((x2 - x1) // 2, (y2 - y1) // 2)

        facial_area = img[y1:y2, x1:x2]
        facial_height, facial_width, _ = facial_area.shape

        if args.verbose:
            print(f"Face dimensions: {facial_width} x {facial_height}")

        mask_center = (facial_width // 2, facial_height // 2)
        mask_radius = (facial_width // 2, facial_height // 2)

        mask = maskf(facial_height, facial_width, mask_center, mask_radius)

        if args.debug:
            print(f"mask_center {mask_center}")
            print(f"mask_radius {mask_radius}")

        blurred_facial_area = cv2.blur(facial_area, ksize=(height // 40, width // 40))

        facial_area2 = np.where(
            mask == 255,
            blurred_facial_area,
            facial_area,
        )

        if args.debug:
            cv2.imshow(
                f"Face {k}",
                cv2.hconcat([facial_area, mask, facial_area2]),
            )
            cv2.waitKey(0)
            cv2.destroyAllWindows()

        img[y1:y2, x1:x2] = facial_area2

        if args.debug:
            stroke_width = max(1, width // 1000)
            img = cv2.rectangle(img, (x1, y1), (x2, y2), (0, 255, 0), stroke_width)

            img = cv2.ellipse(
                img,
                center,
                radius,
                0,
                0,
                360,
                (0, 0, 255),
                stroke_width,
            )
            cv2.imshow(f"Face mask {k}", img)
            cv2.waitKey(0)
            cv2.destroyAllWindows()

    output_path = (
        Path(args.output_directory) if args.output_directory else p.parent / "output"
    )

    output_path.mkdir(exist_ok=True)
    cv2.imwrite(output_path / p.name, img)
    print(f"Saved image:\t{output_path / p.name}\n")

    return


def on_drop(event):
    file_paths = root.tk.splitlist(event.data)

    img_paths = [
        Path(p)
        for p in file_paths
        if Path(p).is_file() and re.match(filename_pattern, Path(p).name.lower())
    ]

    # t0 = time.time()

    for p in img_paths:
        thread = threading.Thread(target=blur_faces, args=(p,))
        thread.start()

    if args.wait:
        input("Press Enter to exit...")

    # print(f"Finished, {time.time() - t0} seconds in total.")


if __name__ == "__main__":
    root = TkinterDnD.Tk()
    root.geometry("600x400")
    root.title("inai-inai")

    label = tk.Label(root, text="Drag & drop images here.")
    label.pack(expand=True, fill="both")

    root.drop_target_register(DND_FILES)
    root.dnd_bind("<<Drop>>", on_drop)
    root.mainloop()
