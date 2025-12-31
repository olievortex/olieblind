namespace olieblind.lib.ForecastModels;

public static class Products
{
    public static readonly List<Product> StandardProducts = [
        new Product(1, "Dewpoint", "Surface Dewpoints (Dewpoint, MSLP, Barbs)"),
        new Product(2, "Temperature", "Surface Temperature (Temperature, MSLP, Barbs)"),
        new Product(3, "Vorticity", "500mb (Height, Absolute Vorticity)"),
        new Product(4, "300mb", "300mb (Height, Wind, Barbs)"),
        new Product(5, "500mb", "500mb (Height, Wind, Temp, Barbs)"),
        new Product(6, "700mbR", "700mb (Height, Rh, Barbs)"),
        new Product(7, "700mbT", "700mb (Height, Temp, Barbs)"),
        new Product(8, "850mb", "850mb (Height, Wind, Temp)"),
        new Product(9, "Cape", "Surface Based CAPE"),
        new Product(10, "Cin", "Surface Based CIN (Masked by Cape < 1000)"),
        new Product(11, "Srh", "3km Storm Relative Helicity"),
        new Product(12, "Shear", "Olie's Bulk Shear (Surface to 500mb Shear, 500mb Height)"),
        new Product(13, "Lcl", "Olie's LCL Estimate (Masked by Cape < 1000)"),
        new Product(14, "Stp", "Olie's Significant Tornado Composite"),
        new Product(15, "Scp", "Olie's Supercell Composite"),
        new Product(16, "StormMotion", "Olie's Storm Motion (Right Mover)")
    ];
}