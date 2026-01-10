"""Create a suite of weather maps from a grib file"""
from logging import Logger
from dataset_grib import DatasetGrib
from dataset_images import DatasetImages
from olie_plotter import OliePlotter
import dataset_grib as dg

class PipelineConvectiveOutlookMaps:
    """Create weather maps from a grib file"""
    def __init__(self, dataset_grib: DatasetGrib, dataset_images: DatasetImages, logger: Logger):
        self.grib = dataset_grib
        self.images = dataset_images
        extent = (-126., 24., -66., 50.)
        height = 1080
        self.plotter = OliePlotter(extent, height)
        self.logger = logger

    def create_300_wind(self):
        """Create a 300mb weather map"""
        self.logger.info("Olieblind.Purple Create 300mb Wind")

        grib = self.grib.select_by_isobar(300)
        ef = dg.get_timestamp(grib)
        image_path = self.images.get_filename('300mb')

        def plot():
            ku = dg.convert_u_knots(grib)
            kv = dg.convert_v_knots(grib)

            v = OliePlotter.HeightValueBarb(\
                dg.convert_gh_dam(grib), \
                dg.convert_uv_magnitude(ku, kv), \
                ku, \
                kv, \
                dg.get_lat(grib), \
                dg.get_lon(grib))
            self.plotter.plot_300_wind(v)

        self.plotter.do_plot('300mb (Height, Wind, Barbs)', plot, ef, image_path)

    def create_500_vorticity(self):
        """Create a 500mb voriticity map"""
        self.logger.info("Olieblind.Purple Create 500mb Vorticity")

        grib = self.grib.select_by_isobar(500)
        ef = dg.get_timestamp(grib)
        image_path = self.images.get_filename("Vorticity")

        def plot():
            v = OliePlotter.HeightValue(\
                dg.convert_gh_dam(grib), \
                dg.convert_vorticity(grib), \
                dg.get_lat(grib), \
                dg.get_lon(grib))
            self.plotter.plot_500_vorticity(v)

        self.plotter.do_plot('500mb (Height, Absolute Vorticity)', plot, ef, image_path)

    def create_500_wind(self):
        """Create a 500mb weather map"""
        self.logger.info("Olieblind.Purple Create 500mb Wind")

        grib = self.grib.select_by_isobar(500)
        ef = dg.get_timestamp(grib)
        image_path = self.images.get_filename('500mb')

        def plot():
            v = OliePlotter.HeightValueBarb(\
                dg.convert_gh_dam(grib), \
                dg.convert_temp_c(grib), \
                dg.convert_u_knots(grib), \
                dg.convert_v_knots(grib), \
                dg.get_lat(grib), \
                dg.get_lon(grib))
            m = dg.convert_uv_magnitude(v.u, v.v)
            self.plotter.plot_500_wind(v, m)

        self.plotter.do_plot('500mb (Height, Wind, Temp, Barbs)', plot, ef, image_path)

    def create_700_rh(self):
        """Create a 700mb relative humidity map"""
        self.logger.info("Olieblind.Purple Create 700mb Rh")

        grib = self.grib.select_by_isobar(700)
        ef = dg.get_timestamp(grib)
        image_path = self.images.get_filename('700mbR')

        def plot():
            v = OliePlotter.HeightValueBarb(\
                dg.convert_gh_dam(grib), \
                dg.get_rh(grib), \
                dg.convert_u_knots(grib), \
                dg.convert_v_knots(grib), \
                dg.get_lat(grib), \
                dg.get_lon(grib))
            self.plotter.plot_700_rh(v)

        self.plotter.do_plot('700mb (Height, Rh, Barbs)', plot, ef, image_path)

    def create_700_temp(self):
        """Create a 700mb Temperature map"""
        self.logger.info("Olieblind.Purple Create 700mb Temp")

        grib = self.grib.select_by_isobar(700)
        ef = dg.get_timestamp(grib)
        image_path = self.images.get_filename('700mbT')

        def plot():
            v = OliePlotter.HeightValueBarb(\
                dg.convert_gh_dam(grib), \
                dg.convert_temp_c(grib), \
                dg.convert_u_knots(grib), \
                dg.convert_v_knots(grib), \
                dg.get_lat(grib), \
                dg.get_lon(grib))
            self.plotter.plot_700_temp(v)

        self.plotter.do_plot('700mb (Height, Temp, Barbs)', plot, ef, image_path)

    def create_850_wind(self):
        """Create a 850mb wind map"""
        self.logger.info("Olieblind.Purple Create 850mb Wind")

        grib = self.grib.select_by_isobar(850)
        ef = dg.get_timestamp(grib)
        image_path = self.images.get_filename('850mb')

        def plot():
            v = OliePlotter.HeightValueBarb(\
                dg.convert_gh_dam(grib), \
                dg.convert_temp_c(grib), \
                dg.convert_u_knots(grib), \
                dg.convert_v_knots(grib), \
                dg.get_lat(grib), \
                dg.get_lon(grib))
            m = dg.convert_uv_magnitude(v.u, v.v)
            self.plotter.plot_850_wind(v, m)

        self.plotter.do_plot('850mb (Height, Wind, Temp)', plot, ef, image_path)

    def create_cape(self):
        """Create a severe parameter CAPE map"""
        self.logger.info("Olieblind.Purple Create CAPE")

        grib = self.grib.select_by_surface('instant')
        ef = dg.get_timestamp(grib)
        image_path = self.images.get_filename("Cape")

        def plot():
            v = OliePlotter.HeightValue(\
                None, \
                dg.get_cape(grib), \
                dg.get_lat(grib), \
                dg.get_lon(grib))
            self.plotter.plot_cape(v)

        self.plotter.do_plot('Surface Based CAPE', plot, ef, image_path)

    def create_cin(self):
        """Create convective inversion weather map"""
        self.logger.info("Olieblind.Purple Create CIN")

        grib = self.grib.select_by_surface('instant')
        ef = dg.get_timestamp(grib)
        image_path = self.images.get_filename("Cin")

        def plot():
            v = OliePlotter.HeightValue(\
                dg.get_cape(grib), \
                dg.get_cin(grib), \
                dg.get_lat(grib), \
                dg.get_lon(grib))
            self.plotter.plot_cin(v)

        self.plotter.do_plot('Surface Based CIN (Masked by Cape < 1000)', plot, ef, image_path)

    def create_lcl(self):
        """Create a lifted condensation level weather map"""
        self.logger.info("Olieblind.Purple Create LCL")

        grib = self.grib.select_by_hag(2)
        ef = dg.get_timestamp(grib)
        image_path = self.images.get_filename("Lcl")

        def plot():
            cape_grib = self.grib.select_by_surface('instant')

            v = OliePlotter.HeightValue(\
                dg.get_cape(cape_grib), \
                dg.derive_olie_lcl(grib), \
                dg.get_lat(grib), \
                dg.get_lon(grib))
            self.plotter.plot_lcl(v)

        self.plotter.do_plot("Olie's LCL Estimate (Masked by Cape < 1000)", plot, ef, image_path)

    def create_scp(self):
        """Create a map using Olie's supercell composite algorithm"""
        self.logger.info("Olieblind.Purple Create SCP")

        grib = self.grib.select_by_surface('instant')
        ef = dg.get_timestamp(grib)
        image_path = self.images.get_filename("Scp")

        def plot():
            grib10 = self.grib.select_by_hag(10)
            grib500 = self.grib.select_by_isobar(500)
            grib3000 = self.grib.select_by_hagl(3000)

            v = OliePlotter.HeightValue(\
                None, \
                dg.derive_olie_scp(grib, grib10, grib500, grib3000), \
                dg.get_lat(grib), \
                dg.get_lon(grib))
            self.plotter.plot_scp(v)

        self.plotter.do_plot("Olie's Supercell Composite", plot, ef, image_path)

    def create_shear(self):
        """Create bulk shear weather map"""
        self.logger.info("Olieblind.Purple Create Shear")

        grib500 = self.grib.select_by_isobar(500)
        ef = dg.get_timestamp(grib500)
        image_path = self.images.get_filename("Shear")

        def plot():
            grib10 = self.grib.select_by_hag(10)

            v = OliePlotter.HeightValueBarb(\
                dg.convert_gh_dam(grib500), \
                None, \
                dg.derive_bulk_shear_u(grib10, grib500), \
                dg.derive_bulk_shear_v(grib10, grib500), \
                dg.get_lat(grib500), \
                dg.get_lon(grib500))
            v.x = dg.convert_uv_magnitude(v.u, v.v)
            self.plotter.plot_shear(v)

        self.plotter.do_plot("Olie's Bulk Shear (Surface to 500mb Shear, 500mb Height)",
                             plot, ef, image_path)

    def create_srh(self):
        """Create storm relative helicity weather map"""
        self.logger.info("Olieblind.Purple Create SRH")

        grib = self.grib.select_by_hagl(3000)
        ef = dg.get_timestamp(grib)
        image_path = self.images.get_filename("Srh")

        def plot():
            v = OliePlotter.HeightValue(\
                None, \
                dg.get_srh(grib), \
                dg.get_lat(grib), \
                dg.get_lon(grib))
            self.plotter.plot_srh(v)

        self.plotter.do_plot('3km Storm Relative Helicity', plot, ef, image_path)

    def create_rh_storm_motion(self):
        """Create a map representing right-handed storm motion"""
        self.logger.info("Olieblind.Purple Create RH-Storm Motion")

        grib10 = self.grib.select_by_hag(10)
        ef = dg.get_timestamp(grib10)
        image_path = self.images.get_filename("StormMotion")

        def plot():
            grib500 = self.grib.select_by_isobar(500)
            grib700 = self.grib.select_by_isobar(700)
            grib850 = self.grib.select_by_isobar(850)
            rh_u, rh_v = dg.derive_olie_rh_storm_motion(grib10, grib500, grib700, grib850)
            m = dg.convert_uv_magnitude(rh_u, rh_v)

            v = OliePlotter.HeightValueBarb(\
                m, \
                m, \
                rh_u, \
                rh_v, \
                dg.get_lat(grib10), \
                dg.get_lon(grib10))
            self.plotter.plot_storm_motion(v)

        self.plotter.do_plot("Olie's Storm Motion (Right Mover)", plot, ef, image_path)

    def create_stp(self):
        """Create a map using Olie's significant tornago algorithm"""
        self.logger.info("Olieblind.Purple Create STP")

        grib = self.grib.select_by_surface('instant')
        ef = dg.get_timestamp(grib)
        image_path = self.images.get_filename("Stp")

        def plot():
            grib2 = self.grib.select_by_hag(2)
            grib10 = self.grib.select_by_hag(10)
            grib500 = self.grib.select_by_isobar(500)
            grib3000 = self.grib.select_by_hagl(3000)

            v = OliePlotter.HeightValue(\
                None, \
                dg.derive_olie_stp(grib, grib2, grib10, grib500, grib3000), \
                dg.get_lat(grib), \
                dg.get_lon(grib))
            self.plotter.plot_stp(v)

        self.plotter.do_plot("Olie's Significant Tornado Composite", plot, ef, image_path)

    def create_surface_dewpoint(self):
        """Create a surface dewpoint map"""
        self.logger.info("Olieblind.Purple Create Surface Dewpoint")

        hag2_grib = self.grib.select_by_hag(2)
        ef = dg.get_timestamp(hag2_grib)
        image_path = self.images.get_filename("Dewpoint")

        def plot():
            ms_grib = self.grib.select_by_ms()
            hag10_grib = self.grib.select_by_hag(10)

            v = OliePlotter.HeightValueBarb(\
                dg.convert_prmsl_mbar(ms_grib), \
                dg.convert_d2m_f(hag2_grib), \
                dg.convert_u10_knots(hag10_grib), \
                dg.convert_v10_knots(hag10_grib), \
                dg.get_lat(hag2_grib), \
                dg.get_lon(hag2_grib))
            self.plotter.plot_surface_dewpoint(v)

        self.plotter.do_plot('Surface Dewpoints (Dewpoint, MSLP, Barbs)', plot, ef, image_path)

    def create_surface_temp(self):
        """Create a surface temperature map"""
        self.logger.info("Olieblind.Purple Create Surface Temperature")

        hag2_grib = self.grib.select_by_hag(2)
        ef = dg.get_timestamp(hag2_grib)
        image_path = self.images.get_filename("Temperature")

        def plot():
            ms_grib = self.grib.select_by_ms()
            hag10_grib = self.grib.select_by_hag(10)

            v = OliePlotter.HeightValueBarb(\
                dg.convert_prmsl_mbar(ms_grib), \
                dg.convert_t2m_f(hag2_grib), \
                dg.convert_u10_knots(hag10_grib), \
                dg.convert_v10_knots(hag10_grib), \
                dg.get_lat(hag2_grib), \
                dg.get_lon(hag2_grib))
            self.plotter.plot_surface_temperature(v)

        self.plotter.do_plot('Surface Temperature (Temperature, MSLP, Barbs)', plot, ef, image_path)
