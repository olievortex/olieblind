"""Map plotting"""
import os
from dataclasses import dataclass
from datetime import datetime
import cartopy.crs as ccrs
import cartopy.feature as cfeature
import matplotlib
import matplotlib.colors as col
import matplotlib.pyplot as plt
import numpy as np
import xarray as xr

from scipy.ndimage import gaussian_filter

class OliePlotter:
    """Map plotting"""
    def __init__(self, extent: list, height: int):
        self.extent = extent
        self.transform = ccrs.PlateCarree()
        self.fig = None
        self.ax = None
        self.height = height

        matplotlib.use('Agg')

    def do_plot(self, title: str, command, effective: datetime, image_path: str):
        """Perform all the steps to create an image"""
        if os.path.isfile(image_path):
            print(f"Skipped '{title}'")
            return

        print(f"Produce '{title}'")

        self.open_plot()
        self.brand_plot(title, effective)
        command()
        self.save_plot(image_path)
        self.close_plot()

    #region Plot helpers

    @dataclass
    class HeightValueBarb:
        """Plot with height, value, and wind barb values"""
        h: xr.Dataset
        x: xr.Dataset
        u: xr.Dataset
        v: xr.Dataset
        lats: xr.Dataset
        lons: xr.Dataset

    @dataclass
    class HeightValue:
        """Plot with height, value, and wind barb values"""
        h: xr.Dataset
        x: xr.Dataset
        lats: xr.Dataset
        lons: xr.Dataset

    def open_plot(self):
        """Start a new plot"""

        # Figure out the dimensions
        l_width = self.extent[2] - self.extent[0]
        l_height = self.extent[3] - self.extent[1]
        l_ratio = l_width / l_height
        r_width = self.height * l_ratio

        self.fig = plt.figure(figsize=(r_width / 100, self.height / 100), dpi=100)
        self.ax = plt.axes(projection=self.transform)
        self.ax.clear()
        self.ax.set_extent([self.extent[0]+360.0, self.extent[2]+360.0, self.extent[1],
                            self.extent[3]], crs=self.transform)
        self.ax.set_axis_off()
        self.ax.set_position([0, 0, 1, 1])

    def brand_plot(self, title: str, time: datetime):
        """Add the coast lines, title, and branding"""
        branding = r"Olie\/ortex   Analytics"
        self.ax.coastlines()
        self.ax.add_feature(cfeature.STATES)

        # Olievortex Analytics
        x_range = np.linspace(self.extent[0], self.extent[2], 1000)
        y_range = np.linspace(self.extent[1], self.extent[3], 1000)
        font_size = 32 * self.height / 720
        self.ax.text(x_range[3], y_range[16], branding, transform=ccrs.Geodetic(),
            fontsize=font_size, fontfamily="Spicy Rice", color='black',
            zorder=9)
        self.ax.text(x_range[2], y_range[18], branding, transform=ccrs.Geodetic(),
            fontsize=font_size, fontfamily="Spicy Rice", color='darkgoldenrod',
            zorder=10)

        # Title
        font_size = 12 * self.height / 720
        title = f"   {title}    {time.strftime("%Y-%b-%d %H:%M").values}z   "
        self.ax.text(x_range[0], y_range[976], title, fontsize=font_size,
                     bbox=dict(facecolor='white', edgecolor='none', alpha=1.0),
                     zorder=10)

    def save_plot(self, image_path: str):
        """Save the image to a file"""
        plt.savefig(image_path, bbox_inches='tight', pad_inches=0)

    def close_plot(self):
        """Free all memory after plot"""
        self.ax.cla()

        plt.cla()
        plt.clf()
        plt.close('all')

        del self.ax
        del self.fig

    @staticmethod
    def create_cdict(color_table):
        """Create a color table for pyplot"""
        reds = []
        greens = []
        blues = []

        for color in color_table:
            rgb1 = col.to_rgb(color[1])
            rgb2 = col.to_rgb(color[2])
            reds.append((color[0], rgb1[0], rgb2[0]))
            greens.append((color[0], rgb1[1], rgb2[1]))
            blues.append((color[0], rgb1[2], rgb2[2]))

        cdict = {
            'red': reds,
            'green': greens,
            'blue': blues
            }

        return cdict

    @staticmethod
    def resample_hvb(hvb: HeightValueBarb, step) -> HeightValueBarb:
        """De-densify the grid"""
        return OliePlotter.HeightValueBarb(\
            hvb.h[::step, ::step], \
            hvb.x[::step, ::step], \
            hvb.u[::step, ::step], \
            hvb.v[::step, ::step], \
            hvb.lats[::step, ::step], \
            hvb.lons[::step, ::step]
        )

    #endregion

    def plot_300_wind(self, v: HeightValueBarb):
        """Plot the 300mb wind speed chart"""
        b = OliePlotter.resample_hvb(v, 25)

        v.h = gaussian_filter(v.h, 2)
        v.x = gaussian_filter(v.x, 2)

        if np.max(v.x) > 245:
            raise ValueError(f"Wind {np.max(v.x)} exceedes handled range")
        if (np.min(v.h) < 810  or np.max(v.h) > 1200):
            raise ValueError(f"Height exceeds handled range {np.min(v.h)} {np.max(v.h)}")

        windf = np.linspace(70, 245, 10)
        windc = (0, 70)
        heights = np.linspace(810, 1200, 78)
        plt.contourf(v.lons, v.lats, v.x, windf, cmap='nipy_spectral', extend='max', alpha=0.25,
                     transform=self.transform)
        plt.contour(v.lons, v.lats, v.x, windc, transform=self.transform, colors='darkblue')
        plt.contour(v.lons, v.lats, v.h, heights, colors='k', alpha=0.25, linewidths=2,
                    transform=self.transform)
        plt.barbs(b.lons, b.lats, b.u, b.v, alpha=0.5, transform=self.transform)

    def plot_500_wind(self, v: HeightValueBarb, m: xr.Dataset):
        """Plot the 500mb wind speed chart"""
        b = OliePlotter.resample_hvb(v, 25)

        v.x = gaussian_filter(v.x, 2)
        v.h = gaussian_filter(v.h, 2)
        m = gaussian_filter(m, 2)

        if (np.min(v.h) < 455  or np.max(v.h) > 620):
            raise ValueError(f"Height exceeds handled range {np.min(v.h)} {np.max(v.h)}")

        temp = np.linspace(-100, 100, 101)
        heights = np.linspace(455, 620, 34)
        windf = np.linspace(40, 160, 7)
        windc = (0, 40)
        self.ax.set_axis_on()
        plt.contourf(v.lons, v.lats, m, windf, cmap='nipy_spectral', extend='max', alpha=0.25,
                     transform=self.transform)
        plt.contour(v.lons, v.lats, m, windc, colors='darkblue', transform=self.transform)
        plt.contour(v.lons, v.lats, v.h, heights, colors='k', linewidths=2,
                    transform=self.transform)
        plt.barbs(b.lons, b.lats, b.u, b.v, alpha=0.5, transform=self.transform)
        temps = plt.contour(v.lons, v.lats, v.x, temp, colors='red', transform=self.transform)
        self.ax.clabel(temps, temps.levels, inline=True, fontsize=10)

    def plot_500_vorticity(self, v: HeightValue):
        """Plot the 500mb absolute vorticity chart"""
        v.x = gaussian_filter(v.x, 2)
        v.h = gaussian_filter(v.h, 2)

        if (np.min(v.h) < 455  or np.max(v.h) > 620):
            raise ValueError(f"Height {v.h} exceeds handled range")

        heights = np.linspace(455, 620, 34)
        vorts = np.linspace(-100, 100, 101)
        plt.contour(v.lons, v.lats, v.h, heights, colors='k', linewidths=2,
                    transform=self.transform)
        plt.contour(v.lons, v.lats, v.x, vorts, colors='red', transform=self.transform)

    def plot_700_rh(self, v: HeightValueBarb):
        """Plot the 700mb relative humidity chart"""
        b = OliePlotter.resample_hvb(v, 25)

        v.x = gaussian_filter(v.x, 2)
        v.h = gaussian_filter(v.h, 2)

        if (np.min(v.h) < 210  or np.max(v.h) > 350):
            raise ValueError(f"Height exceeds handled range {np.min(v.h)} {np.max(v.h)}")

        rh = np.linspace(0, 100, 101)
        heights = np.linspace(210, 350, 29)
        plt.contourf(v.lons, v.lats, v.x, rh, cmap='gray', transform=self.transform)
        plt.contour(v.lons, v.lats, v.h, heights, colors='k', alpha=0.25, linewidths=2,
                    transform=self.transform)
        plt.barbs(b.lons, b.lats, b.u, b.v, alpha=0.5, transform=self.transform)

    def plot_700_temp(self, v: HeightValueBarb):
        """Plot the 700mb temperature chart"""
        b = OliePlotter.resample_hvb(v, 25)

        v.x = gaussian_filter(v.x, 2)
        v.h = gaussian_filter(v.h, 2)
        v.x = np.clip(v.x, -30, 42)

        if (np.min(v.h) < 210  or np.max(v.h) > 350):
            raise ValueError(f"Height exceeds handled range {np.min(v.h)} {np.max(v.h)}")

        temp = np.linspace(-30, 42, 73)
        self.ax.set_axis_on()
        temps = plt.contour(v.lons, v.lats, v.x, temp, cmap='nipy_spectral', linestyles='dashed',
                            transform=self.transform)
        self.ax.clabel(temps, temps.levels, inline=True, fontsize=10)

        # height contour every 5 gpm
        heights = np.linspace(210, 350, 29)
        plt.contour(v.lons, v.lats, v.h, heights, colors='k', linewidths=2,
                    transform=self.transform)
        plt.barbs(b.lons, b.lats, b.u, b.v, alpha=0.5, transform=self.transform)

    def plot_850_wind(self, v: HeightValueBarb, m: xr.Dataset):
        """Plot the 850mb temperature chart"""
        b = OliePlotter.resample_hvb(v, 25)

        m = gaussian_filter(m, 2)
        v.h = gaussian_filter(v.h, 2)
        v.x = gaussian_filter(v.x, 2)
        m = np.clip(m, 0, 100)

        if (np.min(v.h) < 67  or np.max(v.h) > 200):
            raise ValueError(f"Height exceeds handled range {np.min(v.h)} {np.max(v.h)}")

        windf = np.linspace(25, 100, 16)
        windc = (0, 25)
        heights = np.linspace(67, 200, 39)
        temp = np.linspace(-100, 100, 101)
        self.ax.set_axis_on()
        plt.contourf(v.lons, v.lats, m, windf, cmap='nipy_spectral', extend='max', alpha=0.25,
                     transform=self.transform)
        plt.contour(v.lons, v.lats, m, windc, colors='darkblue', transform=self.transform)
        plt.contour(v.lons, v.lats, v.h, heights, colors='k', linewidths=2,
                    transform=self.transform)
        plt.barbs(b.lons, b.lats, b.u, b.v, alpha=0.5, transform=self.transform)
        temps = plt.contour(v.lons, v.lats, v.x, temp, colors='red', linestyles='dashed',
                            transform=self.transform)
        self.ax.clabel(temps, temps.levels, inline=True, fontsize=10)

    def plot_cape(self, v: HeightValue):
        """Plot the CAPE chart"""
        v.x = gaussian_filter(v.x, 2)
        v.x = np.clip(v.x, 0, 3000)

        color_table = [
            (0.0, 'white', 'white'),
            (800.0 / 3000, 'white', 'gray'),
            (1000.0 / 3000, 'lightgray', 'olive'),
            (2000.0 / 3000, 'yellow', 'red'),
            (3000.0 / 3000, 'hotpink', 'hotpink')
        ]
        cape_map = col.LinearSegmentedColormap('olietemp', N=256,
                                               segmentdata=OliePlotter.create_cdict(color_table))

        capef = np.linspace(0, 3000, 61)
        capec = (-10, 800, 1000, 2000, 3000)
        plt.contourf(v.lons, v.lats, v.x, capef, cmap=cape_map, transform=self.transform)
        if np.max(v.x) > 800.0:
            plt.contour(v.lons, v.lats, v.x, capec, colors='darkred', transform=self.transform)

    def plot_cin(self, v: HeightValue):
        """Plot the CIN chart"""
        v.x = np.multiply(v.x, -1)
        v.x = gaussian_filter(v.x, 2)
        v.x = np.clip(v.x, 1, 300)
        v.h = gaussian_filter(v.h, 2)

        v.x[v.h < 1000] = -1

        color_table = [
            (0.00, 'gray', 'gray'),
            (100.0 / 300, 'lightgray', 'red'),
            (200.0 / 300, 'yellow', 'lightsteelblue'),
            (300.0 / 300, 'blue', 'blue')
        ]
        dt_map = col.LinearSegmentedColormap('olietemp', N=256,
                                             segmentdata=OliePlotter.create_cdict(color_table))

        cinf = np.linspace(0, 300, 61)
        cinc = (-1, 100, 200)
        plt.contourf(v.lons, v.lats, v.x, cinf, cmap=dt_map, transform=self.transform)
        if np.max(v.x) > 100.0:
            plt.contour(v.lons, v.lats, v.x, cinc, colors='k', transform=self.transform)

    def plot_lcl(self, v: HeightValue):
        """Plot the Olie Lifted Condensation Level chart"""
        v.x = gaussian_filter(v.x, 2)
        v.x = np.clip(v.x, 1, 3000)
        v.h = gaussian_filter(v.h, 2)

        v.x[v.h < 1000] = -1

        color_table = [
            (0.00, 'gray', 'gray'),
            (500.0 / 3000.0 , 'lightgray', 'red'),
            (800.0 / 3000.0, 'salmon', 'olive'),
            (2000.0 / 3000.0, 'yellow', 'lightsteelblue'),
            (3000.0 / 3000.0, 'blue', 'blue'),
        ]
        tmap = col.LinearSegmentedColormap('olietemp', N=256,
                                           segmentdata=OliePlotter.create_cdict(color_table))

        lclf = np.linspace(0, 3000, 61)
        lclc = (-1, 500, 2000)
        plt.contourf(v.lons, v.lats, v.x, lclf, cmap=tmap, transform=self.transform)
        if np.max(v.x) > 500.0:
            plt.contour(v.lons, v.lats, v.x, lclc, colors='darkred', transform=self.transform)

    def plot_scp(self, v: HeightValue):
        """Plot the supercell parameter"""
        v.x = gaussian_filter(v.x, 2)
        v.x = np.clip(v.x, 0, 2)

        color_table = [
            (0.0, 'white', 'white'),
            (0.8 / 2, 'white', 'gray'),
            (1.0 / 2, 'lightgray', 'red'),
            (2.0 / 2, 'hotpink', 'hotpink')
        ]
        tmap = col.LinearSegmentedColormap('olietemp', N=256,
                                           segmentdata=OliePlotter.create_cdict(color_table))

        scpf = np.linspace(0, 2, 41)
        scpc = (-10, 0.8, 1.0)
        plt.contourf(v.lons, v.lats, v.x, scpf, cmap=tmap, transform=self.transform)
        if np.max(v.x) > 0.8:
            plt.contour(v.lons, v.lats, v.x, scpc, colors='darkred', transform=self.transform)

    def plot_shear(self, v: HeightValueBarb):
        """Plot the Olie Shear chart"""
        b = OliePlotter.resample_hvb(v, 25)

        v.h = gaussian_filter(v.h, 2)
        v.x = gaussian_filter(v.x, 2)
        v.x = np.clip(v.x, 0, 100)

        if (np.min(v.h) < 455  or np.max(v.h) > 620):
            raise ValueError(f"Height exceeds handled range  {np.min(v.h)} {np.max(v.h)}")

        shearf = np.linspace(35, 100, 14)
        shearc = (-10, 35)
        heights = np.linspace(455, 620, 34)
        plt.contourf(v.lons, v.lats, v.x, shearf, cmap='nipy_spectral', alpha=0.25,
                     transform=self.transform)
        shearp = plt.contour(v.lons, v.lats, v.x, shearf, cmap='nipy_spectral',
                             linestyles='dashed', transform=self.transform)
        self.ax.clabel(shearp, shearp.levels, inline=True, fontsize=10)
        plt.contour(v.lons, v.lats, v.x, shearc, colors='darkblue', transform=self.transform)
        plt.contour(v.lons, v.lats, v.h, heights, colors='k', transform=self.transform)
        plt.barbs(b.lons, b.lats, b.u, b.v, alpha=0.5, transform=self.transform)

    def plot_srh(self, v: HeightValue):
        """Plot the SRH chart"""
        v.x = gaussian_filter(v.x, 2)
        v.x = np.clip(v.x, 0, 500)

        srh = np.linspace(150, 500, 8)
        self.ax.set_axis_on()
        plt.contourf(v.lons, v.lats, v.x, srh, cmap='nipy_spectral', alpha=0.25,
                     transform=self.transform)
        srhc = plt.contour(v.lons, v.lats, v.x, srh, cmap='nipy_spectral', linestyles='dashed',
                           transform=self.transform)
        self.ax.clabel(srhc, srhc.levels, inline=True, fontsize=10)

    def plot_storm_motion(self, v: HeightValueBarb):
        """Plot the right handed storm motion chart"""
        b = OliePlotter.resample_hvb(v, 12)
        v.x = gaussian_filter(v.x, 2)
        v.x = np.clip(v.x, 0, 50)

        mag = np.linspace(0, 50, 11)
        plt.contourf(v.lons, v.lats, v.x, mag, cmap='terrain', alpha=0.25,
                     transform=self.transform)
        plt.contour(v.lons, v.lats, v.x, mag, cmap='terrain', transform=self.transform)
        plt.barbs(b.lons, b.lats, b.u, b.v, alpha=0.5, transform=self.transform)

    def plot_stp(self, v: HeightValue):
        """Plot the significant tornado parameter"""
        v.x = gaussian_filter(v.x, 2)
        v.x = np.clip(v.x, 0, 2)

        color_table = [
            (0.0, 'white', 'white'),
            (0.8 / 2, 'white', 'gray'),
            (1.0 / 2, 'lightgray', 'red'),
            (2.0 / 2, 'hotpink', 'hotpink')
        ]
        tmap = col.LinearSegmentedColormap('olietemp', N=256,
                                           segmentdata=OliePlotter.create_cdict(color_table))

        stpf = np.linspace(0, 2, 41)
        stpc = (-10, 0.8, 1.0)
        plt.contourf(v.lons, v.lats, v.x, stpf, cmap=tmap, transform=self.transform)
        if np.max(v.x) > 0.8:
            plt.contour(v.lons, v.lats, v.x, stpc, colors='darkred', transform=self.transform)

    def plot_surface_dewpoint(self, v: HeightValueBarb):
        """Plot the surface dewpoint chart"""
        b = OliePlotter.resample_hvb(v, 25)

        v.x = gaussian_filter(v.x, 2)
        v.h = gaussian_filter(v.h, 2)
        v.x = np.clip(v.x, 0, 100)

        if (np.min(v.h) < 930  or np.max(v.h) > 1100):
            raise ValueError(f"MSLP exceeds handled range {np.min(v.h)} {np.max(v.h)}")

        color_table = [
            (0.00, 'white', 'white'),
            (0.50, 'whitesmoke', 'yellowgreen'),
            (0.55, 'yellowgreen', 'yellow'),
            (0.60, 'yellow', 'tomato'),
            (0.65, 'tomato', 'red'),
            (0.70, 'red', 'violet'),
            (0.75, 'violet', 'fuchsia'),
            (0.80, 'fuchsia', 'hotpink'),
            (1.00, 'hotpink', 'hotpink')
        ]
        dt_map = col.LinearSegmentedColormap('olietemp', N=256,
                                             segmentdata=OliePlotter.create_cdict(color_table))

        dtfill = np.linspace(0, 100, 21)
        dtcont = (0, 50, 60, 70, 80)
        mslps = np.linspace(930, 1100, 69)
        plt.contourf(v.lons, v.lats, v.x, dtfill, cmap=dt_map, transform=self.transform)
        plt.contour(v.lons, v.lats, v.x, dtcont, colors='darkred', transform=self.transform)
        plt.contour(v.lons, v.lats, v.h, mslps, colors='k', alpha=0.25, linewidths=2,
                    transform=self.transform)
        plt.barbs(b.lons, b.lats, b.u, b.v, alpha=0.5, transform=self.transform)

    def plot_surface_temperature(self, v: HeightValueBarb):
        """Plot the surface temperature chart"""
        b = OliePlotter.resample_hvb(v, 25)

        v.x = gaussian_filter(v.x, 2)
        v.h = gaussian_filter(v.h, 2)
        v.x = np.clip(v.x, 0, 100)

        if (np.min(v.h) < 930  or np.max(v.h) > 1100):
            raise ValueError(f"MSLP exceeds handled range {np.min(v.h)} {np.max(v.h)}")

        color_table = [
            (0.00, 'white', 'white'),
            (0.70, 'whitesmoke', 'yellowgreen'),
            (0.75, 'yellowgreen', 'yellow'),
            (0.80, 'yellow', 'tomato'),
            (0.85, 'tomato', 'red'),
            (0.90, 'red', 'violet'),
            (0.95, 'violet', 'fuchsia'),
            (1.00, 'fuchsia', 'hotpink')
        ]
        t_map = col.LinearSegmentedColormap('olietemp', N=256,
                                            segmentdata=OliePlotter.create_cdict(color_table))

        tfill = np.linspace(0, 100, 21)
        tcont = (0, 70, 80, 90)
        mslps = np.linspace(930, 1100, 69)
        plt.contourf(v.lons, v.lats, v.x, tfill, cmap=t_map, transform=self.transform)
        plt.contour(v.lons, v.lats, v.x, tcont, colors='darkred', transform=self.transform)
        plt.contour(v.lons, v.lats, v.h, mslps, colors='k', alpha=0.25, linewidths=2,
                    transform=self.transform)
        plt.barbs(b.lons, b.lats, b.u, b.v, alpha=0.5, transform=self.transform)
