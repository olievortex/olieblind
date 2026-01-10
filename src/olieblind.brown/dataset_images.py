"""Image writer"""
import os

class DatasetImages:
    """Image writer"""
    def __init__(self, image_path: str, prefix: str, frame: int):
        self.image_path = image_path
        self.prefix = prefix
        self.frame = frame

        os.makedirs(self.image_path, exist_ok=True)

    def get_filename(self, group: str) -> str:
        """Generate a filename"""
        return f"{self.image_path}/{self.prefix}_{group}_{self.frame}.png"

    def is_exists(self, group:str) -> bool:
        """See if the file already exists"""
        file = self.get_filename(group)

        return os.path.isfile(file)
