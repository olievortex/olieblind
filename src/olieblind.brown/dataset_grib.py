""" Grib reader """
from datetime import datetime
import numpy as np
import xarray as xr
import metpy.calc as mpcalc

class DatasetGrib:
    """ Grib reader """
    def __init__(self, grib_path: str):
        self.grib_path = grib_path
        self.engine = 'cfgrib'

    #region Selectors

    def select_by_hag(self, level: int) -> xr.Dataset:
        """Get paremeters for a height above ground level"""
        keys = {'typeOfLevel': 'heightAboveGround', 'level': level}
        args = {'filter_by_keys': keys}
        result = xr.open_dataset(self.grib_path, engine=self.engine, backend_kwargs=args,
                                 decode_timedelta=False)

        return result

    def select_by_hagl(self, level: int) -> xr.Dataset:
        """Get paremeters for a height above ground level"""
        keys = {'typeOfLevel': 'heightAboveGroundLayer', 'level': level}
        args = {'filter_by_keys': keys}
        result = xr.open_dataset(self.grib_path, engine=self.engine, backend_kwargs=args,
                                 decode_timedelta=False)

        return result

    def select_by_isobar(self, level: int) -> xr.Dataset:
        """Get parameters for an isobaric level"""
        keys = {'typeOfLevel': 'isobaricInhPa', 'level': level}
        args = {'filter_by_keys': keys}
        result = xr.open_dataset(self.grib_path, engine=self.engine, backend_kwargs=args,
                                 decode_timedelta=False)

        return result

    def select_by_ms(self) -> xr.Dataset:
        """Get paremeters for a height above ground level"""
        keys = {'typeOfLevel': 'meanSea'}
        args = {'filter_by_keys': keys}
        result = xr.open_dataset(self.grib_path, engine=self.engine, backend_kwargs=args,
                                 decode_timedelta=False)

        return result

    def select_by_surface(self, step_type: str) -> xr.Dataset:
        """Get paremeters for a height above ground level. Step type is either
        'accum' or 'instant'."""
        keys = {'typeOfLevel': 'surface', 'stepType': step_type}
        args = {'filter_by_keys': keys}
        result = xr.open_dataset(self.grib_path, engine=self.engine, backend_kwargs=args,
                                 decode_timedelta=False)

        return result

    #endregion

#region Derived

def derive_bulk_shear_u(grib10: xr.Dataset, grib500: xr.Dataset) -> xr.DataArray:
    """Derive the bulk shear u component using calculation"""
    u500 = convert_u_knots(grib500)
    u10 = convert_u10_knots(grib10)

    u = u500 - u10

    return u

def derive_bulk_shear_v(grib10: xr.Dataset, grib500: xr.Dataset) -> xr.DataArray:
    """Derive the bulk shear v component using calculation"""
    v500 = convert_v_knots(grib500)
    v10 = convert_v10_knots(grib10)

    v = v500 - v10

    return v

def derive_olie_lcl(grib2: xr.Dataset) -> xr.DataArray:
    """Derive the lifted condensation level with a basic algorithm"""
    t = convert_t2m_f(grib2)
    td = convert_d2m_f(grib2)

    lcl = np.multiply(t - td, 125)

    return lcl

def derive_olie_scp(grib_instant: xr.Dataset, grib10: xr.Dataset,
                    grib500: xr.Dataset, grib3000: xr.Dataset):
    """Derive the supercell parameter with an algorithm"""
    u = derive_bulk_shear_u(grib10, grib500)
    v = derive_bulk_shear_v(grib10, grib500)
    bs = convert_uv_magnitude(u, v).metpy.dequantify()
    bs = np.divide(bs, 40)
    bs_shape = bs.shape
    bs = np.ravel(bs)
    bs[bs < 0.5] = 0
    bs[bs > 1.0] = 1
    bs = np.reshape(bs, bs_shape)

    cape = get_cape(grib_instant)
    cape = np.divide(cape, 1000)

    srh = get_srh(grib3000)
    srh = np.divide(srh, 125)

    scp = cape * srh * bs

    return scp

def derive_olie_rh_storm_motion(grib10: xr.Dataset, grib500: xr.Dataset,
                                grib700: xr.Dataset, grib850: xr.Dataset) -> \
tuple[xr.DataArray, xr.DataArray]:
    """Derive the right-handed storm motion vector with an algorithm"""
    # 30R75 Method, Take 75% of the 0-6km average vector
    u = (convert_u_knots(grib500) + convert_u_knots(grib700) + \
        convert_u_knots(grib850) + convert_u10_knots(grib10)) / 4.0 * 0.75
    v = (convert_v_knots(grib500) + convert_v_knots(grib700) + \
        convert_v_knots(grib850) + convert_v10_knots(grib10)) / 4.0 * 0.75

    # Turn the vector 30 degrees left
    left = 30.0 * np.pi / 180.0

    u3 = u * np.cos(-left) - v * np.sin(-left)
    v3 = u * np.sin(-left) + v * np.cos(-left)

    return u3, v3

