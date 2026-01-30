"""Demo create map"""
import logging
import sys
from azure.monitor.opentelemetry import configure_azure_monitor

from pipeline_convective_outlook_maps import PipelineConvectiveOutlookMaps
from dataset_grib import DatasetGrib
from dataset_images import DatasetImages

# Configure Logging
logging.basicConfig(level=logging.INFO)
configure_azure_monitor(logger_name='olieblind.brown')
logger = logging.getLogger('olieblind.brown')


def parse_command_line_arguments() -> tuple[str, str, str]:
    """Read and validate the command line arguments"""
    logger.info("Olieblind.Brown: python prold2.py %s", str(sys.argv))

    try:
        if len(sys.argv) != 4:
            raise ValueError("Wrong number of arguments")

        grib_file = sys.argv[1]
        image_folder = sys.argv[2]
        file_prefix = sys.argv[3]

        return (grib_file, image_folder, file_prefix)
    except Exception:
        print("Usage: python prold2.py grib_file image_folder file_prefix")

        raise

def main():
    """Entry point for the command"""
    try:
        grib_file, image_folder, file_prefix = parse_command_line_arguments()

        ds_grib = DatasetGrib(grib_file)
        ds_images = DatasetImages(image_folder, file_prefix, 1)
        pcom = PipelineConvectiveOutlookMaps(ds_grib, ds_images, logger)

        pcom.create_surface_dewpoint()
        pcom.create_surface_temp()
        pcom.create_500_vorticity()
        pcom.create_300_wind()
        pcom.create_500_wind()
        pcom.create_700_rh()
        pcom.create_700_temp()
        pcom.create_850_wind()
        pcom.create_cape()
        pcom.create_cin()
        pcom.create_srh()
        pcom.create_shear()
        pcom.create_lcl()
        pcom.create_stp()
        pcom.create_scp()
        pcom.create_rh_storm_motion()

        logger.info("Olieblind.Brown clean exit")

    except Exception as err:  # pylint: disable=broad-exception-caught
        logger.error(str(err))
        sys.exit(1)

if __name__ == "__main__":
    main()