def derive_olie_stp(grib_instant: xr.Dataset, grib2: xr.Dataset, grib10: xr.Dataset,
                    grib500: xr.Dataset, grib3000: xr.Dataset) -> xr.DataArray:
    """Derive the significant tornado parameter with an algorithm"""
    cape = get_cape(grib_instant)
    cape = np.divide(cape, 1500)

    srh = get_srh(grib3000)
    srh = np.divide(srh, 375)

    lcl = derive_olie_lcl(grib2).metpy.dequantify()
    lcl = np.divide(np.subtract(2000, lcl), 1000)
    lcl = np.clip(lcl, 1, 2)

    u = derive_bulk_shear_u(grib10, grib500)
    v = derive_bulk_shear_v(grib10, grib500)
    bs = convert_uv_magnitude(u, v).metpy.dequantify()
    bs = np.divide(bs, 40)
    bs_shape = bs.shape
    bs = np.ravel(bs)
    bs[bs < 0.625] = 0
    bs[bs > 1.5] = 1.5
    bs = np.reshape(bs, bs_shape)

    stp = cape * srh * bs * lcl

    return stp

#endregion

#region Getters

def get_cape(grib_instant: xr.Dataset) -> xr.DataArray:
    """Get the CAPE variable"""
    return grib_instant['cape']

def get_cin(grib_instant: xr.Dataset) -> xr.DataArray:
    """Get the CIN variable"""
    return grib_instant['cin']

def get_lat(grib: xr.Dataset) -> xr.DataArray:
    """Get the latitude arrays"""
    return grib['latitude']

def get_lon(grib: xr.Dataset) -> xr.DataArray:
    """Get the longitude arrays"""
    return grib['longitude']

def get_rh(grib: xr.Dataset) -> xr.DataArray:
    """Get the relative humidity arrays"""
    return grib['r']

def get_srh(grib_instant: xr.Dataset) -> xr.DataArray:
    """Get the storm relative helicity arrays"""
    return grib_instant['hlcy']

def get_timestamp(grib: xr.Dataset) -> datetime:
    """Get the timestamp from the dataset"""
    return grib['valid_time'].dt

#endregion

#region Conversion

def convert_d2m_f(grib: xr.Dataset) -> xr.DataArray:
    """Convert dewpoint to F"""
    dt = grib['d2m'].metpy.convert_units('degF')

    return dt

def convert_gh_dam(grib: xr.Dataset) -> xr.DataArray:
    """Get the geopotential height in dam"""
    h = grib['gh'].metpy.convert_units('dam')

    return h

def convert_prmsl_mbar(grib: xr.Dataset) -> xr.DataArray:
    """Convert mslp to mbar"""
    mslp =  grib['prmsl'].metpy.convert_units('mbar')

    return mslp

def convert_t2m_f(grib2: xr.Dataset) -> xr.DataArray:
    """Convert dewpoint to F"""
    dt = grib2['t2m'].metpy.convert_units('degF')

    return dt

def convert_temp_c(grib2: xr.Dataset) -> xr.DataArray:
    """Convert temperature to C"""
    t = grib2['t'].metpy.convert_units('degC')

    return t

def convert_u10_knots(grib10: xr.Dataset) -> xr.DataArray:
    """Convert u to knots"""
    u = grib10['u10'].metpy.convert_units('knots')

    return u

def convert_u_knots(grib: xr.Dataset) -> xr.DataArray:
    """Convert u to knots"""
    u = grib['u'].metpy.convert_units('knots')

    return u

def convert_uv_magnitude(u: xr.Dataset, v: xr.Dataset) -> xr.DataArray:
    """Convert u, v to magnitude"""
    m = mpcalc.wind_speed(u, v)

    return m

def convert_v10_knots(grib10: xr.Dataset) -> xr.DataArray:
    """Convert v to knots"""
    v = grib10['v10'].metpy.convert_units('knots')

    return v

def convert_v_knots(grib: xr.Dataset) -> xr.DataArray:
    """Convert v to knots"""
    v = grib['v'].metpy.convert_units('knots')

    return v

def convert_vorticity(grib: xr.Dataset) -> xr.DataArray:
    """Convert vorticity to remove zeroes"""
    av = grib['absv'] * 100000.0

    return av

#endregion
